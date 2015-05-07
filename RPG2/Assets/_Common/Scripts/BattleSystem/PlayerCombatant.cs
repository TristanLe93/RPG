using UnityEngine;
using System.Collections;

public abstract class PlayerCombatant : BattleCombatant {
	public override void Start() {
		base.Start();
	}
	
	public override void Update() {
		base.Start();
	}

	public virtual IEnumerator UseAbility(Ability ability, BattleCombatant target) {
		PlayAnimation(ability.Type);
		
		// wait for ability animation
		do {
			yield return null;
		} while (isAnimating());
		
		ExecuteAbility(ability, target);
		
		// wait for ability animation
		do {
			yield return null;
		} while (target.isAnimating());
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
	}

	public void PlayVictoryAnim() {
		anim.SetBool("isVictory", true);
	}
}
