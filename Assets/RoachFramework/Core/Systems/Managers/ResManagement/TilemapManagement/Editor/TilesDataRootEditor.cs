// File create date:2021/6/26

using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class TilesDataRootEditor : CustomScriptEditor  {
	
	private TilesDataRootObject rootObject;
	private SerializedProperty paletteNameProperty;
	private SerializedProperty tilesPaletteProperty;
	private Vector2 tilePaletteListPos;
	
	protected override void OnInit() {
		rootObject = target as TilesDataRootObject;
		paletteNameProperty = GetPropertyFromTarget("paletteNames");
		tilesPaletteProperty = GetPropertyFromTarget("tilesPalettes");
	}

	protected override void OnCustomInspectorGUI() {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Tiles Palette List");
		tilePaletteListPos =
			EditorGUILayout.BeginScrollView(tilePaletteListPos,
				GUILayout.Height(240f));
		DrawPaletteList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void DrawPaletteList() {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		for (var i = 0; i < rootObject.paletteNames.Count; i++) {
			DrawSingleTilesPalette(i, rootObject.paletteNames[i]);
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTilesPalette(int index, string plName) {
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField(plName);
		EditorGUILayout.PropertyField(
			tilesPaletteProperty.GetArrayElementAtIndex(index));
		EditorGUILayout.EndVertical();
	}
}