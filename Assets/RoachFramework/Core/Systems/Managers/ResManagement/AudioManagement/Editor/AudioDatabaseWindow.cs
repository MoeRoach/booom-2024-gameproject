// File create date:2020/9/30
using System.Linq;
using RoachFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
// Created By Yu.Liu
public class AudioDatabaseWindow : EditorWindow {

    [MenuItem("Window/Data Manager/Audio Data Setup")]
    private static void NewWindow() {
        var windowRect = new Rect(400, 300, 800, 570);
        var window = (AudioDatabaseWindow) GetWindowWithRect(
            typeof(AudioDatabaseWindow), windowRect, true, "音频资源映射表");
        window.Show();
    }

    private AudioDataRootObject rootObject;

    private SerializedObject windowObject;
    private Vector2 audioDatabaseListPos;
    private Vector2 audioGroupListPos;

    private AudioDatabaseObject editDatabaseObject;

    private AudioGroupData editAudioGroup;

    private string createAudioDatabaseName = string.Empty;
    private string createAudioGroupName = string.Empty;

    private readonly string[] groupMode = {
        "Random",
        "Sequence",
        "Combined"
    };

    private string selectGroupMode = "Random";

    private Color expandItemColor;
    private Color shrinkItemColor;

    private void OnEnable() {
        windowObject = new SerializedObject(this);
        LoadAudioDataFromResource();
        expandItemColor = Color.green;
        expandItemColor.a = 0.25f;
        shrinkItemColor = Color.magenta;
        shrinkItemColor.a = 0.25f;
    }

    private void LoadAudioDataFromResource() {
        if (!FileUtils.CheckDirectory(AudioConfigs.PATH_ASSET_RESOURCE_FOLDER)
        ) {
            FileUtils.CreateDirectory(AudioConfigs.PATH_ASSET_RESOURCE_FOLDER);
        }

        var audioDataFolder =
            $"{AudioConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AudioConfigs.FOLDER_AUDIO_DATA}/";
        if (!FileUtils.CheckDirectory(audioDataFolder)) {
            FileUtils.CreateDirectory(audioDataFolder);
        }

        var audioDatabaseFolder =
            $"{AudioConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AudioConfigs.FOLDER_AUDIO_DATA}/{AudioConfigs.FOLDER_AUDIO_DATABASE}/";
        if (!FileUtils.CheckDirectory(audioDatabaseFolder)) {
            FileUtils.CreateDirectory(audioDatabaseFolder);
        }

        var audioDataRoot =
            $"{AudioConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AudioConfigs.URI_AUDIO_DATA_ROOT}.asset";
        rootObject = !FileUtils.CheckFile(audioDataRoot)
            ? AssetUtils.CreateAsset<AudioDataRootObject>(audioDataFolder,
                AudioConfigs.FILE_AUDIO_DATA_ROOT)
            : ResourceUtils.LoadResource<AudioDataRootObject>(AudioConfigs
                .URI_AUDIO_DATA_ROOT);
    }

    private void OnGUI() {
        windowObject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存音频数据")) {
            SaveDataToFile();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("音频数据库列表");
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(240f));
        audioDatabaseListPos =
            EditorGUILayout.BeginScrollView(audioDatabaseListPos,
                GUILayout.Height(240f));
        DrawAudioDatabaseList();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("创建数据库", GUILayout.Width(72f));
        createAudioDatabaseName =
            EditorGUILayout.TextField(createAudioDatabaseName);
        if (GUILayout.Button("创建", GUILayout.Width(72f))) {
            if (!string.IsNullOrEmpty(createAudioDatabaseName)) {
                if (!rootObject.CheckDatabase(createAudioDatabaseName)) {
                    var audioDatabaseFolder =
                        $"{AudioConfigs.PATH_ASSET_RESOURCE_FOLDER}/{AudioConfigs.FOLDER_AUDIO_DATA}/{AudioConfigs.FOLDER_AUDIO_DATABASE}/";
                    var dbName =
                        $"{AudioConfigs.PREFIX_AUDIO_DATABASE}{createAudioDatabaseName.Trim()}";
                    var databaseObject =
                        AssetUtils.CreateAsset<AudioDatabaseObject>(
                            audioDatabaseFolder, dbName);
                    databaseObject.databaseName = createAudioDatabaseName;
                    rootObject.AddAudioDatabase(createAudioDatabaseName,
                        databaseObject);
                    createAudioDatabaseName = string.Empty;
                    Repaint();
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        if (string.IsNullOrEmpty(createAudioDatabaseName)) {
            EditorGUILayout.HelpBox("请输入数据库名称", MessageType.Warning);
        } else {
            if (rootObject.CheckDatabase(createAudioDatabaseName)) {
                EditorGUILayout.HelpBox("数据库已经存在", MessageType.Warning);
            } else {
                EditorGUILayout.HelpBox("数据库可以使用", MessageType.Info);
            }
        }

        EditorGUILayout.LabelField("编辑音频组");
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(160f));
        audioGroupListPos =
            EditorGUILayout.BeginScrollView(audioGroupListPos,
                GUILayout.Height(160f));
        DrawAudioDataList();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        if (editDatabaseObject != null && editAudioGroup != null) {
            DrawAudioDropArea();
        }

        EditorGUILayout.EndVertical();
        windowObject.ApplyModifiedProperties();
    }

    private void DrawAudioDatabaseList() {
        EditorGUILayout.BeginVertical();
        for (var i = 0; i < rootObject.databaseNames.Count; i++) {
            DrawSingleAudioDatabase(i, rootObject.databaseNames[i]);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSingleAudioDatabase(int index, string dbName) {
        if (rootObject != null) {
            var databaseObject = rootObject.audioDatabases[index];
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
            EditorGUILayout.LabelField("输出混音器", GUILayout.Width(80f));
            databaseObject.databaseMixer = EditorGUILayout.ObjectField(
                    GUIContent.none, databaseObject.databaseMixer,
                    typeof(AudioMixerGroup), false, GUILayout.Width(180f)) as
                AudioMixerGroup;
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
                DrawAudioGroupList(databaseObject);
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawAudioGroupList(AudioDatabaseObject database) {
        GUI.backgroundColor = expandItemColor;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUI.backgroundColor = Color.white;
        EditorGUI.indentLevel++;
        for (var i = 0; i < database.groupNames.Count; i++) {
            var group = database.audioGroups[i];
            DrawSingleAudioGroup(database, group);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("创建音频组", GUILayout.Width(80f));
        createAudioGroupName = EditorGUILayout.TextField(createAudioGroupName,
            GUILayout.Width(280f));
        if (GUILayout.Button("创建", GUILayout.Width(100f))) {
            if (!string.IsNullOrEmpty(createAudioGroupName)) {
                if (!database.CheckGroupData(createAudioGroupName)) {
                    var grp = new AudioGroupData(createAudioGroupName);
                    database.AddAudioGroup(createAudioGroupName, grp);
                    createAudioGroupName = string.Empty;
                    Repaint();
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    private void DrawSingleAudioGroup(AudioDatabaseObject database,
        AudioGroupData groupData) {
        if (groupData != null) {
            var indexTag = groupData.dataCount > 0 ? "+" : "-";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(indexTag, GUILayout.Width(24f));
            EditorGUILayout.LabelField(groupData.groupName);
            var grpmode = groupMode[groupData.groupPlayMode];
            var content = new GUIContent(grpmode);
            if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard,
                GUILayout.Width(120f))) {
                editDatabaseObject = database;
                editAudioGroup = groupData;
                var menu = new GenericMenu();
                foreach (var mode in groupMode) {
                    var modeContent = new GUIContent(mode);
                    menu.AddItem(modeContent, grpmode.Equals(mode),
                        OnGroupModeSelected, mode);
                }

                menu.ShowAsContext();
            }

            if (GUILayout.Button("编辑音频组", GUILayout.Width(120f))) {
                SaveDataToFile();
                editDatabaseObject = database;
                editAudioGroup = groupData;
                Repaint();
            }

            if (GUILayout.Button("删除音频组", GUILayout.Width(120f))) {
                if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定音频组？", "确认",
                    "取消")) {
                    database.RemoveGroupData(groupData.groupName);
                    Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void OnGroupModeSelected(object value) {
        selectGroupMode = value.ToString();
        for (var i = 0; i < groupMode.Length; i++) {
            if (selectGroupMode == groupMode[i]) {
                editAudioGroup.groupPlayMode = i;
                break;
            }
        }
    }

    private void DrawAudioDataList() {
        EditorGUILayout.BeginVertical();
        if (editDatabaseObject != null && editAudioGroup != null) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                $"当前数据库:[{editDatabaseObject.databaseName}][{editAudioGroup.groupName}]",
                GUILayout.Width(200f));
            EditorGUILayout.LabelField("Volume", GUILayout.Width(46f));
            editAudioGroup.groupVolume =
                EditorGUILayout.Slider(editAudioGroup.groupVolume, 0f, 1f);
            EditorGUILayout.LabelField("Pitch", GUILayout.Width(36f));
            editAudioGroup.groupPitch =
                EditorGUILayout.Slider(editAudioGroup.groupPitch, 0f, 1f);
            EditorGUILayout.LabelField("Spatial", GUILayout.Width(40f));
            editAudioGroup.groupSpatialBlend =
                EditorGUILayout.Slider(editAudioGroup.groupSpatialBlend, 0f,
                    1f);
            editAudioGroup.groupLoop = EditorGUILayout.ToggleLeft("循环设置",
                editAudioGroup.groupLoop, GUILayout.Width(70f));
            EditorGUILayout.EndHorizontal();
            if (editAudioGroup != null) {
                EditorGUI.indentLevel++;
                for (var i = 0; i < editAudioGroup.dataCount; i++) {
                    var data = editAudioGroup.groupData[i];
                    DrawSingleAudioData(i, data);
                }

                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSingleAudioData(int index, AudioData data) {
        GUI.color = expandItemColor;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"音频名称: {data.clipName}");
        EditorGUILayout.LabelField("数据标签:", GUILayout.Width(90f));
        data.dataName =
            EditorGUILayout.TextField(data.dataName, GUILayout.Width(160f));
        if (GUILayout.Button("删除", GUILayout.Width(120f))) {
            if (EditorUtility.DisplayDialog("删除确认", "请问是否要删除指定音频？", "确认", "取消")
            ) {
                editAudioGroup.groupData.RemoveAt(index);
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

    private void DrawAudioDropArea() {
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
                    var containsAudioClip = DragAndDrop.objectReferences
                        .OfType<AudioClip>().Any();
                    DragAndDrop.visualMode = containsAudioClip
                        ? DragAndDropVisualMode.Copy
                        : DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                    Repaint();
                    break;
                case EventType.DragPerform:
                    if (editDatabaseObject != null && editAudioGroup != null) {
                        var count = DragAndDrop.objectReferences.Length;
                        for (var i = 0; i < count; i++) {
                            var obj = DragAndDrop.objectReferences[i];
                            if (!(obj is AudioClip)) continue;
                            var clip = obj as AudioClip;
                            var dataName = $"编号({editAudioGroup.dataCount})";
                            editAudioGroup.PutAudioData(dataName, clip.name,
                                clip);
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
        GUI.Box(dropRect, "+将音频资源拖放至此");
        GUI.color = color;
        EditorGUILayout.EndHorizontal();
    }
}
