// File create date:2021/5/13

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(PrefabDataRootObject), true)]
public class PrefabDataRootEditor : CustomScriptEditor  {
	
	private PrefabDataRootObject rootObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty prefabDatabaseProperty;
	private Vector2 prefabDatabaseListPos;
	
	protected override void OnInit() {
		rootObject = target as PrefabDataRootObject;
		databaseNameProperty = GetPropertyFromTarget("databaseNames");
		prefabDatabaseProperty = GetPropertyFromTarget("prefabDatabases");
	}

	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Prefab Database List");
		prefabDatabaseListPos =
			EditorGUILayout.BeginScrollView(prefabDatabaseListPos,
				GUILayout.Height(240f));
		DrawDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawDatabaseList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSinglePrefabDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSinglePrefabDatabase(int index, string dbName) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(dbName);
		EditorGUILayout.PropertyField(
			prefabDatabaseProperty.GetArrayElementAtIndex(index));
		EditorGUILayout.EndVertical();
	}
}