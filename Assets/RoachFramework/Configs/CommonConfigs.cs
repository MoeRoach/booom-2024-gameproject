using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 通用配置项
    /// </summary>
    public static class CommonConfigs {

        public const string BROADCAST_FILTER_PANEL_REQUEST = "PanelRequest";
        public const string BROADCAST_FILTER_PANEL_NOTIFY = "PanelNotify";

        public const string BROADCAST_FILTER_WINDOW_REQUEST = "WindowRequest";
        public const string BROADCAST_FILTER_WINDOW_NOTIFY = "WindowNotify";
        
        public const string BROADCAST_FILTER_SCENE_LOAD = "SceneLoad";

        public const string BROADCAST_CONTENT_SHOW = "Show";
        public const string BROADCAST_CONTENT_SHOWN = "Shown";
        public const string BROADCAST_CONTENT_HIDE = "Hide";
        public const string BROADCAST_CONTENT_HIDDEN = "Hidden";
        public const string BROADCAST_CONTENT_VIEW_BEHAVIOR_START = "ViewBehaviorStart";
        public const string BROADCAST_CONTENT_VIEW_BEHAVIOR_FINISH = "ViewBehaviorFinish";
        public const string BROADCAST_CONTENT_SWITCH_SCENE = "SwitchScene";
        public const string BROADCAST_CONTENT_APPEND_SCENE = "AppendScene";
        public const string BROADCAST_CONTENT_REMOVE_SCENE = "RemoveScene";
        
        public const string EXTRA_TAG_IDENTIFIER = "Identifier";
        public const string EXTRA_TAG_POSITION = "Position";
        public const string EXTRA_TAG_ARGS = "Args";
        public const string EXTRA_TAG_SCENE_NAME = "SceneName";

        public const string TARGET_SCENE_QUIT = "EXIT_GAME";
    }
}
