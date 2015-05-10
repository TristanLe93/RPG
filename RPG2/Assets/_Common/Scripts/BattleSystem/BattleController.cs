using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
	public BattleUIController UIController;
	public List<BattleCombatant> Players = new List<BattleCombatant>(4);
	public List<BattleCombatant> Enemies = new List<BattleCombatant>(4);
	
	private bool battleActive = true;

	private Ability selectedAbility;
	private BattleCombatant selectedTarget;
	private BattleCombatant lastSelectedPlayer;
	private List<BattleCombatant> targetList;

	private List<int> playerRanks;
	private List<int> enemyRanks;

	private List<int> turnList;
	private int currentTurn;
	private Ability swap;


	private void Start () {
		// initialise Swap ability
		swap = ScriptableObject.CreateInstance<Ability>();
		swap.TargetType = AbilityTarget.Players;
		swap.UseableRanks = Enumerable.Repeat(true, 4).ToList();

		targetList = new List<BattleCombatant>();

		// automatically start a battle
		StartCoroutine(UpdateState());
	}

	private void InitialiseBattle() {
		int totalCombatants = Players.Count + Enemies.Count;

		playerRanks = Enumerable.Range(0, Players.Count).ToList();
		enemyRanks = Enumerable.Range(Players.Count, Enemies.Count + Players.Count).ToList();
		turnList = Enumerable.Range(0, totalCombatants).ToList();

		// shuffle turnList
		for (int i = 0; i < turnList.Count; i++) {
			int temp = turnList[i];
			int randomIndex = Random.Range(i, turnList.Count);
			turnList[i] = turnList[randomIndex];
			turnList[randomIndex] = temp;
		}

		// setup HUD
		lastSelectedPlayer = Players[0];
		UIController.UpdateUI(lastSelectedPlayer.Name, lastSelectedPlayer.Abilities);
		UIController.UpdateHealthBar(lastSelectedPlayer.Health);
		UIController.DisableButtons();
	}

	private IEnumerator UpdateState() {
		InitialiseBattle();

		while (battleActive) {
			UIController.ResetButtons();
			currentTurn = turnList[0];
			selectedAbility = null;
			selectedTarget = null;

			if (GetCurrentCombatant().IsDead) {
				EndTurn();
			} else if (currentTurn < Players.Count) {
				yield return StartCoroutine(PlayerTurn());
			} else {
				yield return StartCoroutine(EnemyTurn());
			}
		}
	}

	private IEnumerator PlayerTurn() {
		PlayerCombatant player = (PlayerCombatant)GetCurrentCombatant();
		lastSelectedPlayer = player;
		int rank = playerRanks.IndexOf(currentTurn);

		// Update Battle UI
		player.ObjectUI.ShowTurnIcon();
		UIController.UpdateUI(player.Name, player.Abilities);
		UIController.UpdateHealthBar(player.Health);
		UIController.EnableButtons(player.Abilities, rank);

		// select an ability and use it on the target(s)
		yield return StartCoroutine(SelectAbilityAndTarget());

		if (selectedAbility == swap) {
			yield return StartCoroutine(SwapRanks());
		} 
		else {
			yield return StartCoroutine(player.UseAbility(selectedAbility, targetList));
		}

		EndTurn();
	}

	private IEnumerator EnemyTurn() {
		EnemyCombatant enemy = (EnemyCombatant)GetCurrentCombatant();
		enemy.ObjectUI.ShowTurnIcon();
		UIController.DisableButtons();

		yield return StartCoroutine(enemy.BattleAI(UIController, Players));
		UIController.UpdateHealthBar(lastSelectedPlayer.Health);

		EndTurn();
	}

	/// <summary>
	/// End the current turn. 
	/// Check if the either party is dead and award the winner.
	/// </summary>
	private void EndTurn() {
		BattleCombatant b = GetCurrentCombatant();
		b.ObjectUI.HideTurnIcon();
		targetList.Clear();

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
			turnList.Add(currentTurn);
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
		if (selectedAbility.TargetType == AbilityTarget.Enemies && Enemies.Contains(target)) {
			int enemyId = Enemies.IndexOf(target) + Players.Count;
			int rank = enemyRanks.IndexOf(enemyId);

			return selectedAbility.TargetableRanks[rank] && !target.IsDead;
		} 
		// targeting players
		else if (selectedAbility.TargetType == AbilityTarget.Players && Players.Contains(target)) {
			int playerId = Players.IndexOf(target);
			int rank = playerRanks.IndexOf(playerId);
			
			return selectedAbility.TargetableRanks[rank] && !target.IsDead;
		} 
		else if (selectedAbility.TargetType == AbilityTarget.Self)  {
			return lastSelectedPlayer == target;
		}

		return false;
	}

	private bool IsPartyDead(List<BattleCombatant> party) {
		return party.Find((BattleCombatant b) => b.IsDead == false) == null;
	}

	/// <summary>
	/// Swaps the positions of the current player 
	/// with another party member.
	/// Only usable with player's party.
	/// </summary>
	private IEnumerator SwapRanks() {
		int targetId = Players.IndexOf(selectedTarget);
		int targetRank = playerRanks.IndexOf(targetId);
		int currentRank = playerRanks.IndexOf(currentTurn);

		// swap Ranks of current player with target in PlayerRanks
		int tmp = playerRanks[targetRank];
		playerRanks[targetRank] = playerRanks[currentRank];
		playerRanks[currentRank] = tmp;

		// swap transform positions of current player with target
		Vector3 current = Players[currentTurn].transform.position;
		Vector3 target = Players[targetId].transform.position;
		Players[currentTurn].transform.position = target;
		Players[targetId].transform.position = current;

		yield return new WaitForSeconds(1.5f);
	}

	private BattleCombatant GetCurrentCombatant() {
		if (currentTurn < Players.Count)
			return Players[currentTurn];
		
		return Enemies[currentTurn - Players.Count];
	}

	private void ShowTargets() {
		if (selectedAbility.TargetType == AbilityTarget.Enemies) {
			foreach (BattleCombatant enemy in Enemies) {
				if (IsTargetValid(enemy)) {
					enemy.ObjectUI.HideTargetArrow();
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
			selectedAbility = GetCurrentCombatant().Abilities[index];
			targetList.Clear();
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
			int rankIndex = playerRanks.IndexOf(currentTurn);
			ShowTargets();
		} else {
			selectedAbility = null;
		}
	}

}
