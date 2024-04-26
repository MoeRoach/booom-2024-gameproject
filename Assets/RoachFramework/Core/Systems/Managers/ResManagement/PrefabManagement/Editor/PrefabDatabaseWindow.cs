// File create date:2021/5/13
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class PrefabDatabaseWindow : EditorWindow {

	[MenuItem("Window/Data Manager/Prefab Data Setup")]
	private static void NewWindow() {
		var windowRect = new Rect(400, 300, 800, 570);
		var window = (PrefabDatabaseWindow) GetWindowWithRect(
			typeof(PrefabDatabaseWindow), windowRect, true, "预制体资源映射表");
		window.Show();
	}

	private PrefabDataRootObject rootObject;
	private SerializedObject windowObject;
	private Vector2 prefabDatabaseListPos;
	private Vector2 prefabGroupListPos;
	private PrefabDatabaseObject editDatabaseObject;
	private PrefabGroupData editPrefabGroup;
	private string createPrefabDatabaseName = string.Empty;
	private string createPrefabGroupName = string.Empty;

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
		if (!FileUtils.CheckDirectory(PrefabConfigs.PATH_ASSET_RESOURCE_FOLDER)
		) {
			FileUtils.CreateDirectory(PrefabConfigs.PATH_ASSET_RESOURCE_FOLDER);
		}

		var prefabDataFolder =
			$"{PrefabConfigs.PATH_ASSET_RESOURCE_FOLDER}/{PrefabConfigs.FOLDER_PREFAB_DATA}/";
		if (!FileUtils.CheckDirectory(prefabDataFolder)) {
			FileUtils.CreateDirectory(prefabDataFolder);
		}

		var prefabDatabaseFolder =
			$"{PrefabConfigs.PATH_ASSET_RESOURCE_FOLDER}/{PrefabConfigs.FOLDER_PREFAB_DATA}/{PrefabConfigs.FOLDER_PREFAB_DATABASE}/";
		if (!FileUtils.CheckDirectory(prefabDatabaseFolder)) {
			FileUtils.CreateDirectory(prefabDatabaseFolder);
		}

		var prefabDataRoot =
			$"{PrefabConfigs.PATH_ASSET_RESOURCE_FOLDER}/{PrefabConfigs.URI_PREFAB_DATA_ROOT}.asset";
		rootObject = !FileUtils.CheckFile(prefabDataRoot)
			? AssetUtils.CreateAsset<PrefabDataRootObject>(prefabDataFolder,
				PrefabConfigs.FILE_PREFAB_DATA_ROOT)
			: ResourceUtils.LoadResource<PrefabDataRootObject>(PrefabConfigs
				.URI_PREFAB_DATA_ROOT);
	}

	private void OnGUI() {
		windowObject.Update();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("保存预制体数据")) {
			SaveDataToFile();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("预制体数据库列表");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
		prefabDatabaseListPos =
			EditorGUILayout.BeginScrollView(prefabDatabaseListPos,
				GUILayout.Height(240f));
		DrawPrefabDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("创建数据库", GUILayout.Width(72f));
		createPrefabDatabaseName =
			EditorGUILayout.TextField(createPrefabDatabaseName);
		if (GUILayout.Button("创建", GUILayout.Width(72f))) {
			if (!string.IsNullOrEmpty(createPrefabDatabaseName)) {
				if (!rootObject.CheckDatabase(createPrefabDatabaseName)) {
					var prefabDatabaseFolder =
						$"{PrefabConfigs.PATH_ASSET_RESOURCE_FOLDER}/{PrefabConfigs.FOLDER_PREFAB_DATA}/{PrefabConfigs.FOLDER_PREFAB_DATABASE}/";
					var dbName =
						$"{PrefabConfigs.PREFIX_PREFAB_DATABASE}{createPrefabDatabaseName.Trim()}";
					var databaseObject =
						AssetUtils.CreateAsset<PrefabDatabaseObject>(
							prefabDatabaseFolder, dbName);
					databaseObject.databaseName = createPrefabDatabaseName;
					rootObject.AddPrefabDatabase(createPrefabDatabaseName,
						databaseObject);
					createPrefabDatabaseName = string.Empty;
					Repaint();
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		if (string.IsNullOrEmpty(createPrefabDatabaseName)) {
			EditorGUILayout.HelpBox("请输入数据库名称",
				MessageType.Warning);
		} else {
			if (rootObject.CheckDatabase(createPrefabDatabaseName)) {
				EditorGUILayout.HelpBox("数据库已经存在",
					MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox("数据库可以使用",
					MessageType.Info);
			}
		}

		EditorGUILayout.LabelField("编辑预制体组");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(160f));
		prefabGroupListPos =
			EditorGUILayout.BeginScrollView(prefabGroupListPos,
				GUILayout.Height(160f));
		DrawPrefabDataList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if (editDatabaseObject != null && editPrefabGroup != null) {
			DrawPrefabDropArea();
		}

		EditorGUILayout.EndVertical();
		windowObject.ApplyModifiedProperties();
	}

	private void DrawPrefabDatabaseList() {
		EditorGUILayout.BeginVertical();
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSinglePrefabDatabase(i, rootObject.databaseNames[i]);
		}

		EditorGUILayout.EndVertical();
	}

	private void DrawSinglePrefabDatabase(int index, string dbName) {
		if (rootObject != null) {
			var databaseObject = rootObject.prefabDatabases[index];
			var bgColor = databaseObject.isExpanded
				? expandItemColor
				: shrinkItemColor;
			GUI.backgroundColor = bgColor;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			GUI.backgroundColor = Color.white;
			EditorGUILayout.BeginHorizontal();
			databaseObject.isExpanded =
				EditorGUILayout.Toggle(databaseObject.isExpanded,
					GUILayout.Width(16f));
			EditorGUILayout.LabelField(dbName, GUILayout.Width(150f));
			EditorGUILayout.LabelField(string.Empty);
			if (GUILayout.Button("删除", GUILayout.Width(100f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定数据库？", "确认",
					"取消")) {
					rootObject.RemoveDatabase(dbName);
					AssetDatabase.MoveAssetToTrash(
						AssetDatabase.GetAssetPath(databaseObject));
					Repaint();
				}
			}

			EditorGUILayout.EndHorizontal();
			if (databaseObject.isExpanded) {
				DrawPrefabGroupList(databaseObject);
			}

			EditorGUILayout.EndVertical();
		}
	}

	private void DrawPrefabGroupList(PrefabDatabaseObject database) {
		GUI.backgroundColor = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.backgroundColor = Color.white;
		EditorGUI.indentLevel++;
		for (var i = 0; i < database.groupNames.Count; i++) {
			var group = database.prefabGroups[i];
			DrawSinglePrefabGroup(database, group);
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("创建预制体组", GUILayout.Width(88f));
		createPrefabGroupName = EditorGUILayout.TextField(createPrefabGroupName,
			GUILayout.Width(280f));
		if (GUILayout.Button("创建", GUILayout.Width(100f))) {
			if (!string.IsNullOrEmpty(createPrefabGroupName)) {
				if (!database.CheckGroupData(createPrefabGroupName)) {
					var grp = new PrefabGroupData(createPrefabGroupName);
					database.AddPrefabGroup(createPrefabGroupName, grp);
					createPrefabGroupName = string.Empty;
					Repaint();
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}

	private void DrawSinglePrefabGroup(PrefabDatabaseObject database,
		PrefabGroupData groupData) {
		if (groupData != null) {
			var indexTag = groupData.DataCount > 0 ? "+" : "-";
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(indexTag, GUILayout.Width(24f));
			EditorGUILayout.LabelField(groupData.groupName);
			if (GUILayout.Button("编辑预制体组", GUILayout.Width(120f))) {
				SaveDataToFile();
				editDatabaseObject = database;
				editPrefabGroup = groupData;
				Repaint();
			}

			if (GUILayout.Button("删除预制体组", GUILayout.Width(120f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定预制体组？", "确认",
					"取消")) {
					database.RemoveGroupData(groupData.groupName);
					Repaint();
				}
			}

			EditorGUILayout.EndHorizontal();
		}
	}

	private void DrawPrefabDataList() {
		EditorGUILayout.BeginVertical();
		if (editDatabaseObject != null && editPrefabGroup != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(
				$"当前数据库:[{editDatabaseObject.databaseName}][{editPrefabGroup.groupName}]",
				GUILayout.Width(200f));
			EditorGUILayout.EndHorizontal();
			if (editPrefabGroup != null) {
				EditorGUI.indentLevel++;
				for (var i = 0; i < editPrefabGroup.DataCount; i++) {
					var data = editPrefabGroup.groupData[i];
					DrawSinglePrefabData(i, data);
				}

				EditorGUI.indentLevel--;
			}
		}

		EditorGUILayout.EndVertical();
	}

	private void DrawSinglePrefabData(int index, PrefabData data) {
		GUI.color = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.color = Color.white;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField($"预制体名称: {data.prefabName}");
		EditorGUILayout.LabelField("数据标签:", GUILayout.Width(72f));
		data.dataName =
			EditorGUILayout.TextField(data.dataName, GUILayout.Width(160f));
		if (GUILayout.Button("删除", GUILayout.Width(120f))) {
			if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定预制体？", "确认", "取消")
			) {
				editPrefabGroup.groupData.RemoveAt(index);
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

		if (editDatabaseObject != null) {
			EditorUtility.SetDirty(editDatabaseObject);
		}

		AssetDatabase.SaveAssets();
	}

	private void DrawPrefabDropArea() {
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
						.OfType<GameObject>().Any();
					DragAndDrop.visualMode = containsPrefabObject
						? DragAndDropVisualMode.Copy
						: DragAndDropVisualMode.Rejected;
					Event.current.Use();
					Repaint();
					break;
				case EventType.DragPerform:
					if (editDatabaseObject != null && editPrefabGroup != null) {
						var count = DragAndDrop.objectReferences.Length;
						for (var i = 0; i < count; i++) {
							var obj = DragAndDrop.objectReferences[i];
							if (!(obj is GameObject)) continue;
							var prefab = obj as GameObject;
							var dataName =
								$"编号({editPrefabGroup.DataCount})";
							editPrefabGroup.PutPrefabData(dataName, prefab.name,
								prefab);
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
		GUI.Box(dropRect, "+将预制体资源拖放至此");
		GUI.color = color;
		EditorGUILayout.EndHorizontal();
	}
}