// File create date:2021/12/11
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(MaterialDatabaseObject), true)]
public class MaterialDatabaseEditor : CustomScriptEditor  {
	
	private MaterialDatabaseObject databaseObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty groupNamesProperty;
	private SerializedProperty materialGroupsProperty;
	private SerializedProperty expandProperty;
	
	private Vector2 materialGroupListPos;
	
	protected override void OnInit() {
		databaseObject = target as MaterialDatabaseObject;
		databaseNameProperty = GetPropertyFromTarget("databaseName");
		groupNamesProperty = GetPropertyFromTarget("groupNames");
		materialGroupsProperty = GetPropertyFromTarget("materialGroups");
		expandProperty = GetPropertyFromTarget("isExpanded");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PropertyField(databaseNameProperty);
		EditorGUILayout.LabelField("Database Group Datas");
		materialGroupListPos =
			EditorGUILayout.BeginScrollView(materialGroupListPos,
				GUILayout.Height(240f));
		DrawMaterialGroupList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	private void DrawMaterialGroupList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < databaseObject.groupNames.Count; i++) {
			DrawSingleMaterialGroup(databaseObject.groupNames[i],
				databaseObject.materialGroups[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleMaterialGroup(string grpName, MaterialGroupData
		groupData) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(grpName);
		EditorGUILayout.LabelField(
			$"MaterialObject[{groupData.groupData.Count}]");
		EditorGUILayout.EndVertical();
	}
}