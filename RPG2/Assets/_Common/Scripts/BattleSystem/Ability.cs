using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ability class contains information of abilities in the game. 
/// </summary>
public class Ability : ScriptableObject {
	public string Name;
	public AbilityType Type;
	public AbilityTarget TargetType;
	public bool MultiTarget;
	public int Power;

	public List<bool> UseableRanks = new List<bool>(4);
	public List<bool> TargetableRanks = new List<bool>(4);
	public List<StatusEffect> ApplyStatus = new List<StatusEffect>();

	/// <summary>
	/// Determines whether this ability is useable in the user's party rank.
	/// </summary>
	public bool IsUsable(int userRankPos) {
		return UseableRanks[userRankPos];
	}

	/// <summary>
	/// Returns a string representation of the ability. 
	/// Overrides toString() method.
	/// </summary>
	public override string ToString() {
		string tooltip = string.Format("{0}\n{1}\nUse [", Type, TargetType);

		for (int i = 0; i < UseableRanks.Count; i++) {
			if (UseableRanks[i])
				tooltip += i.ToString();

			if (i < UseableRanks.Count - 1)
				tooltip += ",";
		}

		tooltip += "]\nTarget [";

		if (TargetType == AbilityTarget.Self) {
			tooltip += "self";
		}
		else {
			for (int i = 0; i < UseableRanks.Count; i++) {
				if (UseableRanks[i])
					tooltip += i.ToString();
				
				if (i < UseableRanks.Count - 1)
					tooltip += ",";
			}
		}

		tooltip += "]\nPower: " + Power;

		return tooltip;
	}
}
