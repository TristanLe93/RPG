using UnityEngine;
using UnityEditor;
using System.Collections;

public class StatusEffectEditor : EditorWindow {
	private string statusName = "";
	//public Image Icon;
	private int duration = 1;
	private int damagePerTurn = 0;
	private bool skipsTurn = false;

	// stat mods (percentage)
	private int strength;
	private int defense;
	private int magic;
	private int spirit;
	private int speed;
	private int dodge;
	private int accuracy;


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

		GUILayout.Label("Stat Modifiers (Percentage)", EditorStyles.boldLabel);
		GUILayout.Label("Positive numbers = stat increase");
		GUILayout.Label("Negative numbers = stat decrease");
		strength = EditorGUILayout.IntField("strength", strength);
		defense = EditorGUILayout.IntField("defense", defense);
		magic = EditorGUILayout.IntField("magic", magic);
		spirit = EditorGUILayout.IntField("spirit", spirit);
		speed = EditorGUILayout.IntField("speed", speed);
		dodge = EditorGUILayout.IntField("dodge", dodge);
		accuracy = EditorGUILayout.IntField("accuracy", accuracy);


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
		status.Strength = strength;
		status.Defense = defense;
		status.Magic = magic;
		status.Spirit = spirit;
		status.Speed = speed;
		status.Dodge = dodge;
		status.Accuracy = accuracy;

		AssetDatabase.CreateAsset(status, "Assets/_Common/StatusEffects/" + statusName + ".asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = status;
		
		Debug.Log("Ability '" + statusName + "' created!");
	}
}
