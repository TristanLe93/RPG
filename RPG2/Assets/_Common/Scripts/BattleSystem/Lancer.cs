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
}
