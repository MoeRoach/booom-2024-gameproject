using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 基础面板组件自定义编辑器
    /// </summary>
    [CustomEditor(typeof(BaseUiPanel), true)]
    public class BaseUiPanelEditor : CustomScriptEditor {
        
        private SerializedProperty isStaticProperty;
        private SerializedProperty startShowProperty;
        private SerializedProperty hideRootObjectProperty;
        private SerializedProperty hideRaycastProperty;
		
        protected override void OnInit() {
            isStaticProperty = GetPropertyFromTarget("isStatic");
            startShowProperty = GetPropertyFromTarget("startShow");
            hideRootObjectProperty = GetPropertyFromTarget("hideRootObject");
            hideRaycastProperty = GetPropertyFromTarget("hideRaycast");
        }

        protected override void OnCustomInspectorGUI() {
            EditorGUILayout.BeginHorizontal();
            isStaticProperty.boolValue =
                EditorGUILayout.Toggle(isStaticProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("静态窗口", GUILayout.Width(56f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            startShowProperty.boolValue =
                EditorGUILayout.Toggle(startShowProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("默认显示", GUILayout.Width(54f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            hideRootObjectProperty.boolValue =
                EditorGUILayout.Toggle(hideRootObjectProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("物体激活状态跟随显示", GUILayout.Width(128f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            hideRaycastProperty.boolValue =
                EditorGUILayout.Toggle(hideRaycastProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("射线遮挡状态跟随显示", GUILayout.Width(128f));
            EditorGUILayout.EndHorizontal();
        }
    }
}
