using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
	public BattleUIController UIController;
	public List<BattleCombatant> Players;
	public List<BattleCombatant> Enemies;
	
	private bool battleActive = true;

	private Ability selectedAbility;
	private BattleCombatant selectedTarget;

	private List<int> turnList;
	private int currentTurn;


	private void Start () {
		// automatically start a battle
		StartCoroutine(UpdateState());
	}

	private void InitialiseBattle() {
		int totalCombatants = Players.Count + Enemies.Count;
		turnList = Enumerable.Range(0, totalCombatants).ToList();
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

		// Update Battle UI
		player.ObjectUI.ShowTurnIcon();
		UIController.UpdateUI(player.Name, player.Abilities);
		UIController.UpdateHealthBar(player.Health);
		UIController.EnableButtons();

		yield return StartCoroutine(SelectAbilityAndTarget());
		yield return StartCoroutine(player.UseAbility(selectedAbility, selectedTarget));
		EndTurn();
	}

	private IEnumerator EnemyTurn() {
		EnemyCombatant enemy = (EnemyCombatant)GetCurrentCombatant();
		enemy.ObjectUI.ShowTurnIcon();

		yield return StartCoroutine(enemy.BattleAI(UIController, Players));
		EndTurn();
	}

	/// <summary>
	/// End the current turn. 
	/// Check if the either party is dead and award the winner.
	/// </summary>
	private void EndTurn() {
		BattleCombatant b = GetCurrentCombatant();
		b.ObjectUI.HideTurnIcon();

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

		HideTargets();
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
		return !target.IsDead;
	}

	private bool IsPartyDead(List<BattleCombatant> party) {
		return party.Find((BattleCombatant b) => b.IsDead == false) == null;
	}

	private BattleCombatant GetCurrentCombatant() {
		if (currentTurn < Players.Count)
			return Players[currentTurn];
		
		return Enemies[currentTurn - Players.Count];
	}

	private void ShowTargets() {
		foreach (BattleCombatant b in Enemies) {
			if (!b.IsDead) {
				b.ObjectUI.ShowTargetArrow();
			}
		}
		foreach (BattleCombatant b in Players) {
			if (!b.IsDead) {
				b.ObjectUI.ShowTargetArrow();
			}
		}
	}

	private void HideTargets() {
		foreach (BattleCombatant b in Enemies) {
			b.ObjectUI.HideTargetArrow();
		}
		foreach (BattleCombatant b in Players) {
			b.ObjectUI.HideTargetArrow();
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
		if (UIController.IsAButtonSelected()) {
			selectedAbility = GetCurrentCombatant().Abilities[index];
			ShowTargets();
		} else {
			selectedAbility = null;
			HideTargets();
		}
	}
}
