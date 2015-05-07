using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class AbilityEditor : EditorWindow {
	private string abilityName = string.Empty;
	private AbilityType type = AbilityType.Melee;
	private AbilityTarget targets = AbilityTarget.Enemies;
	private int power = 0;

	private List<bool> useList = Enumerable.Repeat(false, 4).ToList();
	private List<bool> targetList = Enumerable.Repeat(false, 4).ToList();


	[MenuItem("RPGEditor/Ability Editor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(AbilityEditor));
	}

	void OnGUI() {
		GUILayout.Label("Ability Info", EditorStyles.boldLabel);
		abilityName = EditorGUILayout.TextField("Name", abilityName);
		type = (AbilityType)EditorGUILayout.EnumPopup("Type", type);
		targets = (AbilityTarget)EditorGUILayout.EnumPopup("Target Party", targets);

		GUILayout.Space(5);

		GUILayout.Label("Stats", EditorStyles.boldLabel);
		power = EditorGUILayout.IntField("Power", power);

		GUILayout.Space(5);

		GUILayout.Label("Use and Target Ranks", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Useable Ranks (1-4)   )", EditorStyles.label);
		useList[0] = EditorGUILayout.Toggle(useList[0]);
		useList[1] = EditorGUILayout.Toggle(useList[1]);
		useList[2] = EditorGUILayout.Toggle(useList[2]);
		useList[3] = EditorGUILayout.Toggle(useList[3]);
		GUILayout.EndHorizontal();

		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Targetable Ranks (1-4)", EditorStyles.label);
		targetList[0] = EditorGUILayout.Toggle(targetList[0]);
		targetList[1] = EditorGUILayout.Toggle(targetList[1]);
		targetList[2] = EditorGUILayout.Toggle(targetList[2]);
		targetList[3] = EditorGUILayout.Toggle(targetList[3]);
		GUILayout.EndHorizontal();

		GUILayout.Space(20);

		if (GUILayout.Button("Create Ability")) {
			CreateAbility();
		}
	}

	private void CreateAbility() {
		Ability ability = ScriptableObject.CreateInstance<Ability>();
		ability.Name = abilityName;
		ability.Type = type;
		ability.TargetType = targets;
		ability.Power = power;
		ability.UseableRanks = useList;
		ability.TargetableRanks = targetList;

		AssetDatabase.CreateAsset(ability, "Assets/_Common/Abilities/" + abilityName + ".asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = ability;

		Debug.Log("Ability '" + abilityName + "' created!");
	}

	private List<bool> CreateList<T>(params bool[] values) {
		return new List<bool>(values);
	}
}
