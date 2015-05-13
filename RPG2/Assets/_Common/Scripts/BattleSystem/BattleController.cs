using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
	public BattleUIController UIController;
	public List<BattleCombatant> Players;
	public List<BattleCombatant> Enemies;
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

	private IEnumerator UpdateState() {
		InitialiseBattle();
		
		while (battleActive) {
			UIController.ResetButtons();
			currentCombatant = turnList[0];
			selectedAbility = null;
			selectedTarget = null;

			bool combatantStunned = currentCombatant.IsStunned();
			currentCombatant.DoStatusEffects();

			if (currentCombatant.IsDead) {
				EndTurn();
			} else if (combatantStunned) {
				UIController.ShowAbilityName("Stunned!");
				currentCombatant.ObjectUI.ShowTurnIcon();
				yield return new WaitForSeconds(1.5f);
				EndTurn();
			} else if (currentCombatant is PlayerCombatant) {
				yield return StartCoroutine(PlayerTurn());
			} else {
				yield return StartCoroutine(EnemyTurn());
			}
		}
	}

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
		// if a party is victorious, play victory/game over
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
	/// Wait until an ability and target has been selected
	/// </summary>
	/// <returns>The ability and target.</returns>
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
			BattleCombatant target = 
				hit.collider.gameObject.GetComponent<BattleCombatant>();

			if (target) {
				if (IsTargetValid(target)) {
					return target;
				}
			}
		}

		return null;
	}

	private bool IsTargetValid(BattleCombatant target) {
		// targeting enemies
		if (selectedAbility.TargetType == AbilityTarget.Enemies && Enemies.Contains(target)) {
			int rank = enemyRanks.IndexOf(target);
			int numDead = NumDeadInParty(Enemies);

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

		// update player transform positions
		if (deadBodies.Count > 0) {
			// add dead bodies back to party
			partyRanks.AddRange(deadBodies);

			for (int i = 0; i < partyRanks.Count; i++) {
				partyRanks[i].transform.position = partyPos[i].position;
			}
		}
	}

	private int NumDeadInParty(List<BattleCombatant> party) {
		int count = 0;
		foreach (BattleCombatant combatant in party) {
			if (combatant.IsDead) count++;
      	}

		return count;
	}

	private void ShowTargets() {
		targetList.Clear();

		if (selectedAbility.TargetType == AbilityTarget.Enemies) {
			foreach (BattleCombatant enemy in Enemies) {
				if (IsTargetValid(enemy)) {
					enemy.ObjectUI.ShowTargetArrow();
					targetList.Add(enemy);
				} 
			}
		} 
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
	
	private void HideTargets() {
		foreach (BattleCombatant e in Enemies) {
			e.ObjectUI.HideTargetArrow();
		}
		foreach (BattleCombatant p in Players) {
			p.ObjectUI.HideTargetArrow();
		}
	}

	private void PlayerVictory() {
		foreach (BattleCombatant b in Players) {
			PlayerCombatant p = (PlayerCombatant)b;
			p.PlayVictoryAnim();
		}
	}
	
	private void EnemyVictory() {
		Debug.Log("GAMEOVER - YOU LOSE");
		//TODO: YOU LOSE
	}
	
	public void Btn_SetAbility(int index) {
		HideTargets();

		if (UIController.WhichButtonIsSelected() == index) {
			selectedAbility = currentCombatant.Abilities[index];
			ShowTargets();
		} else {
			selectedAbility = null;
		}
	}

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
