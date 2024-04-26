// File create date:2021/8/23

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
[CustomEditor(typeof(TextDatabaseObject), true)]
public class TextDatabaseEditor : CustomScriptEditor  {
	
	private TextDatabaseObject databaseObject;
	private SerializedProperty databaseNameProperty;
	private SerializedProperty dataNamesProperty;
	private SerializedProperty textDatasProperty;
	private SerializedProperty expandProperty;
	
	private Vector2 textDataListPos;
	
	protected override void OnInit() {
		databaseObject = target as TextDatabaseObject;
		databaseNameProperty = GetPropertyFromTarget("databaseName");
		dataNamesProperty = GetPropertyFromTarget("dataNames");
		textDatasProperty = GetPropertyFromTarget("textDatas");
		expandProperty = GetPropertyFromTarget("isExpanded");
	}
	
	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PropertyField(databaseNameProperty);
		EditorGUILayout.LabelField("Text Datas");
		textDataListPos = EditorGUILayout.BeginScrollView(textDataListPos,
				GUILayout.Height(240f));
		DrawTextDataList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	private void DrawTextDataList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < databaseObject.dataNames.Count; i++) {
			DrawSingleTextData(databaseObject.dataNames[i],
				databaseObject.textDatas[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTextData(string dn, TextData data) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(dn);
		EditorGUILayout.LabelField($"TextAsset[{data.fileName}]");
		EditorGUILayout.EndVertical();
	}
}