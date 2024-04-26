// File create date:2021/8/23
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class TextDatabaseWindow : EditorWindow {

	[MenuItem("Window/Data Manager/Text Data Setup")]
	private static void NewWindow() {
		var windowRect = new Rect(400, 300, 800, 570);
		var window = (TextDatabaseWindow) GetWindowWithRect(
			typeof(TextDatabaseWindow), windowRect, true, "文本资源映射表");
		window.Show();
	}

	private TextDataRootObject rootObject;

	private SerializedObject windowObject;
	private Vector2 textDatabaseListPos;
	private Vector2 textDataListPos;
	private Vector2 textContentPos;

	private TextDatabaseObject editDatabaseObject;

	private TextData editTextData;

	private string createTextDatabaseName = string.Empty;
	private string createTextDataName = string.Empty;

	private Color expandItemColor;
	private Color shrinkItemColor;
	private Color errorItemColor;

	private void OnEnable() {
		windowObject = new SerializedObject(this);
		LoadTextDataFromResource();
		expandItemColor = Color.green;
		expandItemColor.a = 0.25f;
		shrinkItemColor = Color.magenta;
		shrinkItemColor.a = 0.25f;
		errorItemColor = Color.red;
		errorItemColor.a = 0.25f;
	}

	private void LoadTextDataFromResource() {
		if (!FileUtils.CheckDirectory(TextConfigs.PATH_ASSET_RESOURCE_FOLDER)) {
			FileUtils.CreateDirectory(TextConfigs.PATH_ASSET_RESOURCE_FOLDER);
		}

		var spriteDataFolder =
			$"{TextConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TextConfigs.FOLDER_TEXT_DATA}/";
		if (!FileUtils.CheckDirectory(spriteDataFolder)) {
			FileUtils.CreateDirectory(spriteDataFolder);
		}

		var spriteDatabaseFolder =
			$"{TextConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TextConfigs.FOLDER_TEXT_DATA}/{TextConfigs.FOLDER_TEXT_DATABASE}/";
		if (!FileUtils.CheckDirectory(spriteDatabaseFolder)) {
			FileUtils.CreateDirectory(spriteDatabaseFolder);
		}

		var spriteDataRoot =
			$"{TextConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TextConfigs.URI_TEXT_DATA_ROOT}.asset";
		rootObject = !FileUtils.CheckFile(spriteDataRoot)
			? AssetUtils
				.CreateAsset<TextDataRootObject>(spriteDataFolder,
					TextConfigs
						.FILE_TEXT_DATA_ROOT)
			: ResourceUtils
				.LoadResource<TextDataRootObject>(TextConfigs
					.URI_TEXT_DATA_ROOT);
	}

	private void OnGUI() {
		windowObject.Update();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("保存文本数据")) {
			SaveDataToFile();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("文本数据库列表");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
		textDatabaseListPos =
			EditorGUILayout.BeginScrollView(textDatabaseListPos,
				GUILayout.Height(240f));
		DrawTextDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("创建数据库", GUILayout.Width(72f));
		createTextDatabaseName =
			EditorGUILayout.TextField(createTextDatabaseName);
		if (GUILayout.Button("创建", GUILayout.Width(72f))) {
			if (!string.IsNullOrEmpty(createTextDatabaseName)) {
				if (!rootObject.CheckDatabase(createTextDatabaseName)) {
					var spriteDatabaseFolder =
						$"{TextConfigs.PATH_ASSET_RESOURCE_FOLDER}/{TextConfigs.FOLDER_TEXT_DATA}/{TextConfigs.FOLDER_TEXT_DATABASE}/";
					var dbName =
						$"{TextConfigs.PREFIX_SPRITE_DATABASE}{createTextDatabaseName.Trim()}";
					var databaseObject = AssetUtils
						.CreateAsset<TextDatabaseObject>(spriteDatabaseFolder,
							dbName);
					databaseObject.databaseName = createTextDatabaseName;
					rootObject.AddTextDatabase(createTextDatabaseName,
						databaseObject);
					createTextDatabaseName = string.Empty;
					Repaint();
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		if (string.IsNullOrEmpty(createTextDatabaseName)) {
			EditorGUILayout.HelpBox("请输入数据库名称", MessageType.Warning);
		} else {
			if (rootObject.CheckDatabase(createTextDatabaseName)) {
				EditorGUILayout.HelpBox("数据库已经存在", MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox("数据库可以使用", MessageType.Info);
			}
		}

		EditorGUILayout.LabelField("编辑文本资源配置");
		DrawTextDataSetup();
		windowObject.ApplyModifiedProperties();
	}

	private void DrawTextDatabaseList() {
		EditorGUILayout.BeginVertical();
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleTextDatabase(i, rootObject.databaseNames[i]);
		}

		EditorGUILayout.EndVertical();
	}

	private void DrawSingleTextDatabase(int index, string dbName) {
		if (rootObject != null) {
			var databaseObject = rootObject.textDatabases[index];
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
				DrawTextDataList(databaseObject);
			}

			EditorGUILayout.EndVertical();
		}
	}

	private void DrawTextDataList(TextDatabaseObject database) {
		GUI.backgroundColor = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.backgroundColor = Color.white;
		EditorGUI.indentLevel++;
		for (var i = 0; i < database.dataNames.Count; i++) {
			var text = database.textDatas[i];
			DrawSingleTextData(database, text);
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("创建文本数据", GUILayout.Width(104f));
		createTextDataName =
			EditorGUILayout.TextField(createTextDataName,
				GUILayout.Width(280f));
		if (GUILayout.Button("创建", GUILayout.Width(100f))) {
			if (!string.IsNullOrEmpty(createTextDataName)) {
				if (!database.CheckTextData(createTextDataName)) {
					var data = new TextData(createTextDataName, string.Empty,
						null);
					database.AddTextData(createTextDataName, data);
					createTextDataName = string.Empty;
					Repaint();
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}

	private void
		DrawSingleTextData(TextDatabaseObject database, TextData data) {
		if (data != null) {
			Color itemColor =
				data.textAsset == null ? errorItemColor : expandItemColor;
			GUI.backgroundColor = itemColor;
			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			GUI.backgroundColor = Color.white;
			EditorGUILayout.LabelField(data.dataName);
			if (GUILayout.Button("编辑文本数据", GUILayout.Width(120f))) {
				SaveDataToFile();
				editDatabaseObject = database;
				editTextData = data;
				Repaint();
			}

			if (GUILayout.Button("删除文本数据", GUILayout.Width(120f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定文本数据？", "确认",
					"取消")) {
					database.RemoveTextData(data.dataName);
					Repaint();
				}
			}

			EditorGUILayout.EndHorizontal();
		}
	}

	private void DrawTextDataSetup() {
		EditorGUILayout.BeginVertical();
		if (editDatabaseObject != null && editTextData != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(
				$"当前数据库:[{editDatabaseObject.databaseName}][{editTextData.dataName}]");
			EditorGUILayout.EndHorizontal();
			if (editTextData != null) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				textContentPos = EditorGUILayout.BeginScrollView(textContentPos,
					GUILayout.Height(140f));
				EditorGUILayout.BeginVertical();
				if (editTextData.textAsset != null) {
					GUILayout.Label(editTextData.textAsset.text, EditorStyles
						.wordWrappedLabel);
				} else {
					EditorGUILayout.HelpBox("- 无内容 -", MessageType.Warning);
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
			}

			DrawTextDropArea();
		}

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

	private void DrawTextDropArea() {
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
					var containsSpriteObject = DragAndDrop.objectReferences
						.OfType<TextAsset>().Any();
					DragAndDrop.visualMode = containsSpriteObject
						? DragAndDropVisualMode.Copy
						: DragAndDropVisualMode.Rejected;
					Event.current.Use();
					Repaint();
					break;
				case EventType.DragPerform:
					if (editDatabaseObject != null && editTextData != null) {
						var count = DragAndDrop.objectReferences.Length;
						for (var i = 0; i < count; i++) {
							var obj = DragAndDrop.objectReferences[i];
							if (!(obj is TextAsset)) continue;
							var text = obj as TextAsset;
							editTextData.fileName = text.name;
							editTextData.textAsset = text;
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
		GUI.Box(dropRect, "+将文本资源拖放至此");
		GUI.color = color;
		EditorGUILayout.EndHorizontal();
	}
}