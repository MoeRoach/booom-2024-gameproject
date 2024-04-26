// File create date:2021/2/15

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 音频数据根对象
/// </summary>
[CustomEditor(typeof(AudioDataRootObject), true)]
public class AudioDataRootEditor : CustomScriptEditor {

    private AudioDataRootObject rootObject;
    private SerializedProperty databaseNameProperty;
    private SerializedProperty audioDatabaseProperty;
    private Vector2 audioDatabaseListPos;

    protected override void OnInit() {
        rootObject = target as AudioDataRootObject;
        databaseNameProperty = GetPropertyFromTarget("databaseNames");
        audioDatabaseProperty = GetPropertyFromTarget("audioDatabases");
    }

    protected override void OnCustomInspectorGUI() {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Audio Database List");
        audioDatabaseListPos = EditorGUILayout.BeginScrollView(audioDatabaseListPos, GUILayout.Height(240f));
        DrawDatabaseList();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawDatabaseList() {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (var i = 0; i < rootObject.databaseNames.Count; i++) {
            DrawSingleAudioDatabase(i, rootObject.databaseNames[i]);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSingleAudioDatabase(int index, string dbName) {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(dbName);
        EditorGUILayout.PropertyField(audioDatabaseProperty.GetArrayElementAtIndex(index));
        EditorGUILayout.EndVertical();
    }
}
