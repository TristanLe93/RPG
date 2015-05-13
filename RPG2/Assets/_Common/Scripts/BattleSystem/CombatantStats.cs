using UnityEngine;
using System.Collections;

public class CombatantStats : MonoBehaviour {
	// statistics
	public Stat Health;
	public int MaxHealth;	

	public int Strength;
	public int Defense;
	public int Magic;
	public int Spirit;
	public int Speed;
	public int Dodge;
	public int Accuracy;

	// resistances status (%)
	public int BleedResist;
	public int StunResist;
	public int DebuffResist;

	void Start() {
		Health = new Stat(MaxHealth);
	}
}
