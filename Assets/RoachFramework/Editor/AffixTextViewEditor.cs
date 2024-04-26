using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 带前后缀文字控件编辑器插件
    /// </summary>
    [CustomEditor(typeof(AffixTextView), true)]
    public class AffixTextViewEditor : CustomScriptEditor {
        
        private SerializedProperty hasPrefixProperty;
        private SerializedProperty prefixTextProperty;
        private SerializedProperty hasSuffixProperty;
        private SerializedProperty suffixTextProperty;

        protected override void OnInit() {
            hasPrefixProperty = GetPropertyFromTarget("hasPrefix");
            prefixTextProperty = GetPropertyFromTarget("prefixText");
            hasSuffixProperty = GetPropertyFromTarget("hasSuffix");
            suffixTextProperty = GetPropertyFromTarget("suffixText");
        }

        protected override void OnCustomInspectorGUI() {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("--- 组件配置 ---");
            DrawPrefixInfo();
            DrawSuffixInfo();
            EditorGUILayout.EndVertical();
        }

        private void DrawPrefixInfo() {
            EditorGUILayout.BeginHorizontal();
            hasPrefixProperty.boolValue = EditorGUILayout.Toggle(hasPrefixProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("包含前缀");
            EditorGUILayout.EndHorizontal();
            if (!hasPrefixProperty.boolValue) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("前缀内容：", GUILayout.Width(70f));
            prefixTextProperty.stringValue = EditorGUILayout.TextField(prefixTextProperty.stringValue);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSuffixInfo() {
            EditorGUILayout.BeginHorizontal();
            hasSuffixProperty.boolValue = EditorGUILayout.Toggle(hasSuffixProperty.boolValue, GUILayout.Width(14f));
            EditorGUILayout.LabelField("包含后缀");
            EditorGUILayout.EndHorizontal();
            if (!hasSuffixProperty.boolValue) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("后缀内容：", GUILayout.Width(70f));
            suffixTextProperty.stringValue = EditorGUILayout.TextField(suffixTextProperty.stringValue);
            EditorGUILayout.EndHorizontal();
        }
    }
}
