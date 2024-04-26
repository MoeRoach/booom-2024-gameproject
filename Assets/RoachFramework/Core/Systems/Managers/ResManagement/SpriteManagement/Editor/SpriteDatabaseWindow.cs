// File create date:2021/6/24
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class SpriteDatabaseWindow : EditorWindow  {
	
	[MenuItem("Window/Data Manager/Sprite Data Setup")]
	private static void NewWindow() {
		var windowRect = new Rect(400, 300, 800, 570);
		var window = (SpriteDatabaseWindow)GetWindowWithRect(typeof(SpriteDatabaseWindow), windowRect, true, "图片资源映射表");
		window.Show();
	}
	
	private SpriteDataRootObject rootObject;
	private SerializedObject windowObject;
	private Vector2 spriteDatabaseListPos;
	private Vector2 spriteGroupListPos;
	private SpriteDatabaseObject editDatabaseObject;
	private SpriteGroupData editSpriteGroup;
	private string createSpriteDatabaseName = string.Empty;
	private string createSpriteGroupName = string.Empty;

	private Color expandItemColor;
	private Color shrinkItemColor;
	
	private void OnEnable() {
		windowObject = new SerializedObject(this);
		LoadSpriteDataFromResource();
		expandItemColor = Color.green;
		expandItemColor.a = 0.25f;
		shrinkItemColor = Color.magenta;
		shrinkItemColor.a = 0.25f;
	}

	private void LoadSpriteDataFromResource() {
		if (!FileUtils.CheckDirectory(SpriteConfigs.PATH_ASSET_RESOURCE_FOLDER)) {
			FileUtils.CreateDirectory(SpriteConfigs.PATH_ASSET_RESOURCE_FOLDER);
		}

		var spriteDataFolder =
			$"{SpriteConfigs.PATH_ASSET_RESOURCE_FOLDER}/{SpriteConfigs.FOLDER_SPRITE_DATA}/";
		if (!FileUtils.CheckDirectory(spriteDataFolder)) {
			FileUtils.CreateDirectory(spriteDataFolder);
		}

		var spriteDatabaseFolder =
			$"{SpriteConfigs.PATH_ASSET_RESOURCE_FOLDER}/{SpriteConfigs.FOLDER_SPRITE_DATA}/{SpriteConfigs.FOLDER_SPRITE_DATABASE}/";
		if (!FileUtils.CheckDirectory(spriteDatabaseFolder)) {
			FileUtils.CreateDirectory(spriteDatabaseFolder);
		}

		var spriteDataRoot =
			$"{SpriteConfigs.PATH_ASSET_RESOURCE_FOLDER}/{SpriteConfigs.URI_SPRITE_DATA_ROOT}.asset";
		rootObject = !FileUtils.CheckFile(spriteDataRoot)
			? AssetUtils
				.CreateAsset<SpriteDataRootObject>(spriteDataFolder,
					SpriteConfigs
						.FILE_SPRITE_DATA_ROOT)
			: ResourceUtils
				.LoadResource<SpriteDataRootObject>(SpriteConfigs
					.URI_SPRITE_DATA_ROOT);
	}

	private void OnGUI() {
		windowObject.Update();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("保存图片数据")) {
			SaveDataToFile();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("图片数据库列表");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
		spriteDatabaseListPos = EditorGUILayout.BeginScrollView(spriteDatabaseListPos, GUILayout.Height(240f));
		DrawSpriteDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("创建数据库", GUILayout.Width(72f));
		createSpriteDatabaseName = EditorGUILayout.TextField(createSpriteDatabaseName);
		if (GUILayout.Button("创建", GUILayout.Width(72f))) {
			if (!string.IsNullOrEmpty(createSpriteDatabaseName)) {
				if (!rootObject.CheckDatabase(createSpriteDatabaseName)) {
					var spriteDatabaseFolder =
						$"{SpriteConfigs.PATH_ASSET_RESOURCE_FOLDER}/{SpriteConfigs.FOLDER_SPRITE_DATA}/{SpriteConfigs.FOLDER_SPRITE_DATABASE}/";
					var dbName =
						$"{SpriteConfigs.PREFIX_SPRITE_DATABASE}{createSpriteDatabaseName.Trim()}";
					var databaseObject = AssetUtils
						.CreateAsset<SpriteDatabaseObject>(spriteDatabaseFolder,
							dbName);
					databaseObject.databaseName = createSpriteDatabaseName;
					rootObject.AddSpriteDatabase(createSpriteDatabaseName,
						databaseObject);
					createSpriteDatabaseName = string.Empty;
					Repaint();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		if (string.IsNullOrEmpty(createSpriteDatabaseName)) {
			EditorGUILayout.HelpBox("请输入数据库名称", MessageType.Warning);
		} else {
			if (rootObject.CheckDatabase(createSpriteDatabaseName)) {
				EditorGUILayout.HelpBox("数据库已经存在", MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox("数据库可以使用", MessageType.Info);
			}
		}
		EditorGUILayout.LabelField("编辑图片精灵组");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(160f));
		spriteGroupListPos = EditorGUILayout.BeginScrollView(spriteGroupListPos, GUILayout.Height(160f));
		DrawSpriteDataList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if (editDatabaseObject != null && editSpriteGroup != null) {
			DrawSpriteDropArea();
		}
		EditorGUILayout.EndVertical();
		windowObject.ApplyModifiedProperties();
	}
	
	private void DrawSpriteDatabaseList() {
		EditorGUILayout.BeginVertical();
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleSpriteDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleSpriteDatabase(int index, string dbName) {
		if (rootObject != null) {
			var databaseObject = rootObject.spriteDatabases[index];
			var bgColor = databaseObject.isExpanded ? expandItemColor : shrinkItemColor;
			GUI.backgroundColor = bgColor;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			GUI.backgroundColor = Color.white;
			EditorGUILayout.BeginHorizontal();
			databaseObject.isExpanded = EditorGUILayout.Toggle(databaseObject.isExpanded, GUILayout.Width(16f));
			EditorGUILayout.LabelField(dbName, GUILayout.Width(150f));
			EditorGUILayout.LabelField(string.Empty);
			if (GUILayout.Button("删除", GUILayout.Width(100f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定数据库？", "确认", "取消")) {
					rootObject.RemoveDatabase(dbName);
					AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(databaseObject));
					Repaint();
				}
			}
			EditorGUILayout.EndHorizontal();
			if (databaseObject.isExpanded) {
				DrawSpriteGroupList(databaseObject);
			}
			EditorGUILayout.EndVertical();
		}
	}
	
	private void DrawSpriteGroupList(SpriteDatabaseObject database) {
		GUI.backgroundColor = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.backgroundColor = Color.white;
		EditorGUI.indentLevel++;
		for (var i = 0; i < database.groupNames.Count; i++) {
			var group = database.spriteGroups[i];
			DrawSingleSpriteGroup(database, group);
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("创建图片精灵组", GUILayout.Width(104f));
		createSpriteGroupName = EditorGUILayout.TextField(createSpriteGroupName, GUILayout.Width(280f));
		if (GUILayout.Button("创建", GUILayout.Width(100f))) {
			if (!string.IsNullOrEmpty(createSpriteGroupName)) {
				if (!database.CheckGroupData(createSpriteGroupName)) {
					var grp = new SpriteGroupData(createSpriteGroupName);
					database.AddSpriteGroup(createSpriteGroupName, grp);
					createSpriteGroupName = string.Empty;
					Repaint();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleSpriteGroup(SpriteDatabaseObject database, 
	SpriteGroupData groupData) {
		if (groupData != null) {
			var indexTag = groupData.DataCount > 0 ? "+" : "-";
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(indexTag, GUILayout.Width(24f));
			EditorGUILayout.LabelField(groupData.groupName);
			if (GUILayout.Button("编辑图片精灵组", GUILayout.Width(120f))) {
				SaveDataToFile();
				editDatabaseObject = database;
				editSpriteGroup = groupData;
				Repaint();
			}
			if (GUILayout.Button("删除图片精灵组", GUILayout.Width(120f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定图片精灵组？", "确认", "取消")) {
					database.RemoveGroupData(groupData.groupName);
					Repaint();
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
	
	private void DrawSpriteDataList() {
		EditorGUILayout.BeginVertical();
		if (editDatabaseObject != null && editSpriteGroup != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"当前数据库:[{editDatabaseObject.databaseName}][{editSpriteGroup.groupName}]", GUILayout.Width(200f));
			EditorGUILayout.EndHorizontal();
			if (editSpriteGroup != null) {
				EditorGUI.indentLevel++;
				for (var i = 0; i < editSpriteGroup.DataCount; i++) {
					var data = editSpriteGroup.groupData[i];
					DrawSingleSpriteData(i, data);
				}
				EditorGUI.indentLevel--;
			}
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleSpriteData(int index, SpriteData data) {
		GUI.color = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.color = Color.white;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField($"精灵名称: {data.spriteName}");
		EditorGUILayout.LabelField("数据标签:", GUILayout.Width(72f));
		data.dataName = EditorGUILayout.TextField(data.dataName, GUILayout.Width(160f));
		if (GUILayout.Button("删除", GUILayout.Width(120f))) {
			if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定图片精灵？", "确认", "取消")) {
				editSpriteGroup.groupData.RemoveAt(index);
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
	
	private void DrawSpriteDropArea() {
		GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
		var lastRect = GUILayoutUtility.GetLastRect();
		EditorGUILayout.BeginHorizontal();
		var dropRect = new Rect(lastRect.width - 200f, lastRect.y - 2f, 200f, 20f);
		var containsMouse = dropRect.Contains(Event.current.mousePosition);
		if (containsMouse) {
			switch (Event.current.type) {
				case EventType.DragUpdated:
					var containsSpriteObject = DragAndDrop.objectReferences
						.OfType<Sprite>().Any();
					DragAndDrop.visualMode = containsSpriteObject
						? DragAndDropVisualMode.Copy
						: DragAndDropVisualMode.Rejected;
					Event.current.Use();
					Repaint();
					break;
				case EventType.DragPerform:
					if (editDatabaseObject != null && editSpriteGroup != null) {
						var count = DragAndDrop.objectReferences.Length;
						for (var i = 0; i < count; i++) {
							var obj = DragAndDrop.objectReferences[i];
							if (!(obj is Sprite)) continue;
							var sprite = obj as Sprite;
							var dataName =
								$"编号({editSpriteGroup.DataCount})";
							editSpriteGroup.PutSpriteData(dataName, sprite.name,
								sprite);
						}
					}

					Event.current.Use();
					Repaint();
					break;
			}
		}

		var color = GUI.color;
		GUI.color = new Color(color.r, color.g, color.b, containsMouse ? 0.8f : 0.4f);
		GUI.Box(dropRect, "+将图片资源拖放至此");
		GUI.color = color;
		EditorGUILayout.EndHorizontal();
	}
}