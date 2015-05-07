using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class EnemyCombatant : BattleCombatant {
	
	public override void Start () {
		base.Start();
	}

	public override void Update () {
		base.Start();
	}

	public virtual IEnumerator BattleAI(BattleUIController ui, List<BattleCombatant> targetList) {
		List<BattleCombatant> aliveTargets = targetList.Where(t => t.Health.Current > 0).ToList();
		BattleCombatant target = aliveTargets[Random.Range(0, aliveTargets.Count)];
		Ability ability = Abilities[0];

		ui.ShowAbilityName(ability.Name);
		yield return new WaitForSeconds(1.5f);
		yield return StartCoroutine(UseAbility(ui, ability, target));
	}

	protected virtual IEnumerator UseAbility(BattleUIController ui, Ability ability, BattleCombatant target) {
		PlayAttackAnim();
		
		// wait for ability animation
		do {
			yield return null;
		} while (isAnimating());
		
		int damage = Strength + ability.Power;
		target.Damage(damage);
		ui.UpdateHealthBar(target.Health);
		
		// wait for ability animation
		do {
			yield return null;
		} while (target.isAnimating());
	}
}
