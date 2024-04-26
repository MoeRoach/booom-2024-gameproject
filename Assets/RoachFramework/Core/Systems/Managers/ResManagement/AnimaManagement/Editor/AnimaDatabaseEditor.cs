// File create date:2023/9/10
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(AnimaDatabaseObject), true)]
public class AnimaDatabaseEditor : CustomScriptEditor {
	
	private AnimaDatabaseObject databaseObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty groupNamesProperty;
	private SerializedProperty animaGroupsProperty;
	private SerializedProperty expandProperty;
	
	private Vector2 spriteGroupListPos;
	
	protected override void OnInit() {
		databaseObject = target as AnimaDatabaseObject;
		databaseNameProperty = GetPropertyFromTarget("databaseName");
		groupNamesProperty = GetPropertyFromTarget("groupNames");
		animaGroupsProperty = GetPropertyFromTarget("animaGroups");
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
			DrawSingleSpriteGroup(databaseObject.groupNames[i], databaseObject.animaGroups[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleSpriteGroup(string grpName, AnimaGroupData groupData) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(grpName);
		EditorGUILayout.LabelField($"AnimationClip[{groupData.groupData.Count}]");
		EditorGUILayout.EndVertical();
	}
}