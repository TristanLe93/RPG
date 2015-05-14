using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The BattleController maintains the flow of combat.
/// </summary>
public class BattleController : MonoBehaviour {
	// the controller of the UI
	public BattleUIController UIController;

	public List<BattleCombatant> Players;
	public List<BattleCombatant> Enemies;

	// The positions of each combatant should be
	public List<Transform> PlayerPos;
	public List<Transform> EnemyPos;

	private bool battleActive = true;

	private Ability selectedAbility;
	private BattleCombatant selectedTarget;
	private BattleCombatant lastSelectedPlayer;
	private List<BattleCombatant> targetList = new List<BattleCombatant>();

	private List<BattleCombatant> turnList = new List<BattleCombatant>();
	private List<BattleCombatant> playerRanks;
	private List<BattleCombatant> enemyRanks;
	
	private BattleCombatant currentCombatant;
	private Ability swap;
	

	private void Start () {
		// automatically start a battle
		StartCoroutine(UpdateState());
	}

	// Start the battle and maintain its states
	private IEnumerator UpdateState() {
		InitialiseBattle();

		while (battleActive) {
			// reset all components before issuing a turn
			UIController.ResetButtons();
			currentCombatant = turnList[0];
			selectedAbility = null;
			selectedTarget = null;

			// perform status effects on the current combatant's turn
			bool combatantStunned = currentCombatant.IsStunned();
			currentCombatant.DoStatusEffects();


			if (currentCombatant.IsDead) {
				EndTurn();
			} 
			else if (combatantStunned) {
				UIController.ShowAbilityName("Stunned!");
				currentCombatant.ObjectUI.ShowTurnIcon();
				yield return new WaitForSeconds(1.5f);
				EndTurn();
			} 
			else if (currentCombatant is PlayerCombatant) {
				yield return StartCoroutine(PlayerTurn());
			} 
			else {
				yield return StartCoroutine(EnemyTurn());
			}
		}
	}

	/// <summary>
	/// Set up all components in the battle engine.
	/// </summary>
	private void InitialiseBattle() {
		// initialise Swap ability
		swap = ScriptableObject.CreateInstance<Ability>();
		swap.TargetType = AbilityTarget.Players;
		swap.UseableRanks = Enumerable.Repeat(true, 4).ToList();

		// setup players and enemies
		playerRanks = Players;
		enemyRanks = Enemies;
		turnList.AddRange(Players);
		turnList.AddRange(Enemies);

		// shuffle turnList
		for (int i = 0; i < turnList.Count; i++) {
			BattleCombatant temp = turnList[i];
			int randomIndex = Random.Range(i, turnList.Count);
			turnList[i] = turnList[randomIndex];
			turnList[randomIndex] = temp;
		}

		// setup HUD
		lastSelectedPlayer = Players[0];
		UIController.UpdateUI(lastSelectedPlayer.Name, lastSelectedPlayer.Abilities);
		UIController.UpdateHealthBar(lastSelectedPlayer.Stats.Health);
		UIController.DisableButtons();
	}

	/// <summary>
	/// Start the player's turn.
	/// </summary>
	private IEnumerator PlayerTurn() {
		PlayerCombatant player = (PlayerCombatant)currentCombatant;
		lastSelectedPlayer = player;
		int rank = playerRanks.IndexOf(currentCombatant);

		// Update Battle UI
		player.ObjectUI.ShowTurnIcon();
		UIController.UpdateUI(player.Name, player.Abilities);
		UIController.UpdateHealthBar(player.Stats.Health);
		UIController.UpdateStatusEffectsIcons(player.StatusEffects);
		UIController.EnableButtons(player.Abilities, rank);

		// select an ability and use it on the target(s)
		yield return StartCoroutine(SelectAbilityAndTarget());

		if (selectedAbility == swap) {
			SwapRanks(player, selectedTarget);
			yield return new WaitForSeconds(1.5f);
		} 
		else {
			yield return StartCoroutine(player.UseAbility(selectedAbility, targetList));
		}

		// update current player after all effects been used
		UIController.UpdateHealthBar(player.Stats.Health);
		UIController.UpdateStatusEffectsIcons(player.StatusEffects);

		EndTurn();
	}

	/// <summary>
	/// Start the enemies' turn
	/// </summary>
	private IEnumerator EnemyTurn() {
		EnemyCombatant enemy = (EnemyCombatant)currentCombatant;
		enemy.ObjectUI.ShowTurnIcon();
		UIController.DisableButtons();

		// wait for any damage/heal animations
		do { 
			yield return null;
		} while (enemy.isAnimating());

		yield return StartCoroutine(enemy.BattleAI(UIController, Players));
		yield return StartCoroutine(enemy.WaitForTargetAnimation());

		EndTurn();
	}

	/// <summary>
	/// End the current turn. 
	/// Check if the either party is dead and award the winner.
	/// </summary>
	private void EndTurn() {
		if (IsPartyDead(Players)) {
			battleActive = false;
			EnemyVictory();
		}
		else if (IsPartyDead(Enemies)) { 
			battleActive = false;
			PlayerVictory();
		} 
		else {
			currentCombatant.ObjectUI.HideTurnIcon();
			targetList.Clear();

			ShuffleDeadInParty(playerRanks, PlayerPos);
			ShuffleDeadInParty(enemyRanks, EnemyPos);

			turnList.Add(currentCombatant);
			turnList.RemoveAt(0);
		}
	}

	/// <summary>
	/// Wait for the player to selected an ability and target.
	/// </summary>
	private IEnumerator SelectAbilityAndTarget() {
		while (!selectedAbility || !selectedTarget) {
			if (Input.GetMouseButtonUp(0) && selectedAbility) {
				selectedTarget = FindTarget();
			}
			
			yield return null;
		}

		if (!selectedAbility.MultiTarget) {
			targetList.Clear();
			targetList.Add(selectedTarget);
		}

		HideTargets();
		UIController.DisableButtons();
	}
	
	/// <summary>
	/// Use Raycast to detect clickable target and return it.
	/// </summary>
	private BattleCombatant FindTarget() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);
		
		if (hit.collider) {
			BattleCombatant target = hit.collider.gameObject.GetComponent<BattleCombatant>();
			if (target) {
				if (IsTargetValid(target)) {
					return target;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Determines whether the target is valid for the selectedAbility.
	/// </summary>
	private bool IsTargetValid(BattleCombatant target) {
		// if the target is an enemy
		if (selectedAbility.TargetType == AbilityTarget.Enemies && Enemies.Contains(target)) {
			int rank = enemyRanks.IndexOf(target);
			int numDead = NumDeadInParty(Enemies);

			// if the back rank is selectable, then the back rank is shifted
			// on how many members are dead in the party.
			if (numDead > 0 && selectedAbility.TargetableRanks[3]) {
				List<bool> newTargetableList = new List<bool>(selectedAbility.TargetableRanks);
	
				// shift targetable combatants
				while (numDead > 0) {
					newTargetableList.RemoveAt(0);
					newTargetableList.Add(true);
					numDead--;
				}

				return newTargetableList[rank] && !target.IsDead;
			}

			return selectedAbility.TargetableRanks[rank] && !target.IsDead;
		} 
		// targeting players
		else if (selectedAbility.TargetType == AbilityTarget.Players && Players.Contains(target)) {
			int rank = playerRanks.IndexOf(target);
			int numDead = NumDeadInParty(Players);
			
			// if back rank, then push the targetable further
			if (numDead > 0 && selectedAbility.TargetableRanks[3]) {
				List<bool> newTargetableList = new List<bool>(selectedAbility.TargetableRanks);
				
				while (numDead > 0) {
					newTargetableList.RemoveAt(0);
					newTargetableList.Add(true);
					numDead -= 1;
				}
				
				return newTargetableList[rank] && !target.IsDead;
			}

			return selectedAbility.TargetableRanks[rank] && !target.IsDead;
		} 
		// targeting self
		else if (selectedAbility.TargetType == AbilityTarget.Self) {
			return currentCombatant == target;
		}

		return false;
	}

	/// <summary>
	/// Determines whether the party is dead.
	/// </summary>
	private bool IsPartyDead(List<BattleCombatant> party) {
		return party.Find((BattleCombatant b) => b.IsDead == false) == null;
	}

	/// <summary>
	/// Swaps the positions of the current player with another party member.
	/// Only usable with player's party.
	/// </summary>
	private void SwapRanks(BattleCombatant current, BattleCombatant target) {
		int currentRank = playerRanks.IndexOf(current);
		int targetRank = playerRanks.IndexOf(target);

		// swap Ranks of current player with target in PlayerRanks
		BattleCombatant tmp = playerRanks[targetRank];
		playerRanks[targetRank] = playerRanks[currentRank];
		playerRanks[currentRank] = tmp;

		// swap transform positions of current player with target
		Vector3 currentPos = Players[currentRank].transform.position;
		Vector3 targetPos = Players[targetRank].transform.position;
		Players[currentRank].transform.position = targetPos;
		Players[targetRank].transform.position = currentPos;
	}

	/// <summary>
	/// Shuffles dead members to the back of the party
	/// </summary>
	private void ShuffleDeadInParty(List<BattleCombatant> partyRanks, List<Transform> partyPos) {
		List<BattleCombatant> deadBodies = new List<BattleCombatant>();

		// remove the dead bodies from party
		for (int rank = 0; rank < partyRanks.Count; rank++) {
			if (partyRanks[rank].IsDead) {
				deadBodies.Add(partyRanks[rank]);
				partyRanks.RemoveAt(rank);
				rank--;
			}
		}

		// add dead bodies back to party
		if (deadBodies.Count > 0) {
			partyRanks.AddRange(deadBodies);

			// recalculate the positions of each member in party
			for (int i = 0; i < partyRanks.Count; i++) {
				partyRanks[i].transform.position = partyPos[i].position;
			}
		}
	}

	/// <summary>
	/// Number of dead combatants in the party
	/// </summary>
	private int NumDeadInParty(List<BattleCombatant> party) {
		int count = 0;
		foreach (BattleCombatant combatant in party) {
			if (combatant.IsDead) count++;
      	}

		return count;
	}

	/// <summary>
	/// Shows the available targets for the selected ability.
	/// </summary>
	private void ShowTargets() {
		targetList.Clear();

		// show enemy specific targets
		if (selectedAbility.TargetType == AbilityTarget.Enemies) {
			foreach (BattleCombatant enemy in Enemies) {
				if (IsTargetValid(enemy)) {
					enemy.ObjectUI.ShowTargetArrow();
					targetList.Add(enemy);
				} 
			}
		} 
		// show player specific targets
		else if (selectedAbility.TargetType == AbilityTarget.Players) {
			foreach (BattleCombatant player in Players) {
				if (IsTargetValid(player)) {
					player.ObjectUI.ShowTargetArrow();
					targetList.Add(player);
				} 
			}
		} 
		else {
			// targeting self
			lastSelectedPlayer.ObjectUI.ShowTargetArrow();
		}
	}

	/// <summary>
	/// Hides all available targets.
	/// </summary>
	private void HideTargets() {
		foreach (BattleCombatant targets in targetList) {
			targets.ObjectUI.HideTargetArrow();
		}
	}

	/// <summary>
	/// Do stuff when the player wins. eg. experience, loot, money
	/// </summary>
	private void PlayerVictory() {
		foreach (BattleCombatant b in Players) {
			PlayerCombatant p = (PlayerCombatant)b;
			p.PlayVictoryAnim();
		}
	}

	/// <summary>
	/// Game over when the player loses
	/// </summary>
	private void EnemyVictory() {
		Debug.Log("GAMEOVER - YOU LOSE");
		//TODO: YOU LOSE
	}

	/// <summary>
	/// A UIButton method to set an ability to use.
	/// </summary>
	public void Btn_SetAbility(int index) {
		HideTargets();

		if (UIController.WhichButtonIsSelected() == index) {
			selectedAbility = currentCombatant.Abilities[index];
			ShowTargets();
		} else {
			selectedAbility = null;
		}
	}

	/// <summary>
	/// A UIButton method to set 'Swap' as the selected ability.
	/// </summary>
	public void Btn_Swap() {
		HideTargets();

		if (UIController.IsAButtonSelected()) {
			selectedAbility = swap;

			// set targetable ranks 
			swap.TargetableRanks = Enumerable.Repeat(true, 4).ToList();
			int rank = playerRanks.IndexOf(currentCombatant);
			swap.TargetableRanks[rank] = false;

			ShowTargets();
		} else {
			selectedAbility = null;
		}
	}

}
