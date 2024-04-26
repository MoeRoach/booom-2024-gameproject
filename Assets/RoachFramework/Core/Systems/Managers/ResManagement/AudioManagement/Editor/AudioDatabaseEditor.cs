// File create date:2021/2/15

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 音频数据库对象
/// </summary>
[CustomEditor(typeof(AudioDatabaseObject), true)]
public class AudioDatabaseEditor : CustomScriptEditor {

    private AudioDatabaseObject databaseObject;
    private SerializedProperty audioMixerProperty;
    private SerializedProperty databaseNameProperty;
    private SerializedProperty groupNamesProperty;
    private SerializedProperty audioGroupsProperty;
    private SerializedProperty expandProperty;

    private Vector2 audioGroupListPos;

    protected override void OnInit() {
        databaseObject = target as AudioDatabaseObject;
        audioMixerProperty = GetPropertyFromTarget("databaseMixer");
        databaseNameProperty = GetPropertyFromTarget("databaseName");
        groupNamesProperty = GetPropertyFromTarget("groupNames");
        audioGroupsProperty = GetPropertyFromTarget("audioGroups");
        expandProperty = GetPropertyFromTarget("isExpanded");
    }

    protected override void OnCustomInspectorGUI() {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(databaseNameProperty);
        EditorGUILayout.PropertyField(audioMixerProperty);
        EditorGUILayout.LabelField("Database Group Datas");
        audioGroupListPos = EditorGUILayout.BeginScrollView(audioGroupListPos, GUILayout.Height(240f));
        DrawAudioGroupList();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawAudioGroupList() {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (var i = 0; i < databaseObject.groupNames.Count; i++) {
            DrawSingleAudioGroup(databaseObject.groupNames[i], databaseObject.audioGroups[i]);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSingleAudioGroup(string grpName, AudioGroupData groupData) {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(grpName);
        EditorGUILayout.LabelField($"AudioClips[{groupData.groupData.Count}]");
        EditorGUILayout.EndVertical();
    }
}
