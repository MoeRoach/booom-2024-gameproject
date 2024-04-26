// File create date:2019/11/19
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class CodePreviewWindow : EditorWindow {

    private string _previewCode;
    private Vector2 _scrollerPos;

    public void SetupCode(string code) {
        _previewCode = code;
    }

    private void OnGUI() {
        using var scroller = new GUILayout.ScrollViewScope(_scrollerPos);
        _scrollerPos = scroller.scrollPosition;
        if (!string.IsNullOrEmpty(_previewCode)) {
            GUILayout.Label(_previewCode);
        } else {
            EditorGUILayout.HelpBox("没有可预览的代码", MessageType.Warning);
        }
    }
}
