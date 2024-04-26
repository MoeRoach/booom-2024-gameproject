// File create date:2021/6/26

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class TilesPaletteEditor : CustomScriptEditor  {
	
	private TilesPaletteObject paletteObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty groupNamesProperty;
	private SerializedProperty spriteGroupsProperty;
	private SerializedProperty expandProperty;
	
	private Vector2 spriteGroupListPos;
	
	protected override void OnInit() {
		paletteObject = target as TilesPaletteObject;
		databaseNameProperty = GetPropertyFromTarget("databaseName");
		groupNamesProperty = GetPropertyFromTarget("groupNames");
		spriteGroupsProperty = GetPropertyFromTarget("spriteGroups");
		expandProperty = GetPropertyFromTarget("isExpanded");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PropertyField(databaseNameProperty);
		EditorGUILayout.LabelField("Palette Group Datas");
		spriteGroupListPos =
			EditorGUILayout.BeginScrollView(spriteGroupListPos,
				GUILayout.Height(240f));
		DrawTilesGroupList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	private void DrawTilesGroupList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < paletteObject.groupNames.Count; i++) {
			DrawSingleTilesGroup(paletteObject.groupNames[i],
				paletteObject.tilesGroups[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTilesGroup(string grpName, TilesGroupData
		groupData) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(grpName);
		EditorGUILayout.LabelField(
			$"TileObject[{groupData.groupData.Count}]");
		EditorGUILayout.EndVertical();
	}
}