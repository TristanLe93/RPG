using UnityEngine;
using System.Collections;

public abstract class PlayerCombatant : BattleCombatant {
	public override void Start() {
		base.Start();
	}
	
	public override void Update() {
		base.Start();
	}

	public virtual void UseAbility(Ability ability, BattleCombatant target) {
		int damage = Strength + ability.Power;
		target.Damage(damage);
	}
}
