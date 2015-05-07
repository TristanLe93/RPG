using UnityEngine;
using System.Collections.Generic;

public class Ability : ScriptableObject {
	public string Name;
	public AbilityType Type;
	public AbilityTarget TargetType;
	public int Power;

	public List<bool> UseableRanks = new List<bool>(4);
	public List<bool> TargetableRanks = new List<bool>(4);

	public bool IsUsable(int userRankPos) {
		return UseableRanks[userRankPos];
	}

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
