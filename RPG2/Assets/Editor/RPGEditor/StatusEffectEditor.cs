using UnityEngine;
using UnityEditor;
using System.Collections;

public class StatusEffectEditor : EditorWindow {
	private string statusName = "";
	//public Image Icon;
	private int duration = 1;
	private int damagePerTurn = 0;
	private bool skipsTurn = false;


	[MenuItem("RPGEditor/Status Effect Editor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(StatusEffectEditor));
	}

	void OnGUI() {
		GUILayout.Label("Status Effect Info", EditorStyles.boldLabel);
		statusName = EditorGUILayout.TextField("Name", statusName);
		duration = EditorGUILayout.IntField("Duration", duration);
		damagePerTurn = EditorGUILayout.IntField("Damage Per Turn", damagePerTurn);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Skips Turn?", EditorStyles.label); 
		skipsTurn = EditorGUILayout.Toggle(skipsTurn);
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		if (GUILayout.Button("Create Status Effect")) {
			CreateStatus();
		}
	}

	private void CreateStatus() {
		StatusEffect status = ScriptableObject.CreateInstance<StatusEffect>();
		status.Name = statusName;
		status.Duration = duration;
		status.DamagePerTurn = damagePerTurn;
		status.SkipsTurn = skipsTurn;

		AssetDatabase.CreateAsset(status, "Assets/_Common/StatusEffects/" + statusName + ".asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = status;
		
		Debug.Log("Ability '" + statusName + "' created!");
	}
}
