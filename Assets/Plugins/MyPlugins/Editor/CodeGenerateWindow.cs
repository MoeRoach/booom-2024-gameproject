// File create date:2019/11/18
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class CodeGenerateWindow : EditorWindow {

    [MenuItem("Assets/Code Generate")]
    private static void NewWindow() {
        var windowRect = new Rect(400, 300, 960, 690);
        var window = (CodeGenerateWindow)GetWindowWithRect(typeof(CodeGenerateWindow), windowRect, true, "脚本自动生成");
        window.Show();
    }

    [MenuItem("GameObject/Code Generate", false, 10)]
    private static void AddWindow(MenuCommand menuCommand) {
        var windowRect = new Rect(400, 300, 960, 690);
        var window = (CodeGenerateWindow)GetWindowWithRect(typeof(CodeGenerateWindow), windowRect, true, "脚本自动生成");
        window.SetupViewRoot(menuCommand.context as GameObject);
        window.Show();
    }

    private const string TypeNameButton = "Button";
    private static CodeGenerateWindow codeWindow;
    private static string cacheScriptPath;

    private SerializedObject _serializedObj;
    private GameObject _lastRoot;
    private GameObject _viewRoot;

    private readonly string[] _rootType = {
        "MonoSingleton",
        "BaseObject", 
        "BaseWidget", 
        "BasePanel", 
        "BaseWindow", 
        "BaseItem",
        "BaseGraphic"
    };
    private string _selectType = "BaseObject";

    private readonly List<ObjectNode> _nodeList = new List<ObjectNode>();
    private Vector2 _nodeListPos;

    private ObjectNode _editNode;
    private Color _selectItemColor;
    private Color _unselectItemColor;

    private string _scriptName;
    private string _scriptInterface;

    private string _scriptPath;

    private readonly StringBuilder _codeBuilder = new StringBuilder();
    private readonly List<ObjectNode> _indexWidgets = new List<ObjectNode>();

    private void OnEnable() {
        _serializedObj = new SerializedObject(this);
        _selectItemColor = Color.blue;
        _selectItemColor.a = 0.25f;
        _unselectItemColor = Color.red;
        _unselectItemColor.a = 0.25f;
    }

    private void OnGUI() {
        _serializedObj.Update();
        if (codeWindow == null) {
            codeWindow = GetWindow<CodeGenerateWindow>();
        }
        DrawTitleArea();
        using (new EditorGUILayout.VerticalScope()) {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("对象列表：");
            DrawWidgetList();
            EditorGUILayout.Space();
            DrawFunctionArea();
        }
    }

    private void SetupViewRoot(GameObject root) {
        _viewRoot = root;
    }

    private void DrawTitleArea() {
        // 标题部分，包括根节点，脚本名称，父类以及接口
        using var vScope = new EditorGUILayout.VerticalScope(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("生成代码的根节点:", GUILayout.Width(100f));
        _viewRoot = EditorGUILayout.ObjectField(_viewRoot, typeof(GameObject), true) as GameObject;
        if (_viewRoot != null && _lastRoot != _viewRoot) {
            // 不同的根节点，刷新数据
            UpdateDataList();
            _scriptName = _viewRoot.name;
            _scriptInterface = "";
            _lastRoot = _viewRoot;
        }
        _scriptPath = cacheScriptPath;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("脚本名称:", GUILayout.Width(100f));
        _scriptName = EditorGUILayout.TextField(_scriptName);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("上级类型:", GUILayout.Width(100f));
        //scriptParent = EditorGUILayout.TextField(scriptParent);
        var content = new GUIContent(_selectType);
        if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard)) {
            var menu = new GenericMenu();
            foreach (var item in _rootType) {
                var itemContent = new GUIContent(item);
                menu.AddItem(itemContent, _selectType.Equals(item), OnValueSelected, item);
            }
            menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("实现接口:", GUILayout.Width(100f));
        _scriptInterface = EditorGUILayout.TextField(_scriptInterface);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void OnValueSelected(object value) {
        _selectType = value.ToString();
    }

    private void UpdateDataList() {
        _nodeList.Clear();
        var searchQueue = new Queue<SearchCell>();
        if (_viewRoot.transform.childCount > 0) {
            for (var i = 0; i < _viewRoot.transform.childCount; i++) {
                searchQueue.Enqueue(new SearchCell(_viewRoot.transform.GetChild(i), ""));
            }
        }
        while (searchQueue.Count > 0) {
            var cursorCell = searchQueue.Dequeue();
            if (GenerateSingleWidget(cursorCell, out var node)) {
                // 有后继节点
                for (var i = 0; i < cursorCell.root.childCount; i++) {
                    searchQueue.Enqueue(new SearchCell(cursorCell.root.GetChild(i), node.path));
                }
            }
            _nodeList.Add(node);
        }
    }

    private bool GenerateSingleWidget(SearchCell cell, out ObjectNode node) {
        node = new ObjectNode {
            isSelected = false,
            name = cell.root.name,
            path = $"{cell.parentPath}/{cell.root.name}",
            varName = cell.root.name,
            isCustomType = false,
            customType = string.Empty
        };
        var components = cell.root.GetComponents<Component>();
        foreach (var com in components) {
            node.typeList.Add(com.GetType().Name);
        }
        node.typeIndex = 0;
        node.genEvent = false;
        return cell.root.childCount > 0;
    }

    private void DrawWidgetList() {
        // 控件列表
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField("", GUILayout.Width(2f));
        EditorGUILayout.LabelField("S", GUILayout.Width(16f));
        EditorGUILayout.LabelField("名称", GUILayout.Width(100f));
        EditorGUILayout.LabelField("变量名", GUILayout.Width(160f));
        EditorGUILayout.LabelField("", GUILayout.Width(72f));
        EditorGUILayout.LabelField("类型", GUILayout.Width(100f));
        EditorGUILayout.EndHorizontal();
        _nodeListPos = EditorGUILayout.BeginScrollView(_nodeListPos, GUILayout.Height(360f));
        foreach (var node in _nodeList) {
            DrawSingleNode(node);
        }
        if (_viewRoot == null) {
            EditorGUILayout.HelpBox("没有根节点", MessageType.Info);
        } else {
            if (_nodeList.Count <= 0) {
                EditorGUILayout.HelpBox("找不到指定物体", MessageType.Info);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
    }

    private void DrawSingleNode(ObjectNode node) {
        // 绘制单个控件信息
        GUI.backgroundColor = node.isSelected ? _selectItemColor : _unselectItemColor;
        using var vScope = new EditorGUILayout.VerticalScope(GUI.skin.box);
        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginHorizontal();
        node.isSelected = EditorGUILayout.Toggle(node.isSelected, GUILayout.Width(16f));
        EditorGUILayout.LabelField(node.name, GUILayout.Width(100f));
        node.varName = EditorGUILayout.TextField(node.varName, GUILayout.Width(160f));
        EditorGUILayout.LabelField("是否物体", GUILayout.Width(50f));
        node.isObject = EditorGUILayout.Toggle(node.isObject, GUILayout.Width(16f));
        if (!node.isObject) {
            if (node.isCustomType) {
                node.customType =
                    EditorGUILayout.TextField(node.customType, GUILayout.Width(140f));
            } else {
                var content = new GUIContent(node.typeList[node.typeIndex]);
                if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard,
                    GUILayout.Width(140f))) {
                    _editNode = node;
                    var menu = new GenericMenu();
                    foreach (var item in node.typeList) {
                        var itemContent = new GUIContent(item);
                        menu.AddItem(itemContent, node.typeList[node.typeIndex].Equals(item),
                            OnNodeTypeSelected, item);
                    }

                    menu.ShowAsContext();
                }
            }

            node.isCustomType = EditorGUILayout.Toggle(node.isCustomType, GUILayout.Width(26f));
            EditorGUILayout.LabelField("是否自定义类型", GUILayout.Width(100f));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        node.genEvent = EditorGUILayout.Toggle(node.genEvent, GUILayout.Width(26f));
        EditorGUILayout.LabelField("生成UI事件", GUILayout.Width(90f));
        if (GUILayout.Button("转到", GUILayout.Width(40f))) {
            var target = _viewRoot.transform.Find(node.path.Substring(1)).gameObject;
            EditorGUIUtility.PingObject(target);
            Selection.activeObject = target;
        }
        EditorGUILayout.LabelField(node.path);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndHorizontal();
    }

    private void OnNodeTypeSelected(object value) {
        var type = value as string;
        _editNode.typeIndex = _editNode.typeList.IndexOf(type);
    }

    private void DrawFunctionArea() {
        // 功能区域
        using (new EditorGUILayout.VerticalScope()) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成路径:", GUILayout.Width(60f));
            _scriptPath = EditorGUILayout.TextField(_scriptPath);
            cacheScriptPath = _scriptPath;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("预览代码")) {
                CompileScriptCode();
                var windowRect = new Rect(400, 300, 600, 800);
                var previewWindow = GetWindowWithRect<CodePreviewWindow>(windowRect, true, "代码预览");
                previewWindow.SetupCode(_codeBuilder.ToString());
                previewWindow.Show();
            }
            if (GUILayout.Button("生成代码文件")) {
                CompileScriptCode();
                GenerateScriptFile();
            }
            if (string.IsNullOrEmpty(_scriptPath)) {
                EditorGUILayout.HelpBox("未设置路径，代码会被默认保存到Asset目录下", MessageType.Warning);
            } else {
                var filePath = GetScriptFilePath();
                if (File.Exists(filePath)) {
                    EditorGUILayout.HelpBox("指定代码文件已经存在，请修改类名或者路径", MessageType.Error);
                }
            }
        }
    }

    private void GenerateScriptFile() {
        var filePath = GetScriptFilePath();
        if (!File.Exists(filePath)) {
            File.WriteAllText(filePath, _codeBuilder.ToString(), new UTF8Encoding(true, false));
            AssetDatabase.ImportAsset(filePath);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }

    private string GetScriptFilePath() {
        var pathBuilder = new StringBuilder(_scriptPath);
        if (pathBuilder.Length > 0) {
            if (pathBuilder[0] == '/') {
                pathBuilder.Remove(0, 1);
            }
            if (pathBuilder[^1] != '/') {
                pathBuilder.Append('/');
            }
        }
        var filePath = pathBuilder.ToString();
        if (filePath.IndexOf("Assets", StringComparison.Ordinal) == 0) {
            // 头部发现一个Assets根目录
            filePath += $"{_scriptName}.cs";
        } else {
            // 头部没有Assets根目录，添加一个
            filePath = $"Assets/{filePath}{_scriptName}.cs";
        }
        return filePath;
    }

    private void CompileScriptCode() {
        // 按步骤一行一行插入代码即可
        _codeBuilder.Clear();
        _indexWidgets.Clear();
        AppendScriptHeader(_codeBuilder);
        AppendScriptClass(_codeBuilder);
    }

    private void AppendScriptHeader(StringBuilder builder) {
        builder.AppendLine($"// Generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine("using RoachLite;");
        builder.AppendLine("using RoachLite.Basic;");
        builder.AppendLine("using RoachLite.Services;");
        builder.AppendLine("using RoachLite.Services.Broadcast;");
        builder.AppendLine("using RoachLite.Services.Message;");
        builder.AppendLine("using RoachLite.Utils;");
        builder.AppendLine("using RoachLite.UIComponent;");
        builder.AppendLine("using RoachLite.Data;");
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("using UnityEngine.EventSystems;");
        builder.AppendLine("using UnityEngine.UI;");
        builder.AppendLine("// Generated By View Code Generator");
        builder.AppendLine();
    }

    private void AppendScriptClass(StringBuilder builder) {
        if (_selectType == "MonoSingleton") {
            builder.AppendLine(!string.IsNullOrEmpty(_scriptInterface)
                ? $"public class {_scriptName} : {_selectType}<{_scriptName}>, {_scriptInterface} {{"
                : $"public class {_scriptName} : {_selectType}<{_scriptName}> {{");
        } else {
            builder.AppendLine(!string.IsNullOrEmpty(_scriptInterface)
                ? $"public class {_scriptName} : {_selectType},{_scriptInterface} {{"
                : $"public class {_scriptName} : {_selectType} {{");
        }
        builder.AppendLine();
        if (_selectType == "ViewWindow") {
            builder.AppendLine($"\tpublic const string WINDOW_IDENTIFIER = \"{_scriptName}\";");
            builder.AppendLine();
        }
        if (_nodeList.Count > 0) {
            AppendScriptVariables(builder);
            builder.AppendLine();
        }
        switch (_selectType) {
            case "MonoSingleton":
                AppendOnAwakeSegment(builder);
                builder.AppendLine();
                AppendOnStartSegment(builder);
                builder.AppendLine();
                break;
            case "BaseObject":
                AppendOnAwakeSegment(builder);
                builder.AppendLine();
                AppendOnStartSegment(builder);
                builder.AppendLine();
                AppendOnLazyLoadSegment(builder);
                builder.AppendLine();
                AppendOnUpdateSegment(builder);
                builder.AppendLine();
                break;
            case "BaseWidget":
                AppendPreLoadSegment(builder);
                builder.AppendLine();
                AppendLoadMembersSegment(builder);
                builder.AppendLine();
                AppendLoadViewsSegment(builder);
                builder.AppendLine();
                AppendPostLoadSegment(builder);
                builder.AppendLine();
                AppendHandleMessageSegment(builder);
                builder.AppendLine();
                AppendUpdateViewSegment(builder);
                builder.AppendLine();
                break;
            case "BasePanel":
                AppendPreLoadSegment(builder);
                builder.AppendLine();
                AppendLoadMembersSegment(builder);
                builder.AppendLine();
                AppendLoadViewsSegment(builder);
                builder.AppendLine();
                AppendPostLoadSegment(builder);
                builder.AppendLine();
                AppendHandleMessageSegment(builder);
                builder.AppendLine();
                AppendUpdateViewSegment(builder);
                builder.AppendLine();
                break;
            case "BaseWindow":
                AppendPreLoadSegment(builder);
                builder.AppendLine();
                AppendLoadMembersSegment(builder);
                builder.AppendLine();
                AppendLoadViewsSegment(builder);
                builder.AppendLine();
                AppendPostLoadSegment(builder);
                builder.AppendLine();
                AppendHandleMessageSegment(builder);
                builder.AppendLine();
                AppendUpdateViewSegment(builder);
                builder.AppendLine();
                break;
            case "BaseItem":
                AppendPreLoadSegment(builder);
                builder.AppendLine();
                AppendLoadMembersSegment(builder);
                builder.AppendLine();
                AppendItemLoadViewsSegment(builder);
                builder.AppendLine();
                AppendItemPostLoadSegment(builder);
                builder.AppendLine();
                AppendHandleMessageSegment(builder);
                builder.AppendLine();
                AppendUpdateViewSegment(builder);
                builder.AppendLine();
                break;
            case "BaseGraphic":
                AppendOnAwakeSegment(builder);
                builder.AppendLine();
                AppendOnStartSegment(builder);
                builder.AppendLine();
                AppendDrawGraphic(builder);
                builder.AppendLine();
                break;
        }
        foreach (var item in _indexWidgets) {
            if (!item.genEvent) continue;
            if (item.isObject) {
                AppendEventTrigger(builder, item.varName);
            } else {
                if (item.isCustomType || item.typeList[item.typeIndex] != TypeNameButton)
                    continue;
                AppendButtonClickEvent(builder, item.varName);
                builder.AppendLine();
            }
        }
        AppendBroadcastReceiver(builder);
        builder.AppendLine("}");
    }

    private void AppendScriptVariables(StringBuilder builder) {
        foreach (var item in _nodeList) {
            if (!item.isSelected) continue;
            // 选中的才处理
            if (item.isObject) {
                builder.AppendLine($"\tprivate GameObject _{item.varName};");
            } else {
                builder.AppendLine(item.isCustomType
                    ? $"\tprivate {item.customType} _{item.varName};"
                    : $"\tprivate {item.typeList[item.typeIndex]} _{item.varName};");
            }
            _indexWidgets.Add(item);
        }
    }

    private void AppendOnAwakeSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void OnAwake() {");
        builder.AppendLine("\t\tbase.OnAwake();");
        foreach (var item in _indexWidgets) {
            if (item.isObject) {
                builder.AppendLine(
                    $"\t\t_{item.varName} = FindGameObject(\"{item.path.Substring(1)}\");");
            } else {
                builder.AppendLine(
                    item.isCustomType
                        ? $"\t\t_{item.varName} = FindComponent<{item.customType}>(\"{item.path.Substring(1)}\");"
                        : $"\t\t_{item.varName} = FindComponent<{item.typeList[item.typeIndex]}>(\"{item.path.Substring(1)}\");");
            }
        }

        builder.AppendLine("\t");
        foreach (var item in _indexWidgets) {
            if (!item.genEvent) continue;
            if (item.isObject) {
                builder.AppendLine(
                    $"\t\t_{item.varName}.SetupEventTrigger(On{CapitalFirstCharacter(item.varName)}Click, EventTriggerType.PointerClick);");
            } else {
                if (!item.isCustomType &&
                    item.typeList[item.typeIndex] == TypeNameButton) {
                    builder.AppendLine(
                        $"\t\t_{item.varName}.SetupButtonListener(On{CapitalFirstCharacter(item.varName)}Click);");
                }
            }
        }

        builder.AppendLine("\t}");
    }

    private void AppendOnStartSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void OnStart() {");
        builder.AppendLine("\t\tbase.OnStart();");
        builder.AppendLine("\t}");
    }
    
    private void AppendOnLazyLoadSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void OnLazyLoad() {");
        builder.AppendLine("\t\tbase.OnLazyLoad();");
        builder.AppendLine("\t}");
    }
    
    private void AppendOnUpdateSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void OnUpdate() {");
        builder.AppendLine("\t\tbase.OnUpdate();");
        builder.AppendLine("\t}");
    }

    private void AppendPreLoadSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void PreLoad() {");
        builder.AppendLine("\t\tbase.PreLoad();");
        builder.AppendLine("\t}");
    }

    private void AppendLoadMembersSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void LoadMembers() {");
        builder.AppendLine("\t\tbase.LoadMembers();");
        builder.AppendLine("\t}");
    }

    private void AppendLoadViewsSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void LoadViews() {");
        builder.AppendLine("\t\tbase.LoadViews();");
        foreach (var item in _indexWidgets) {
            if (item.isObject) {
                builder.AppendLine(
                    $"\t\t_{item.varName} = FindGameObject(\"{item.path.Substring(1)}\");");
            } else {
                builder.AppendLine(
                    item.isCustomType
                        ? $"\t\t_{item.varName} = FindComponent<{item.customType}>(\"{item.path.Substring(1)}\");"
                        : $"\t\t_{item.varName} = FindComponent<{item.typeList[item.typeIndex]}>(\"{item.path.Substring(1)}\");");
            }
        }
        builder.AppendLine();
        foreach (var item in _indexWidgets) {
            if (!item.genEvent) continue;
            if (item.isObject) {
                builder.AppendLine(
                    $"\t\t_{item.varName}.SetupEventTrigger(On{CapitalFirstCharacter(item.varName)}Click, EventTriggerType.PointerClick);");
            } else {
                if (!item.isCustomType && item.typeList[item.typeIndex] == TypeNameButton) {
                    builder.AppendLine(
                        $"\t\t_{item.varName}.SetupButtonListener(On{CapitalFirstCharacter(item.varName)}Click);");
                }
            }
        }
        builder.AppendLine("\t}");
    }

    private void AppendItemLoadViewsSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void ItemLoadViews() {");
        builder.AppendLine("\t\tbase.LoadViews();");
        foreach (var item in _indexWidgets) {
            if (item.isObject) {
                builder.AppendLine(
                    $"\t\t_{item.varName} = FindGameObject(\"{item.path.Substring(1)}\");");
            } else {
                builder.AppendLine(
                    item.isCustomType
                        ? $"\t\t_{item.varName} = FindComponent<{item.customType}>(\"{item.path.Substring(1)}\");"
                        : $"\t\t_{item.varName} = FindComponent<{item.typeList[item.typeIndex]}>(\"{item.path.Substring(1)}\");");
            }
        }
        builder.AppendLine();
        foreach (var item in _indexWidgets) {
            if (!item.genEvent) continue;
            if (item.isObject) {
                builder.AppendLine(
                    $"\t\t_{item.varName}.SetupEventTrigger(On{CapitalFirstCharacter(item.varName)}Click, EventTriggerType.PointerClick);");
            } else {
                if (!item.isCustomType && item.typeList[item.typeIndex] == TypeNameButton) {
                    builder.AppendLine(
                        $"\t\t_{item.varName}.SetupButtonListener(On{CapitalFirstCharacter(item.varName)}Click);");
                }
            }
        }
        builder.AppendLine("\t}");
    }

    private void AppendPostLoadSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void PostLoad() {");
        builder.AppendLine("\t\tbase.PostLoad();");
        builder.AppendLine("\t}");
    }

    private void AppendItemPostLoadSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void ItemPostLoad() {");
        builder.AppendLine("\t\tbase.PostLoad();");
        builder.AppendLine("\t}");
    }

    private void AppendHandleMessageSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void HandleMessage(MessageInfo e) {");
        builder.AppendLine("\t\tbase.HandleMessage(e);");
        builder.AppendLine("\t}");
    }

    private void AppendUpdateViewSegment(StringBuilder builder) {
        builder.AppendLine("\tpublic override void UpdateViews() {");
        builder.AppendLine("\t\tbase.UpdateViews();");
        builder.AppendLine("\t}");
    }

    private void AppendButtonClickEvent(StringBuilder builder, string varName) {
        builder.AppendLine($"\tprivate void On{CapitalFirstCharacter(varName)}Click() {{");
        builder.AppendLine("\t\t// Click Callback");
        builder.AppendLine("\t}");
    }

    private void AppendEventTrigger(StringBuilder builder, string varName) {
        builder.AppendLine($"\tprivate void On{CapitalFirstCharacter(varName)}Click(BaseEventData ev) {{");
        builder.AppendLine("\t\t// Click Callback");
        builder.AppendLine("\t}");
    }

    private void AppendBroadcastReceiver(StringBuilder builder) {
        builder.AppendLine("\tprivate void ReceiveBroadcast(BroadcastInfo info) {");
        builder.AppendLine("\t\t// Broadcast Process");
        builder.AppendLine("\t}");
    }

    private void AppendDrawGraphic(StringBuilder builder) {
        builder.AppendLine("\tpublic override void DrawGraphic(ref VertexHelper vh) {");
        builder.AppendLine("\t\tbase.DrawGraphic(vh);");
        builder.AppendLine("\t}");
    }

    private string CapitalFirstCharacter(string str) {
        return $"{char.ToUpper(str[0])}{str.Substring(1)}";
    }

    private class ObjectNode {
        public bool isSelected;
        public string name;
        public string varName;
        public bool isCustomType;
        public string customType;
        public List<string> typeList;
        public int typeIndex;
        public string path;
        public bool genEvent;
        public bool isObject;

        public ObjectNode() {
            typeList = new List<string>();
        }
    }

    private class SearchCell {
        public Transform root;
        public string parentPath;
        public SearchCell(Transform root, string path) {
            this.root = root;
            parentPath = path;
        }
    }
}
