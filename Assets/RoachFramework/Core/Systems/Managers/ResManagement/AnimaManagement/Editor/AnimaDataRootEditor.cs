// File create date:2023/9/10
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(AnimaDataRootObject), true)]
public class AnimaDataRootEditor : CustomScriptEditor {
	
	private AnimaDataRootObject rootObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty animaDatabaseProperty;
	private Vector2 animaDatabaseListPos;
	
	protected override void OnInit() {
		rootObject = target as AnimaDataRootObject;
		databaseNameProperty = GetPropertyFromTarget("databaseNames");
		animaDatabaseProperty = GetPropertyFromTarget("animaDatabases");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Anima Database List");
		animaDatabaseListPos =
			EditorGUILayout.BeginScrollView(animaDatabaseListPos,
				GUILayout.Height(240f));
		DrawDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawDatabaseList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleAnimaDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleAnimaDatabase(int index, string dbName) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(dbName);
		EditorGUILayout.PropertyField(
			animaDatabaseProperty.GetArrayElementAtIndex(index));
		EditorGUILayout.EndVertical();
	}
}