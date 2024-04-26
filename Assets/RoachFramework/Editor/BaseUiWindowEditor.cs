using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 窗口基类的编辑器插件
    /// </summary>
    [CustomEditor(typeof(BaseUiWindow), true)]
    public class BaseUiWindowEditor : BaseUiPanelEditor {
        
        private SerializedProperty isPopupProperty;
        private SerializedProperty isDragableProperty;
        private SerializedProperty dragableAreaProperty;

        protected override void OnInit() {
            base.OnInit();
            isPopupProperty = GetPropertyFromTarget("isPopup");
            isDragableProperty = GetPropertyFromTarget("isDragable");
            dragableAreaProperty = GetPropertyFromTarget("dragableTrigger");
        }

        protected override void OnCustomInspectorGUI() {
            base.OnCustomInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            isPopupProperty.boolValue =
                EditorGUILayout.Toggle(isPopupProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("窗口动画", GUILayout.Width(56f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            isDragableProperty.boolValue = 
                EditorGUILayout.Toggle(isDragableProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("窗口拖拽");
            EditorGUILayout.EndHorizontal();
            if (isDragableProperty.boolValue) {
                EditorGUILayout.PropertyField(dragableAreaProperty);
            }
        }
    }
}
