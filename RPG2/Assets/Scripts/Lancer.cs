using UnityEngine;
using System.Collections;

public class Lancer : PlayerCombatant {
	public override void Start() {
		base.Start();

		Name = "Lancer";
		Health = new Stat(50);
		Strength = 10;
	}

	public override void Update() {
		base.Start();
	}

	public override void UseAbility(Ability ability, BattleCombatant target) {
		int damage = Strength + ability.Power;
		target.Damage(damage);
	}
}
