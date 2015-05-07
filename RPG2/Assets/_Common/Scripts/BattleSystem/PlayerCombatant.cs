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
		PlayAttackAnim();
		
		// wait for ability animation
		do {
			yield return null;
		} while (isAnimating());
		
		int damage = Strength + ability.Power;
		target.Damage(damage);
		
		// wait for ability animation
		do {
			yield return null;
		} while (target.isAnimating());
	}

	public void PlayVictoryAnim() {
		anim.SetBool("isVictory", true);
	}
}
