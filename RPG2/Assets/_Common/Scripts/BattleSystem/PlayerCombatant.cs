using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerCombatant : BattleCombatant {
	public override void Start() {
		base.Start();
	}
	
	public override void Update() {
		base.Start();
	}

	public virtual IEnumerator UseAbility(Ability ability, List<BattleCombatant> targets) {
		PlayAnimation(ability.Type);
		
		// wait for ability animation to complete
		do {
			yield return null;
		} while (isAnimating());

		foreach (BattleCombatant target in targets) {
			ExecuteAbility(ability, target);
		}

		// wait for target animation to complete
		do {
			yield return null;
		} while (targets[0].isAnimating());
	}

	protected void PlayAnimation(AbilityType type) {
		switch (type) {
		case AbilityType.Melee: PlayAttackAnim(); break;
		case AbilityType.Magic: PlayMagicAnim(); break;
		case AbilityType.Heal: PlayItemAnim(); break;
		case AbilityType.Ranged: PlayAttackAnim(); break;
		}
	}

	protected void ExecuteAbility(Ability ability, BattleCombatant target) {
		int dmg = 0;

		switch (ability.Type) {
			case AbilityType.Melee: 
			dmg = ability.Power + Strength;
			target.Damage(dmg);
			break;

			case AbilityType.Magic: 
			dmg = ability.Power;
			target.Damage(dmg);
			break;

			case AbilityType.Heal: 
			dmg = ability.Power;
			target.Heal(dmg);
			break;

			case AbilityType.Ranged: 
			dmg = ability.Power + Strength;
			target.Damage(dmg);
			break;
		}

		// apply status effect to target if any
		if (ability.ApplyStatus.Count > 0) {
			foreach (StatusEffect status in ability.ApplyStatus) {
				StatusEffect statusClone = Object.Instantiate(status) as StatusEffect;
				target.StatusEffects.Add(statusClone);
			}
		}
	}

	public void PlayVictoryAnim() {
		anim.SetBool("isVictory", true);
	}
}
