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
}
