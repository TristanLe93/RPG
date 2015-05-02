using UnityEngine;
using System.Collections;

public class Lancer : PlayerCombatant {

	public override void Start() {
		base.Start();

		Health = new Stat(50);
		Strength = 10;
	}

	public override void Update() {
		base.Start();
	}
}
