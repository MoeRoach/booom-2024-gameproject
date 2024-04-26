using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 勾选框组编辑器插件
    /// </summary>
    [CustomEditor(typeof(CheckboxGroup), true)]
    public class CheckboxGroupEditor : CustomScriptEditor {
        
        private SerializedProperty singleSelectProperty;
        private SerializedProperty canSwitchOffProperty;
        private SerializedProperty isExtraTogglesProperty;
        private SerializedProperty toggleListProperty;

        protected override void OnInit() {
            singleSelectProperty = GetPropertyFromTarget("singleSelect");
            canSwitchOffProperty = GetPropertyFromTarget("canSwitchOff");
            isExtraTogglesProperty = GetPropertyFromTarget("isExtraToggles");
            toggleListProperty = GetPropertyFromTarget("toggleList");
        }

        protected override void OnCustomInspectorGUI() {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("--- 选项组配置 ---");
            DrawBasicInfo();
            DrawToggleList();
            EditorGUILayout.EndVertical();
        }

        private void DrawBasicInfo() {
            EditorGUILayout.BeginHorizontal();
            singleSelectProperty.boolValue = EditorGUILayout.Toggle(singleSelectProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("单项选择", GUILayout.Width(56f));
            EditorGUILayout.Space();
            canSwitchOffProperty.boolValue = EditorGUILayout.Toggle(canSwitchOffProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("可取消选择", GUILayout.Width(70f));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToggleList() {
            EditorGUILayout.BeginHorizontal();
            isExtraTogglesProperty.boolValue = EditorGUILayout.Toggle(isExtraTogglesProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("外部配置选择框");
            EditorGUILayout.EndHorizontal();
            if (isExtraTogglesProperty.boolValue) {
                EditorGUILayout.PropertyField(toggleListProperty);
            }
        }
    }
}
