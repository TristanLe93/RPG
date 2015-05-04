using UnityEngine;
using System.Collections.Generic;

public class EnemyLancer : EnemyCombatant {
	
	public override void Start () {
		base.Start();

		Name = "Enemy";
		Health = new Stat(50);
		Strength = 10;
	}

	public override void Update () {
		base.Update();
	}

	public override void BattleAI(BattleController controller, List<BattleCombatant> targetList) {
		controller.SetAbility(Abilities[0]);
		controller.SetTarget(targetList[Random.Range(0, targetList.Count-1)]);
	}

	public override void UseAbility(Ability ability, BattleCombatant target) {
		int damage = Strength + ability.Power;
		target.Damage(damage);
	}
}
