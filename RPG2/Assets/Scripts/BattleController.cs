using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour {
	// combat parties
	public List<BattleCombatant> Players;
	public List<BattleCombatant> Enemies;

	public BattleUIController UIController;
	private PlayerCombatant lastPlayerCombatant;

	// target select
	private BattleCombatant selectedTarget;
	private Ability selectedAbility;

	// turn list
	private List<int> turnList;
	private int currentTurn;


	void Start () {
		StartBattle();
	}

	void Update () {
	}

	public void StartBattle() {
		int totalCombatants = Players.Count + Enemies.Count;
		turnList = Enumerable.Range(0, totalCombatants).ToList();

		//disableUI
		SetupNextTurn();
	}

	private void SetupNextTurn() {
		currentTurn = turnList[0];

		// update CombatUI with current combatant info
		if (currentTurn < Players.Count) {
			lastPlayerCombatant = (PlayerCombatant)GetCurrentCombatant();

			UIController.UpdateUI(lastPlayerCombatant.Name, lastPlayerCombatant.Abilities);
			UIController.UpdateHealthBar(lastPlayerCombatant.Health);
			UIController.EnableUI();
		} else {
			BeginEnemyTurn();
		}
	}

	private void BeginEnemyTurn() {
		EnemyCombatant enemy = (EnemyCombatant)GetCurrentCombatant();
		enemy.BattleAI(this, Players);
		enemy.PlayAttackAnim();
		StartCoroutine(WaitForAnimation(enemy));
	}

	/// <summary>
	/// End the current turn. 
	/// Check if the either party is dead and award the winner.
	/// </summary>
	private void EndTurn() {
		if (IsPartyDead(Players)) {
			EnemyVictory();
		}
		else if (IsPartyDead(Enemies)) { 
			PlayerVictory();
		} 
		else {
			turnList.Add(currentTurn);
			turnList.RemoveAt(0);

			SetupNextTurn(); 
		}
	}


	private IEnumerator WaitForAnimation(BattleCombatant combatant) {
		UIController.DisableUI();

		// wait for USER animations
		do {
			yield return null;
		} while (combatant.isAnimating());

		combatant.UseAbility(selectedAbility, selectedTarget);
		UIController.UpdateHealthBar(lastPlayerCombatant.Health);
		
		// wait for TARGET animations
		do {
			yield return null;
		} while (selectedTarget.isAnimating());

		// animation complete!
		EndTurn();
	}

	private BattleCombatant GetCurrentCombatant() {
		if (currentTurn < Players.Count)
			return Players[currentTurn];
		
		return Enemies[currentTurn - Players.Count];
	}

	private bool IsPartyDead(List<BattleCombatant> party) {
		return party.Find((BattleCombatant c) => c.isDead == false) == null;
	}

	private void PlayerVictory() {
		foreach (BattleCombatant b in Players) {
			b.PlayVictoryAnim();
		}
	}
	
	private void EnemyVictory() {
		Debug.Log("GAMEOVER - YOU LOSE");
		//TODO: YOU LOSE
	}

	public void SetAbility(Ability a) {
		selectedAbility = a;
	}

	public void SetTarget(BattleCombatant target) {
		selectedTarget = target;
	}

	public void PlayerAttack() {
		PlayerCombatant currentCombatant = (PlayerCombatant)GetCurrentCombatant();
		selectedAbility = currentCombatant.Abilities[0];
		selectedTarget = Enemies[0];

		currentCombatant.PlayAttackAnim();
		StartCoroutine(WaitForAnimation(currentCombatant));
	}

	public void PlayerHeal() {
		PlayerCombatant currentCombatant = (PlayerCombatant)GetCurrentCombatant();
		selectedAbility = currentCombatant.Abilities[1];
		selectedTarget = Players[0];
		
		currentCombatant.PlayItemAnim();
		StartCoroutine(WaitForAnimation(currentCombatant));
	}

	public void PlayerMagic() {
		PlayerCombatant currentCombatant = (PlayerCombatant)GetCurrentCombatant();
		selectedAbility = currentCombatant.Abilities[2];
		selectedTarget = Enemies[0];

		currentCombatant.PlayMagicAnim();
		StartCoroutine(WaitForAnimation(currentCombatant));
	}
}
