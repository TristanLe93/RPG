using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyCombatant : BattleCombatant {
	
	public override void Start () {
		base.Start();
	}

	public override void Update () {
		base.Start();
	}

	public abstract void BattleAI(BattleController controller, List<BattleCombatant> targetList);
}
