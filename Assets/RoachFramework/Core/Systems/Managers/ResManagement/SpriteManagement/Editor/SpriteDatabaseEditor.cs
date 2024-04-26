// File create date:2021/6/24

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(SpriteDatabaseObject), true)]
public class SpriteDatabaseEditor : CustomScriptEditor  {
	
	private SpriteDatabaseObject databaseObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty groupNamesProperty;
	private SerializedProperty spriteGroupsProperty;
	private SerializedProperty expandProperty;
	
	private Vector2 spriteGroupListPos;
	
	protected override void OnInit() {
		databaseObject = target as SpriteDatabaseObject;
		databaseNameProperty = GetPropertyFromTarget("databaseName");
		groupNamesProperty = GetPropertyFromTarget("groupNames");
		spriteGroupsProperty = GetPropertyFromTarget("spriteGroups");
		expandProperty = GetPropertyFromTarget("isExpanded");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PropertyField(databaseNameProperty);
		EditorGUILayout.LabelField("Database Group Datas");
		spriteGroupListPos =
			EditorGUILayout.BeginScrollView(spriteGroupListPos,
				GUILayout.Height(240f));
		DrawSpriteGroupList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSpriteGroupList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < databaseObject.groupNames.Count; i++) {
			DrawSingleSpriteGroup(databaseObject.groupNames[i],
				databaseObject.spriteGroups[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleSpriteGroup(string grpName, SpriteGroupData
		groupData) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(grpName);
		EditorGUILayout.LabelField(
			$"SpriteObject[{groupData.groupData.Count}]");
		EditorGUILayout.EndVertical();
	}
}