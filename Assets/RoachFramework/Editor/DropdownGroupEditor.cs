using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 下拉框组编辑器插件
    /// </summary>
    [CustomEditor(typeof(DropdownGroup), true)]
    public class DropdownGroupEditor : CustomScriptEditor {
        
        private SerializedProperty isExtraDropdownsProperty;
        private SerializedProperty dropdownListProperty;

        protected override void OnInit() {
            isExtraDropdownsProperty = GetPropertyFromTarget("isExtraDropdowns");
            dropdownListProperty = GetPropertyFromTarget("dropdownList");
        }

        protected override void OnCustomInspectorGUI() {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("--- 下拉组配置 ---");
            DrawDropdownList();
            EditorGUILayout.EndVertical();
        }

        private void DrawDropdownList() {
            EditorGUILayout.BeginHorizontal();
            isExtraDropdownsProperty.boolValue = EditorGUILayout.Toggle(isExtraDropdownsProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("外部配置下拉框");
            EditorGUILayout.EndHorizontal();
            if (isExtraDropdownsProperty.boolValue) {
                EditorGUILayout.PropertyField(dropdownListProperty);
            }
        }
    }
}
