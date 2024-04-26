// File create date:2021/6/24

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(SpriteDataRootObject), true)]
public class SpriteDataRootEditor : CustomScriptEditor  {
	
	private SpriteDataRootObject rootObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty spriteDatabaseProperty;
	private Vector2 spriteDatabaseListPos;
	
	protected override void OnInit() {
		rootObject = target as SpriteDataRootObject;
		databaseNameProperty = GetPropertyFromTarget("databaseNames");
		spriteDatabaseProperty = GetPropertyFromTarget("spriteDatabases");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Sprite Database List");
		spriteDatabaseListPos =
			EditorGUILayout.BeginScrollView(spriteDatabaseListPos,
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
			spriteDatabaseProperty.GetArrayElementAtIndex(index));
		EditorGUILayout.EndVertical();
	}
}