// File create date:2021/5/13

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(PrefabDatabaseObject), true)]
public class PrefabDatabaseEditor : CustomScriptEditor {

	private PrefabDatabaseObject databaseObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty groupNamesProperty;
	private SerializedProperty prefabGroupsProperty;
	private SerializedProperty expandProperty;

	private Vector2 prefabGroupListPos;

	protected override void OnInit() {
		databaseObject = target as PrefabDatabaseObject;
		databaseNameProperty = GetPropertyFromTarget("databaseName");
		groupNamesProperty = GetPropertyFromTarget("groupNames");
		prefabGroupsProperty = GetPropertyFromTarget("prefabGroups");
		expandProperty = GetPropertyFromTarget("isExpanded");
	}

	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PropertyField(databaseNameProperty);
		EditorGUILayout.LabelField("Database Group Datas");
		prefabGroupListPos =
			EditorGUILayout.BeginScrollView(prefabGroupListPos,
				GUILayout.Height(240f));
		DrawPrefabGroupList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawPrefabGroupList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < databaseObject.groupNames.Count; i++) {
			DrawSinglePrefabGroup(databaseObject.groupNames[i],
				databaseObject.prefabGroups[i]);
		}

		EditorGUILayout.EndVertical();
	}

	private void DrawSinglePrefabGroup(string grpName,
		PrefabGroupData groupData) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(grpName);
		EditorGUILayout.LabelField(
			$"PrefabObject[{groupData.groupData.Count}]");
		EditorGUILayout.EndVertical();
	}
}