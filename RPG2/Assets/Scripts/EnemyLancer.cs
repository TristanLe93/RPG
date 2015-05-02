using UnityEngine;
using System.Collections.Generic;

public class EnemyLancer : EnemyCombatant {
	
	public override void Start () {
		base.Start();
	}

	public override void Update () {
		base.Update();
	}

	public override void BattleAI(BattleController controller, List<BattleCombatant> targetList) {
		controller.SetAbility(Abilities[0]);
		controller.SetTarget(targetList[Random.Range(0, targetList.Count-1)]);
	}
}
