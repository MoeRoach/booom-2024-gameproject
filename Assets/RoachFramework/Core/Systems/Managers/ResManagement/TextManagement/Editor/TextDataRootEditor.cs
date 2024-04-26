// File create date:2021/8/23

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(TextDataRootObject), true)]
public class TextDataRootEditor : CustomScriptEditor  {
	
	private TextDataRootObject rootObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty textDatabaseProperty;
	private Vector2 textDatabaseListPos;
	
	protected override void OnInit() {
		rootObject = target as TextDataRootObject;
		databaseNameProperty = GetPropertyFromTarget("databaseNames");
		textDatabaseProperty = GetPropertyFromTarget("textDatabases");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Text Database List");
		textDatabaseListPos =
			EditorGUILayout.BeginScrollView(textDatabaseListPos,
				GUILayout.Height(240f));
		DrawDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawDatabaseList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleSpriteDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleSpriteDatabase(int index, string dbName) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(dbName);
		EditorGUILayout.PropertyField(
			textDatabaseProperty.GetArrayElementAtIndex(index));
		EditorGUILayout.EndVertical();
	}
}