using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// EnemyCombatants use AI to determine what they do. 
/// </summary>
public abstract class EnemyCombatant : BattleCombatant {
	protected BattleCombatant target;
	protected Ability ability;
	
	public override void Start () {
		base.Start();
	}

	public override void Update () {
		base.Start();
	}

	/// <summary>
	/// The AI code to determine what ability to use in battle.
	/// </summary>
	public virtual IEnumerator BattleAI(BattleUIController ui, List<BattleCombatant> targetList) {
		List<BattleCombatant> aliveTargets = targetList.Where(t => t.Stats.Health.Current > 0).ToList();
		target = aliveTargets[Random.Range(0, aliveTargets.Count)];
		ability = Abilities[0];

		yield return StartCoroutine(ShowAbility(ui));
		yield return StartCoroutine(UseAbility(ability, target));

		// update target information
		ui.UpdateUI(target.name, target.Abilities);
		ui.UpdateHealthBar(target.Stats.Health);
		ui.UpdateStatusEffectsIcons(target.StatusEffects);
	}

	/// <summary>
	/// Show the ability being used
	/// </summary>
	protected IEnumerator ShowAbility(BattleUIController ui) {
		ui.ShowAbilityName(ability.Name);
		yield return new WaitForSeconds(1.5f);
	}

	protected virtual IEnumerator UseAbility(Ability ability, BattleCombatant target) {
		PlayAnimation(ability.Type);

		do {
			yield return null;
		} while (isAnimating());
		
		ExecuteAbility(ability, target);
	}

	public IEnumerator WaitForTargetAnimation() {
		do {
			yield return null;
		} while (target.isAnimating());
	}
}
