// File create date:2021/12/11
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class MaterialDatabaseWindow : EditorWindow  {
	
	[MenuItem("Window/Data Manager/Material Data Setup")]
	private static void NewWindow() {
		var windowRect = new Rect(400, 300, 800, 570);
		var window = (MaterialDatabaseWindow)GetWindowWithRect(typeof(MaterialDatabaseWindow), windowRect, true, "材质资源映射表");
		window.Show();
	}
	
	private MaterialDataRootObject rootObject;

	private SerializedObject windowObject;
	private Vector2 materialDatabaseListPos;
	private Vector2 materialGroupListPos;

	private MaterialDatabaseObject editDatabaseObject;

	private MaterialGroupData editMaterialGroup;

	private string createMaterialDatabaseName = string.Empty;
	private string createMaterialGroupName = string.Empty;

	private Color expandItemColor;
	private Color shrinkItemColor;
	
	private void OnEnable() {
		windowObject = new SerializedObject(this);
		LoadMaterialDataFromResource();
		expandItemColor = Color.green;
		expandItemColor.a = 0.25f;
		shrinkItemColor = Color.magenta;
		shrinkItemColor.a = 0.25f;
	}
	
	private void LoadMaterialDataFromResource() {
		if (!FileUtils.CheckDirectory(MaterialConfigs.PATH_ASSET_RESOURCE_FOLDER)) {
			FileUtils.CreateDirectory(MaterialConfigs.PATH_ASSET_RESOURCE_FOLDER);
		}

		var spriteDataFolder =
			$"{MaterialConfigs.PATH_ASSET_RESOURCE_FOLDER}/{MaterialConfigs.FOLDER_MATERIAL_DATA}/";
		if (!FileUtils.CheckDirectory(spriteDataFolder)) {
			FileUtils.CreateDirectory(spriteDataFolder);
		}

		var spriteDatabaseFolder =
			$"{MaterialConfigs.PATH_ASSET_RESOURCE_FOLDER}/{MaterialConfigs.FOLDER_MATERIAL_DATA}/{MaterialConfigs.FOLDER_MATERIAL_DATABASE}/";
		if (!FileUtils.CheckDirectory(spriteDatabaseFolder)) {
			FileUtils.CreateDirectory(spriteDatabaseFolder);
		}

		var spriteDataRoot =
			$"{MaterialConfigs.PATH_ASSET_RESOURCE_FOLDER}/{MaterialConfigs.URI_MATERIAL_DATA_ROOT}.asset";
		rootObject = !FileUtils.CheckFile(spriteDataRoot)
			? AssetUtils
				.CreateAsset<MaterialDataRootObject>(spriteDataFolder,
					MaterialConfigs
						.FILE_MATERIAL_DATA_ROOT)
			: ResourceUtils
				.LoadResource<MaterialDataRootObject>(MaterialConfigs
					.URI_MATERIAL_DATA_ROOT);
	}

	private void OnGUI() {
		windowObject.Update();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("保存材质数据")) {
			SaveDataToFile();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("材质数据库列表");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
		materialDatabaseListPos = EditorGUILayout.BeginScrollView(materialDatabaseListPos, 
		GUILayout.Height(240f));
		DrawMaterialDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("创建数据库", GUILayout.Width(72f));
		createMaterialDatabaseName = EditorGUILayout.TextField
		(createMaterialDatabaseName);
		if (GUILayout.Button("创建", GUILayout.Width(72f))) {
			if (!string.IsNullOrEmpty(createMaterialDatabaseName)) {
				if (!rootObject.CheckDatabase(createMaterialDatabaseName)) {
					var materialDatabaseFolder =
						$"{MaterialConfigs.PATH_ASSET_RESOURCE_FOLDER}/{MaterialConfigs.FOLDER_MATERIAL_DATA}/{MaterialConfigs.FOLDER_MATERIAL_DATABASE}/";
					var dbName =
						$"{MaterialConfigs.PREFIX_MATERIAL_DATABASE}{createMaterialDatabaseName.Trim()}";
					var databaseObject = AssetUtils
						.CreateAsset<MaterialDatabaseObject>(materialDatabaseFolder,
							dbName);
					databaseObject.databaseName = createMaterialDatabaseName;
					rootObject.AddSpriteDatabase(createMaterialDatabaseName,
						databaseObject);
					createMaterialDatabaseName = string.Empty;
					Repaint();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		if (string.IsNullOrEmpty(createMaterialDatabaseName)) {
			EditorGUILayout.HelpBox("请输入数据库名称", MessageType.Warning);
		} else {
			if (rootObject.CheckDatabase(createMaterialDatabaseName)) {
				EditorGUILayout.HelpBox("数据库已经存在", MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox("数据库可以使用", MessageType.Info);
			}
		}
		EditorGUILayout.LabelField("编辑材质组");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(160f));
		materialGroupListPos = EditorGUILayout.BeginScrollView(materialGroupListPos, GUILayout.Height(160f));
		DrawMaterialDataList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if (editDatabaseObject != null && editMaterialGroup != null) {
			DrawMaterialDropArea();
		}
		EditorGUILayout.EndVertical();
		windowObject.ApplyModifiedProperties();
	}
	
	private void DrawMaterialDatabaseList() {
		EditorGUILayout.BeginVertical();
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleMaterialDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleMaterialDatabase(int index, string dbName) {
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
				DrawMaterialGroupList(databaseObject);
			}
			EditorGUILayout.EndVertical();
		}
	}
	
	private void DrawMaterialGroupList(MaterialDatabaseObject database) {
		GUI.backgroundColor = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.backgroundColor = Color.white;
		EditorGUI.indentLevel++;
		for (var i = 0; i < database.groupNames.Count; i++) {
			var group = database.materialGroups[i];
			DrawSingleMaterialGroup(database, group);
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("创建材质组", GUILayout.Width(104f));
		createMaterialGroupName = EditorGUILayout.TextField(createMaterialGroupName, GUILayout.Width(280f));
		if (GUILayout.Button("创建", GUILayout.Width(100f))) {
			if (!string.IsNullOrEmpty(createMaterialGroupName)) {
				if (!database.CheckGroupData(createMaterialGroupName)) {
					var grp = new MaterialGroupData(createMaterialGroupName);
					database.AddMaterialGroup(createMaterialGroupName, grp);
					createMaterialGroupName = string.Empty;
					Repaint();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleMaterialGroup(MaterialDatabaseObject database, 
		MaterialGroupData groupData) {
		if (groupData != null) {
			var indexTag = groupData.DataCount > 0 ? "+" : "-";
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(indexTag, GUILayout.Width(24f));
			EditorGUILayout.LabelField(groupData.groupName);
			if (GUILayout.Button("编辑材质组", GUILayout.Width(120f))) {
				SaveDataToFile();
				editDatabaseObject = database;
				editMaterialGroup = groupData;
				Repaint();
			}
			if (GUILayout.Button("删除材质组", GUILayout.Width(120f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定材质组？", "确认", "取消")) {
					database.RemoveGroupData(groupData.groupName);
					Repaint();
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
	
	private void DrawMaterialDataList() {
		EditorGUILayout.BeginVertical();
		if (editDatabaseObject != null && editMaterialGroup != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"当前数据库:[{editDatabaseObject.databaseName}][{editMaterialGroup.groupName}]", GUILayout.Width(200f));
			EditorGUILayout.EndHorizontal();
			if (editMaterialGroup != null) {
				EditorGUI.indentLevel++;
				for (var i = 0; i < editMaterialGroup.DataCount; i++) {
					var data = editMaterialGroup.groupData[i];
					DrawSingleMaterialData(i, data);
				}
				EditorGUI.indentLevel--;
			}
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleMaterialData(int index, MaterialData data) {
		GUI.color = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.color = Color.white;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField($"材质名称: {data.materialName}");
		EditorGUILayout.LabelField("数据标签:", GUILayout.Width(72f));
		data.dataName = EditorGUILayout.TextField(data.dataName, GUILayout.Width(160f));
		if (GUILayout.Button("删除", GUILayout.Width(120f))) {
			if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定材质？", "确认", "取消")) {
				editMaterialGroup.groupData.RemoveAt(index);
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
	
	private void DrawMaterialDropArea() {
		GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
		var lastRect = GUILayoutUtility.GetLastRect();
		EditorGUILayout.BeginHorizontal();
		var dropRect = new Rect(lastRect.width - 200f, lastRect.y - 2f, 200f, 20f);
		var containsMouse = dropRect.Contains(Event.current.mousePosition);
		if (containsMouse) {
			switch (Event.current.type) {
				case EventType.DragUpdated:
					var containsMaterialObject = DragAndDrop.objectReferences
						.OfType<Material>().Any();
					DragAndDrop.visualMode = containsMaterialObject
						? DragAndDropVisualMode.Copy
						: DragAndDropVisualMode.Rejected;
					Event.current.Use();
					Repaint();
					break;
				case EventType.DragPerform:
					if (editDatabaseObject != null && editMaterialGroup != null) {
						var count = DragAndDrop.objectReferences.Length;
						for (var i = 0; i < count; i++) {
							var obj = DragAndDrop.objectReferences[i];
							if (!(obj is Material)) continue;
							var mat = obj as Material;
							var dataName =
								$"编号({editMaterialGroup.DataCount})";
							editMaterialGroup.PutSpriteData(dataName, mat.name,
								mat);
						}
					}

					Event.current.Use();
					Repaint();
					break;
			}
		}

		var color = GUI.color;
		GUI.color = new Color(color.r, color.g, color.b, containsMouse ? 0.8f : 0.4f);
		GUI.Box(dropRect, "+将材质资源拖放至此");
		GUI.color = color;
		EditorGUILayout.EndHorizontal();
	}
}