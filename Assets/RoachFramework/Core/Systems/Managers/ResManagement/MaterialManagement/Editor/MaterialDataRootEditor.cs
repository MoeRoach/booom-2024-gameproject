// File create date:2021/12/11
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(MaterialDataRootObject), true)]
public class MaterialDataRootEditor : CustomScriptEditor  {
	
	private MaterialDataRootObject rootObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty materialDatabaseProperty;
	private Vector2 materialDatabaseListPos;
	
	protected override void OnInit() {
		rootObject = target as MaterialDataRootObject;
		databaseNameProperty = GetPropertyFromTarget("databaseNames");
		materialDatabaseProperty = GetPropertyFromTarget("materialDatabases");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Material Database List");
		materialDatabaseListPos =
			EditorGUILayout.BeginScrollView(materialDatabaseListPos,
				GUILayout.Height(240f));
		DrawDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawDatabaseList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleMaterialDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleMaterialDatabase(int index, string dbName) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(dbName);
		EditorGUILayout.PropertyField(
			materialDatabaseProperty.GetArrayElementAtIndex(index));
		EditorGUILayout.EndVertical();
	}
}