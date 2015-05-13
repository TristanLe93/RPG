using UnityEngine;
using UnityEngine.UI;

public class StatusEffect : ScriptableObject {
	public string Name;
	public Sprite Icon;
	public int Duration;
	public int DamagePerTurn;
	public bool SkipsTurn;

	// stat modifiers (percentage +-)
	public int Strength;
	public int Defense;
	public int Magic;
	public int Spirit;
	public int Speed;
	public int Dodge;
	public int Accuracy;
}
