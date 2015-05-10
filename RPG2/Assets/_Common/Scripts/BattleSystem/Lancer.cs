using UnityEngine;
using System.Collections;

public class Lancer : PlayerCombatant {
	public override void Start() {
		base.Start();

		Name = "Lancer" + Random.Range(0, 100);
		Health = new Stat(10);
		Strength = 10;
	}

	public override void Update() {
		base.Start();
	}
}
