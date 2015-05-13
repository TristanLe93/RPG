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

	public void PlayVictoryAnim() {
		anim.SetBool("isVictory", true);
	}
}
