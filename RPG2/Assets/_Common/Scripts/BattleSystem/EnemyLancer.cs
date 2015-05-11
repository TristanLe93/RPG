using UnityEngine;
using System.Collections.Generic;

public class EnemyLancer : EnemyCombatant {
	
	public override void Start () {
		base.Start();

		Name = "Enemy";
		Health = new Stat(10);
		Strength = 10;
	}

	public override void Update () {
		base.Update();
	}
}
