// File create date:2021/6/26
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created By Yu.Liu
public class TilesPaletteWindow : EditorWindow  {
	
	[MenuItem("Window/Data Manager/Tiles Data Setup")]
	private static void NewWindow() {
		var windowRect = new Rect(400, 300, 800, 570);
		var window = (TilesPaletteWindow) GetWindowWithRect(
			typeof(TilesPaletteWindow), windowRect, true, "瓦片资源映射表");
		window.Show();
	}
	
	private TilesDataRootObject rootObject;

	private SerializedObject windowObject;
	private Vector2 tilesPaletteListPos;
	private Vector2 tilesGroupListPos;

	private TilesPaletteObject editPaletteObject;

	private TilesGroupData editTilesGroup;

	private string createTilesPaletteName = string.Empty;
	private string createTilesGroupName = string.Empty;

	private Color expandItemColor;
	private Color shrinkItemColor;
	
	private void OnEnable() {
		windowObject = new SerializedObject(this);
		LoadPrefabDataFromResource();
		expandItemColor = Color.green;
		expandItemColor.a = 0.25f;
		shrinkItemColor = Color.magenta;
		shrinkItemColor.a = 0.25f;
	}

	private void LoadPrefabDataFromResource() {
		if (!FileUtils.CheckDirectory(TilesConfigs.PATH_ASSET_RESOURCE_FOLDER)
		) {
			FileUtils.CreateDirectory(TilesConfigs.PATH_ASSET_RESOURCE_FOLDER);
		}

		var prefabDataFolder =
			$"{TilesConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TilesConfigs.FOLDER_TILES_DATA}/";
		if (!FileUtils.CheckDirectory(prefabDataFolder)) {
			FileUtils.CreateDirectory(prefabDataFolder);
		}

		var prefabDatabaseFolder =
			$"{TilesConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TilesConfigs.FOLDER_TILES_DATA}/{TilesConfigs.FOLDER_TILES_PALETTE}/";
		if (!FileUtils.CheckDirectory(prefabDatabaseFolder)) {
			FileUtils.CreateDirectory(prefabDatabaseFolder);
		}

		var prefabDataRoot =
			$"{TilesConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TilesConfigs.URI_TILES_DATA_ROOT}.asset";
		rootObject = !FileUtils.CheckFile(prefabDataRoot)
			? AssetUtils.CreateAsset<TilesDataRootObject>(prefabDataFolder,
				TilesConfigs.FILE_TILES_DATA_ROOT)
			: ResourceUtils.LoadResource<TilesDataRootObject>(TilesConfigs
				.URI_TILES_DATA_ROOT);
	}

	private void OnGUI() {
		windowObject.Update();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("保存瓦片数据")) {
			SaveDataToFile();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("瓦片调色板列表");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
		tilesPaletteListPos =
			EditorGUILayout.BeginScrollView(tilesPaletteListPos,
				GUILayout.Height(240f));
		DrawTilesPaletteList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("创建调色板", GUILayout.Width(72f));
		createTilesPaletteName =
			EditorGUILayout.TextField(createTilesPaletteName);
		if (GUILayout.Button("创建", GUILayout.Width(72f))) {
			if (!string.IsNullOrEmpty(createTilesPaletteName)) {
				if (!rootObject.CheckPalette(createTilesPaletteName)) {
					var tilesPaletteFolder =
						$"{TilesConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TilesConfigs.FOLDER_TILES_DATA}/{TilesConfigs.FOLDER_TILES_PALETTE}/";
					var plName =
						$"{TilesConfigs.PREFIX_TILES_PALETTE}{createTilesPaletteName.Trim()}";
					var databaseObject =
						AssetUtils.CreateAsset<TilesPaletteObject>(
							tilesPaletteFolder, plName);
					databaseObject.paletteName = createTilesPaletteName;
					rootObject.AddTilesPalette(createTilesPaletteName,
						databaseObject);
					createTilesPaletteName = string.Empty;
					Repaint();
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		if (string.IsNullOrEmpty(createTilesPaletteName)) {
			EditorGUILayout.HelpBox("请输入调色板名称",
				MessageType.Warning);
		} else {
			if (rootObject.CheckPalette(createTilesPaletteName)) {
				EditorGUILayout.HelpBox("调色板已经存在",
					MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox("调色板可以使用",
					MessageType.Info);
			}
		}

		EditorGUILayout.LabelField("编辑瓦片组");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(160f));
		tilesGroupListPos =
			EditorGUILayout.BeginScrollView(tilesGroupListPos,
				GUILayout.Height(160f));
		DrawTilesDataList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if (editPaletteObject != null && editTilesGroup != null) {
			DrawTilesDropArea();
		}

		EditorGUILayout.EndVertical();
		windowObject.ApplyModifiedProperties();
	}
	
	private void DrawTilesPaletteList() {
		EditorGUILayout.BeginVertical();
		for (var i = 0; i < rootObject.paletteNames.Count; i++) {
			DrawSingleTilesPalette(i, rootObject.paletteNames[i]);
		}

		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTilesPalette(int index, string plName) {
		if (rootObject != null) {
			var paletteObject = rootObject.tilesPalettes[index];
			var bgColor = paletteObject.isExpanded
				? expandItemColor
				: shrinkItemColor;
			GUI.backgroundColor = bgColor;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			GUI.backgroundColor = Color.white;
			EditorGUILayout.BeginHorizontal();
			paletteObject.isExpanded =
				EditorGUILayout.Toggle(paletteObject.isExpanded,
					GUILayout.Width(16f));
			EditorGUILayout.LabelField(plName, GUILayout.Width(150f));
			EditorGUILayout.LabelField(string.Empty);
			if (GUILayout.Button("删除", GUILayout.Width(100f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定调色板？", "确认",
					"取消")) {
					rootObject.RemovePalette(plName);
					AssetDatabase.MoveAssetToTrash(
						AssetDatabase.GetAssetPath(paletteObject));
					Repaint();
				}
			}

			EditorGUILayout.EndHorizontal();
			if (paletteObject.isExpanded) {
				DrawTilesGroupList(paletteObject);
			}

			EditorGUILayout.EndVertical();
		}
	}

	private void DrawTilesGroupList(TilesPaletteObject palette) {
		GUI.backgroundColor = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.backgroundColor = Color.white;
		EditorGUI.indentLevel++;
		for (var i = 0; i < palette.groupNames.Count; i++) {
			var group = palette.tilesGroups[i];
			DrawSingleTilesGroup(palette, group);
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("创建瓦片组", GUILayout.Width(88f));
		createTilesGroupName = EditorGUILayout.TextField(createTilesGroupName,
			GUILayout.Width(280f));
		if (GUILayout.Button("创建", GUILayout.Width(100f))) {
			if (!string.IsNullOrEmpty(createTilesGroupName)) {
				if (!palette.CheckGroupData(createTilesGroupName)) {
					var grp = new TilesGroupData(createTilesGroupName);
					palette.AddTilesGroup(createTilesGroupName, grp);
					createTilesGroupName = string.Empty;
					Repaint();
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTilesGroup(TilesPaletteObject palette,
		TilesGroupData groupData) {
		if (groupData != null) {
			var indexTag = groupData.DataCount > 0 ? "+" : "-";
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(indexTag, GUILayout.Width(24f));
			EditorGUILayout.LabelField(groupData.groupName);
			if (GUILayout.Button("编辑瓦片组", GUILayout.Width(120f))) {
				SaveDataToFile();
				editPaletteObject = palette;
				editTilesGroup = groupData;
				Repaint();
			}

			if (GUILayout.Button("删除瓦片组", GUILayout.Width(120f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定瓦片组？", "确认",
					"取消")) {
					palette.RemoveGroupData(groupData.groupName);
					Repaint();
				}
			}

			EditorGUILayout.EndHorizontal();
		}
	}

	private void DrawTilesDataList() {
		EditorGUILayout.BeginVertical();
		if (editPaletteObject != null && editTilesGroup != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(
				$"当前调色板:[{editPaletteObject.paletteName}][{editTilesGroup.groupName}]",
				GUILayout.Width(200f));
			EditorGUILayout.EndHorizontal();
			if (editTilesGroup != null) {
				EditorGUI.indentLevel++;
				for (var i = 0; i < editTilesGroup.DataCount; i++) {
					var data = editTilesGroup.groupData[i];
					DrawSingleTilesData(i, data);
				}

				EditorGUI.indentLevel--;
			}
		}

		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTilesData(int index, TilesData data) {
		GUI.color = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.color = Color.white;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField($"瓦片名称: {data.tileName}");
		EditorGUILayout.LabelField("数据标签:", GUILayout.Width(72f));
		data.dataName =
			EditorGUILayout.TextField(data.dataName, GUILayout.Width(160f));
		if (GUILayout.Button("删除", GUILayout.Width(120f))) {
			if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定瓦片？", "确认", "取消")
			) {
				editTilesGroup.groupData.RemoveAt(index);
				Repaint();
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	private void SaveDataToFile() {
		if (rootObject != null) {
			EditorUtility.SetDirty(rootObject);
		}

		if (editPaletteObject != null) {
			EditorUtility.SetDirty(editPaletteObject);
		}

		AssetDatabase.SaveAssets();
	}

	private void DrawTilesDropArea() {
		GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true),
			GUILayout.Height(0));
		var lastRect = GUILayoutUtility.GetLastRect();
		EditorGUILayout.BeginHorizontal();
		var dropRect =
			new Rect(lastRect.width - 200f, lastRect.y - 2f, 200f, 20f);
		var containsMouse = dropRect.Contains(Event.current.mousePosition);
		if (containsMouse) {
			switch (Event.current.type) {
				case EventType.DragUpdated:
					var containsPrefabObject = DragAndDrop.objectReferences
						.OfType<TileBase>().Any();
					DragAndDrop.visualMode = containsPrefabObject
						? DragAndDropVisualMode.Copy
						: DragAndDropVisualMode.Rejected;
					Event.current.Use();
					Repaint();
					break;
				case EventType.DragPerform:
					if (editPaletteObject != null && editTilesGroup != null) {
						var count = DragAndDrop.objectReferences.Length;
						for (var i = 0; i < count; i++) {
							var obj = DragAndDrop.objectReferences[i];
							if (!(obj is TileBase)) continue;
							var tile = obj as TileBase;
							var dataName =
								$"编号({editTilesGroup.DataCount})";
							editTilesGroup.PutTileData(dataName, tile.name,
								tile);
						}
					}

					Event.current.Use();
					Repaint();
					break;
			}
		}

		var color = GUI.color;
		GUI.color = new Color(color.r, color.g, color.b,
			containsMouse ? 0.8f : 0.4f);
		GUI.Box(dropRect, "+将瓦片资源拖放至此");
		GUI.color = color;
		EditorGUILayout.EndHorizontal();
	}
}