using UnityEngine;
using UnityEditor;

public class AbilityEditor : EditorWindow {
	private string abilityName = string.Empty;
	private AbilityType type = AbilityType.Attack;
	private int power = 0;

	[MenuItem("RPGEditor/Ability Editor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(AbilityEditor));
	}

	void OnGUI() {
		GUILayout.Label ("Ability Settings", EditorStyles.boldLabel);
		abilityName = EditorGUILayout.TextField("Name", abilityName);
		type = (AbilityType)EditorGUILayout.EnumPopup("Type", type);
		power = EditorGUILayout.IntField("Power", power);

		if (GUILayout.Button("Create Ability")) {
			CreateAbility();
			Debug.Log("Ability '" + abilityName + "' created!");
		}
	}

	private void CreateAbility() {
		Ability ability = ScriptableObject.CreateInstance<Ability>();
		ability.Name = abilityName;
		ability.Type = type;
		ability.Power = power;

		AssetDatabase.CreateAsset(ability, "Assets/_Common/Abilities/" + abilityName + ".asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = ability;
	}
}
