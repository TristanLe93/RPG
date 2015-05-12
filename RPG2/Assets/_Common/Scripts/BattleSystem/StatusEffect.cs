using UnityEngine;
using UnityEngine.UI;

public class StatusEffect : ScriptableObject {
	public string Name;
	public Image Icon;
	public int Duration;
	public int DamagePerTurn;
	public bool SkipsTurn;
}
