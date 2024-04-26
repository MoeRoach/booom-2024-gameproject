// File create date:2023/9/10
using System;
using System.Collections.Generic;
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class AnimaDatabaseWindow : EditorWindow {
	
	[MenuItem("Window/Data Manager/Anima Data Setup")]
	private static void NewWindow() {
		var windowRect = new Rect(400, 300, 800, 570);
		var window = (AnimaDatabaseWindow)GetWindowWithRect(typeof(AnimaDatabaseWindow), windowRect, true, "动画资源映射表");
		window.Show();
	}
	
	private AnimaDataRootObject rootObject;
	private SerializedObject windowObject;
	private Vector2 animaDatabaseListPos;
	private Vector2 animaGroupListPos;
	private AnimaDatabaseObject editDatabaseObject;
	private AnimaGroupData editAnimaGroup;
	private string createAnimaDatabaseName = string.Empty;
	private string createAnimaGroupName = string.Empty;
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
		if (!FileUtils.CheckDirectory(AnimaConfigs.PATH_ASSET_RESOURCE_FOLDER)) {
			FileUtils.CreateDirectory(AnimaConfigs.PATH_ASSET_RESOURCE_FOLDER);
		}

		var animaDataFolder =
			$"{AnimaConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AnimaConfigs.FOLDER_ANIMA_DATA}/";
		if (!FileUtils.CheckDirectory(animaDataFolder)) {
			FileUtils.CreateDirectory(animaDataFolder);
		}

		var animaDatabaseFolder =
			$"{AnimaConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AnimaConfigs.FOLDER_ANIMA_DATA}/{AnimaConfigs.FOLDER_ANIMA_DATABASE}/";
		if (!FileUtils.CheckDirectory(animaDatabaseFolder)) {
			FileUtils.CreateDirectory(animaDatabaseFolder);
		}

		var animaDataRoot =
			$"{AnimaConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AnimaConfigs.URI_ANIMA_DATA_ROOT}.asset";
		rootObject = !FileUtils.CheckFile(animaDataRoot)
			? AssetUtils.CreateAsset<AnimaDataRootObject>(animaDataFolder,
				AnimaConfigs.FILE_ANIMA_DATA_ROOT)
			: ResourceUtils.LoadResource<AnimaDataRootObject>(AnimaConfigs.URI_ANIMA_DATA_ROOT);
	}

	private void OnGUI() {
		windowObject.Update();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("保存动画数据")) {
			SaveDataToFile();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("动画数据库列表");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
		animaDatabaseListPos =
			EditorGUILayout.BeginScrollView(animaDatabaseListPos, GUILayout.Height(240f));
		DrawAnimaDatabaseList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("创建数据库", GUILayout.Width(72f));
		createAnimaDatabaseName = EditorGUILayout.TextField(createAnimaDatabaseName);
		if (GUILayout.Button("创建", GUILayout.Width(72f))) {
			if (!string.IsNullOrEmpty(createAnimaDatabaseName)) {
				if (!rootObject.CheckDatabase(createAnimaDatabaseName)) {
					var animaDatabaseFolder =
						$"{AnimaConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AnimaConfigs.FOLDER_ANIMA_DATA}/{AnimaConfigs.FOLDER_ANIMA_DATABASE}/";
					var dbName =
						$"{AnimaConfigs.PREFIX_ANIMA_DATABASE}{createAnimaDatabaseName.Trim()}";
					var databaseObject = AssetUtils
						.CreateAsset<AnimaDatabaseObject>(animaDatabaseFolder, dbName);
					databaseObject.databaseName = createAnimaDatabaseName;
					rootObject.AddAnimaDatabase(createAnimaDatabaseName, databaseObject);
					createAnimaDatabaseName = string.Empty;
					Repaint();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		if (string.IsNullOrEmpty(createAnimaDatabaseName)) {
			EditorGUILayout.HelpBox("请输入数据库名称", MessageType.Warning);
		} else {
			if (rootObject.CheckDatabase(createAnimaDatabaseName)) {
				EditorGUILayout.HelpBox("数据库已经存在", MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox("数据库可以使用", MessageType.Info);
			}
		}
		EditorGUILayout.LabelField("编辑动画片段组");
		EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(160f));
		animaGroupListPos =
			EditorGUILayout.BeginScrollView(animaGroupListPos, GUILayout.Height(160f));
		DrawAnimaDataList();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		if (editDatabaseObject != null && editAnimaGroup != null) {
			DrawSpriteDropArea();
		}
		EditorGUILayout.EndVertical();
		windowObject.ApplyModifiedProperties();
	}
	
	private void DrawAnimaDatabaseList() {
		EditorGUILayout.BeginVertical();
		for (var i = 0; i < rootObject.databaseNames.Count; i++) {
			DrawSingleAnimaDatabase(i, rootObject.databaseNames[i]);
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleAnimaDatabase(int index, string dbName) {
		if (rootObject != null) {
			var databaseObject = rootObject.animaDatabases[index];
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
				DrawAnimaGroupList(databaseObject);
			}
			EditorGUILayout.EndVertical();
		}
	}
	
	private void DrawAnimaGroupList(AnimaDatabaseObject database) {
		GUI.backgroundColor = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.backgroundColor = Color.white;
		EditorGUI.indentLevel++;
		for (var i = 0; i < database.groupNames.Count; i++) {
			var group = database.animaGroups[i];
			DrawSingleAnimaGroup(database, group);
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("创建动画片段组", GUILayout.Width(104f));
		createAnimaGroupName = EditorGUILayout.TextField(createAnimaGroupName, GUILayout.Width(280f));
		if (GUILayout.Button("创建", GUILayout.Width(100f))) {
			if (!string.IsNullOrEmpty(createAnimaGroupName)) {
				if (!database.CheckGroupData(createAnimaGroupName)) {
					var grp = new AnimaGroupData(createAnimaGroupName);
					database.AddAnimaGroup(createAnimaGroupName, grp);
					createAnimaGroupName = string.Empty;
					Repaint();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleAnimaGroup(AnimaDatabaseObject database, AnimaGroupData groupData) {
		if (groupData != null) {
			var indexTag = groupData.DataCount > 0 ? "+" : "-";
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(indexTag, GUILayout.Width(24f));
			EditorGUILayout.LabelField(groupData.groupName);
			if (GUILayout.Button("编辑动画片段组", GUILayout.Width(120f))) {
				SaveDataToFile();
				editDatabaseObject = database;
				editAnimaGroup = groupData;
				Repaint();
			}
			if (GUILayout.Button("删除动画片段组", GUILayout.Width(120f))) {
				if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定动画片段组？", "确认", "取消")) {
					database.RemoveGroupData(groupData.groupName);
					Repaint();
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
	
	private void DrawAnimaDataList() {
		EditorGUILayout.BeginVertical();
		if (editDatabaseObject != null && editAnimaGroup != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"当前数据库:[{editDatabaseObject.databaseName}][{editAnimaGroup.groupName}]", GUILayout.Width(200f));
			EditorGUILayout.EndHorizontal();
			if (editAnimaGroup != null) {
				EditorGUI.indentLevel++;
				for (var i = 0; i < editAnimaGroup.DataCount; i++) {
					var data = editAnimaGroup.groupData[i];
					DrawSingleAnimaData(i, data);
				}
				EditorGUI.indentLevel--;
			}
		}
		EditorGUILayout.EndVertical();
	}
	
	private void DrawSingleAnimaData(int index, AnimaData data) {
		GUI.color = expandItemColor;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.color = Color.white;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField($"片段名称: {data.animaName}");
		EditorGUILayout.LabelField("数据标签:", GUILayout.Width(72f));
		data.dataName = EditorGUILayout.TextField(data.dataName, GUILayout.Width(160f));
		if (GUILayout.Button("删除", GUILayout.Width(120f))) {
			if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定动画片段？", "确认", "取消")) {
				editAnimaGroup.groupData.RemoveAt(index);
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
						.OfType<AnimationClip>().Any();
					DragAndDrop.visualMode = containsSpriteObject
						? DragAndDropVisualMode.Copy
						: DragAndDropVisualMode.Rejected;
					Event.current.Use();
					Repaint();
					break;
				case EventType.DragPerform:
					if (editDatabaseObject != null && editAnimaGroup != null) {
						var count = DragAndDrop.objectReferences.Length;
						for (var i = 0; i < count; i++) {
							var obj = DragAndDrop.objectReferences[i];
							if (!(obj is AnimationClip)) continue;
							var clip = obj as AnimationClip;
							var dataName =
								$"编号({editAnimaGroup.DataCount})";
							editAnimaGroup.PutClipData(dataName, clip.name, clip);
						}
					}

					Event.current.Use();
					Repaint();
					break;
			}
		}

		var color = GUI.color;
		GUI.color = new Color(color.r, color.g, color.b, containsMouse ? 0.8f : 0.4f);
		GUI.Box(dropRect, "+将动画资源拖放至此");
		GUI.color = color;
		EditorGUILayout.EndHorizontal();
	}
}