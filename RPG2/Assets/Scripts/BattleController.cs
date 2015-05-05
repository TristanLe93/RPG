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

	private BattleState battleState = BattleState.Start;


	void Start () {
		StartBattle();
	}

	void Update () {
		if (Input.GetMouseButtonUp(0) && 
		    selectedAbility != null &&
		    battleState == BattleState.PlayerTurn) {
			SetTarget();
		}
	}

	public void SetAbility(Ability a) {
		selectedAbility = a;
	}
	
	public void SetTarget(BattleCombatant target) {
		selectedTarget = target;
	}

	private void SetTarget() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);

		if (hit.collider != null) {
			BattleCombatant target = 
				hit.collider.gameObject.GetComponent<BattleCombatant>();

			selectedTarget = target;
			battleState = BattleState.EnemyTurn;

			HideTargets();
			UIController.DisableButtons();
			PlayAbilityAnimation(lastPlayerCombatant);
			StartCoroutine(WaitForAnimation(lastPlayerCombatant));
		}
	}
	
	public void StartBattle() {
		// initialise the turn list
		int totalCombatants = Players.Count + Enemies.Count;
		turnList = Enumerable.Range(0, totalCombatants).ToList();

		SetupNextTurn();
	}

	
	private void SetupNextTurn() {
		currentTurn = turnList[0];
		selectedAbility = null;
		selectedTarget = null;
		UIController.ResetButtons();

		GetCurrentCombatant().ObjectUI.ShowTurnIcon();

		// update CombatUI with current combatant info
		if (currentTurn < Players.Count) {
			battleState = BattleState.PlayerTurn;
			lastPlayerCombatant = (PlayerCombatant)GetCurrentCombatant();

			UIController.UpdateUI(lastPlayerCombatant.Name, lastPlayerCombatant.Abilities);
			UIController.UpdateHealthBar(lastPlayerCombatant.Health);
			UIController.EnableButtons();
		} else {
			BeginEnemyTurn();
		}
	}

	private void BeginEnemyTurn() {
		EnemyCombatant enemy = (EnemyCombatant)GetCurrentCombatant();
		enemy.BattleAI(this, Players);
		UIController.ShowAbilityName(selectedAbility.Name);
		StartCoroutine(DoEnemyTurn(enemy));
	}

	private IEnumerator DoEnemyTurn(EnemyCombatant enemy) {
		yield return new WaitForSeconds(1f);
		PlayAbilityAnimation(enemy);
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
		combatant.ObjectUI.HideTurnIcon();

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

	private void ShowTargets() {
		foreach (BattleCombatant c in Enemies) {
			c.ObjectUI.ShowTargetArrow();
		}
		foreach (BattleCombatant c in Players) {
			c.ObjectUI.ShowTargetArrow();
		}
	}

	private void HideTargets() {
		foreach (BattleCombatant c in Enemies) {
			c.ObjectUI.HideTargetArrow();
		}
		foreach (BattleCombatant c in Players) {
			c.ObjectUI.HideTargetArrow();
		}
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

	private void PlayAbilityAnimation(BattleCombatant c) {
		if (selectedAbility.Type == AbilityType.Attack)
			c.PlayAttackAnim();
		else if (selectedAbility.Type == AbilityType.Magic)
			c.PlayMagicAnim();
		else if (selectedAbility.Type == AbilityType.Heal) 
			c.PlayItemAnim();
	}

	
	public void Btn_SetAbility(int index) {
		if (UIController.IsAButtonSelected()) {
			selectedAbility = lastPlayerCombatant.Abilities[index];
			ShowTargets();
		} else {
			selectedAbility = null;
			HideTargets();
		}
	}
}
