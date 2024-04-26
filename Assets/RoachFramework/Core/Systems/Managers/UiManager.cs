using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 窗口管理器，全局单例，不可销毁，包含静态窗体所需的Canvas
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(ObjectIdentifier))]
    public class UiManager : MonoSingleton<UiManager> {
        
        private Canvas _managerCanvas;
        
        private Transform _windowRoot;
        private readonly Dictionary<string, WindowInfo> _windowSet = new Dictionary<string, WindowInfo>();
        private readonly List<WindowInfo> _windowStack = new List<WindowInfo>();
        private ITagFactory<GameObject> _windowFactory;
        public event Action<bool> OverallWindowStateCallabck;

        private Transform _panelRoot;
        private readonly Dictionary<string, PanelInfo> _panelSet = new Dictionary<string, PanelInfo>();
        private readonly List<PanelInfo> _panelStack = new List<PanelInfo>();
        private ITagFactory<GameObject> _panelFactory;
        public event Action<bool> OverallPanelStateCallabck;

        protected override void OnAwake() {
            canBeDestroy = false;
            base.OnAwake();
            var wroot = new GameObject("WindowRoot");
            var wrect = wroot.AddComponent<RectTransform>();
            wrect.anchorMin = Vector2.zero;
            wrect.anchorMax = Vector2.one;
            _windowRoot = wrect;
            _windowRoot.SetParent(transform);
            var proot = new GameObject("PanelRoot");
            var prect = proot.AddComponent<RectTransform>();
            prect.anchorMin = Vector2.zero;
            prect.anchorMax = Vector2.one;
            _panelRoot = prect;
            _panelRoot.SetParent(transform);
        }
        
        public void SetupWindowFactory(ITagFactory<GameObject> factory) {
            _windowFactory = factory;
        }

        public void SetupPanelFactoru(ITagFactory<GameObject> factory) {
	        _panelFactory = factory;
        }

        protected override void SetupBroadcastFilter(BroadcastFilter filter) {
            base.SetupBroadcastFilter(filter);
            filter.AddFilter(CommonConfigs.BROADCAST_FILTER_PANEL_REQUEST);
            filter.AddFilter(CommonConfigs.BROADCAST_FILTER_PANEL_NOTIFY);
            filter.AddFilter(CommonConfigs.BROADCAST_FILTER_WINDOW_REQUEST);
            filter.AddFilter(CommonConfigs.BROADCAST_FILTER_WINDOW_NOTIFY);
        }

        public override void ReceiveBroadcast(BroadcastInfo info) {
            base.ReceiveBroadcast(info);
            if (info.Action == CommonConfigs.BROADCAST_FILTER_WINDOW_REQUEST) {
	            if (info.Content == CommonConfigs.BROADCAST_CONTENT_SHOW) {
		            var wid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (!string.IsNullOrEmpty(wid) && _windowSet.ContainsKey(wid)) {
			            var winfo = _windowSet[wid];
			            var args = info.ContainsExtra(CommonConfigs.EXTRA_TAG_ARGS)
				            ? info.GetComplexExtra<Dictionary<string, string>>(CommonConfigs.EXTRA_TAG_ARGS) : null;
			            if (args != null) winfo.windowObject.SetupArgs(args);
			            if (info.ContainsExtra(CommonConfigs.EXTRA_TAG_POSITION)) {
				            var pos = info.GetComplexExtra<CVector2>(CommonConfigs.EXTRA_TAG_POSITION);
				            winfo.windowObject.ShowWindowAtPosition(pos);
			            } else {
				            winfo.windowObject.ShowWindow();
			            }

			            UpdateWindowOrder(wid);
		            }
	            } else if (info.Content == CommonConfigs.BROADCAST_CONTENT_HIDE) {
		            var wid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (!string.IsNullOrEmpty(wid) && _windowSet.ContainsKey(wid)) {
			            var winfo = _windowSet[wid];
			            winfo.windowObject.DismissWindow();
			            SinkWindow(wid);
		            }
	            }
            } else if (info.Action == CommonConfigs.BROADCAST_FILTER_WINDOW_NOTIFY) {
	            if (info.Content == CommonConfigs.BROADCAST_CONTENT_SHOW) {
		            var wid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (_windowSet.ContainsKey(wid)) {
			            _windowSet[wid].isActive = true;
		            } else {
			            LogUtils.LogWarning(
				            $"Cannot Find Window with Identifier - [{wid}] when showing it.");
		            }

		            UpdateWindowState();
	            } else if (info.Content == CommonConfigs.BROADCAST_CONTENT_HIDE) {
		            var wid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (_windowSet.ContainsKey(wid)) {
			            _windowSet[wid].isActive = false;
		            } else {
			            LogUtils.LogWarning(
				            $"Cannot Find Window with Identifier - [{wid}] when hiding it.");
		            }

		            UpdateWindowState();
	            }
            } else if (info.Action == CommonConfigs.BROADCAST_FILTER_PANEL_REQUEST) {
	            if (info.Content == CommonConfigs.BROADCAST_CONTENT_SHOW) {
		            var pid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (!string.IsNullOrEmpty(pid) && _panelSet.ContainsKey(pid)) {
			            var pinfo = _panelSet[pid];
			            var args = info.ContainsExtra(CommonConfigs.EXTRA_TAG_ARGS)
				            ? info.GetComplexExtra<Dictionary<string, string>>(CommonConfigs.EXTRA_TAG_ARGS) : null;
			            if (args != null) pinfo.panelObject.SetupArgs(args);
			            pinfo.panelObject.Show();
			            UpdatePanelOrder(pid);
		            }
	            } else if (info.Content == CommonConfigs.BROADCAST_CONTENT_HIDE) {
		            var pid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (!string.IsNullOrEmpty(pid) && _panelSet.ContainsKey(pid)) {
			            var pinfo = _panelSet[pid];
			            pinfo.panelObject.Hide();
			            SinkPanel(pid);
		            }
	            }
            } else if (info.Action == CommonConfigs.BROADCAST_FILTER_PANEL_NOTIFY) {
	            if (info.Content == CommonConfigs.BROADCAST_CONTENT_SHOW) {
		            var pid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (_panelSet.ContainsKey(pid)) {
			            _panelSet[pid].isActive = true;
		            } else {
			            LogUtils.LogWarning(
				            $"Cannot Find Panel with Identifier - [{pid}] when showing it.");
		            }

		            UpdatePanelState();
	            } else if (info.Content == CommonConfigs.BROADCAST_CONTENT_HIDE) {
		            var pid = info.GetStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER);
		            if (_panelSet.ContainsKey(pid)) {
			            _panelSet[pid].isActive = false;
		            } else {
			            LogUtils.LogWarning(
				            $"Cannot Find Panel with Identifier - [{pid}] when hiding it.");
		            }

		            UpdatePanelState();
	            }
            }
        }

        #region Windows Functions

        public void RegisterWindow(BaseUiWindow window, bool isStatic = false) {
			var id = window.GetComponent<ObjectIdentifier>();
			if (!_windowSet.ContainsKey(id.oid)) {
				if (isStatic) {
					window.transform.SetParent(_windowRoot);
				}

				window.RegisterDestroyCallback(OnWindowDestroy);
				var info = new WindowInfo(window, isStatic);
				_windowSet[id.oid] = info;
				if (!window.startShow) return;
				info.windowOrder = _windowStack.Count;
				_windowStack.Add(info);
			} else {
				LogUtils.LogWarning(
					$"Cannot Register Window[{window.name}] because Identifier Duplication - [{id.oid}]");
			}
		}

		public void CreateWindow(string wTag, Transform canvasRoot, bool isStatic = false) {
			if (_windowFactory == null) {
				Debug.LogWarning($"Cannot create window: {wTag} because no Window Factory!");
				return;
			}

			var winObj = _windowFactory.Create(wTag);
			var window = winObj != null ? winObj.GetComponent<BaseUiWindow>() : null;
			if (window != null) {
				winObj.transform.SetParent(canvasRoot == null ? _windowRoot : canvasRoot);
			} else {
				LogUtils.LogError($"Cannot create window {wTag}, please check the Factory.");
			}
		}

		public void SetWindowStatic(string id, bool isStatic) {
			if (string.IsNullOrEmpty(id) || !_windowSet.ContainsKey(id)) return;
			if (!_windowSet[id].isStatic && isStatic) {
				_windowSet[id].windowObject.transform.SetParent(_windowRoot);
			}

			_windowSet[id].windowObject.isStatic = isStatic;
			_windowSet[id].isStatic = isStatic;
		}

		public void UnregisterWindow(string id) {
			if (string.IsNullOrEmpty(id) || !_windowSet.ContainsKey(id)) return;
			RemoveWindowFromStack(id);
			_windowSet.Remove(id);
		}

		private void OnWindowDestroy(GameObject winObj) {
			var id = winObj.GetComponent<ObjectIdentifier>();
			UnregisterWindow(id.oid);
		}

		private void UpdateWindowState() {
			var state = false;
			foreach (var id in _windowSet.Keys) {
				state |= _windowSet[id].isActive;
			}

			OverallWindowStateCallabck?.Invoke(state);
		}

		/// <summary>
		/// 将指定ID的窗口挪到栈顶
		/// </summary>
		/// <param name="id">窗口ID</param>
		private void UpdateWindowOrder(string id) {
			if (string.IsNullOrEmpty(id) || !_windowSet.ContainsKey(id)) return;
			var info = RemoveWindowFromStack(id);
			info.windowOrder = _windowStack.Count;
			info.windowObject.SetupSortingOrder(info.windowOrder + 1);
			_windowStack.Add(info);
		}

		/// <summary>
		/// 将指定ID的窗口沉底
		/// </summary>
		/// <param name="id"></param>
		private void SinkWindow(string id) {
			if (string.IsNullOrEmpty(id) || !_windowSet.ContainsKey(id)) return;
			var info = RemoveWindowFromStack(id);
			info.windowObject.SetupSortingOrder(info.windowOrder + 1);
			_windowStack.Insert(0, info);
		}

		private WindowInfo RemoveWindowFromStack(string id) {
			var info = _windowSet[id];
			var prevOrder = info.windowOrder;
			if (prevOrder >= 0 && prevOrder < _windowStack.Count) {
				_windowStack.RemoveAt(prevOrder);
				for (var i = prevOrder; i < _windowStack.Count; i++) {
					_windowStack[i].windowOrder--;
					_windowStack[i].windowObject
						.SetupSortingOrder(_windowStack[i].windowOrder + 1);
				}
			}

			info.windowOrder = -1;
			return info;
		}

		private void ClearWindows() {
			var wids = new HashSet<string>(_windowSet.Keys);
			foreach (var wid in wids) {
				UnregisterWindow(wid);
			}
		}

        #endregion

        #region Panels Functions

        public void RegisterPanel(BaseUiPanel panel, bool isStatic = false) {
	        var id = panel.GetComponent<ObjectIdentifier>();
	        if (!_panelSet.ContainsKey(id.oid)) {
		        if (isStatic) {
			        panel.transform.SetParent(_panelRoot);
		        }

		        panel.RegisterDestroyCallback(OnPanelDestroy);
		        var info = new PanelInfo(panel, isStatic);
		        _panelSet[id.oid] = info;
		        if (!panel.startShow) return;
		        info.panelOrder = _panelStack.Count;
		        _panelStack.Add(info);
	        } else {
		        LogUtils.LogWarning(
			        $"Cannot Register Panel[{panel.name}] because Identifier Duplication - [{id.oid}]");
	        }
        }

        public void CreatePanel(string pTag, Transform canvasRoot, bool isStatic = false) {
	        if (_panelFactory == null) {
		        Debug.LogWarning($"Cannot create panel: {pTag} because no panel Factory!");
		        return;
	        }

	        var panObj = _panelFactory.Create(pTag);
	        var panel = panObj != null ? panObj.GetComponent<BaseUiPanel>() : null;
	        if (panel != null) {
		        panObj.transform.SetParent(canvasRoot == null ? _panelRoot : canvasRoot);
	        } else {
		        LogUtils.LogError($"Cannot create panel {pTag}, please check the Factory.");
	        }
        }

        public void SetPanelStatic(string id, bool isStatic) {
	        if (string.IsNullOrEmpty(id) || !_panelSet.ContainsKey(id)) return;
	        if (!_panelSet[id].isStatic && isStatic) {
		        _panelSet[id].panelObject.transform.SetParent(_panelRoot);
	        }

	        _panelSet[id].panelObject.isStatic = isStatic;
	        _panelSet[id].isStatic = isStatic;
        }

        public void UnregisterPanel(string id) {
	        if (string.IsNullOrEmpty(id) || !_panelSet.ContainsKey(id)) return;
	        RemovePanelFromStack(id);
	        _panelSet.Remove(id);
        }

        private void OnPanelDestroy(GameObject panObj) {
	        var id = panObj.GetComponent<ObjectIdentifier>();
	        UnregisterPanel(id.oid);
        }

        private void UpdatePanelState() {
	        var state = false;
	        foreach (var id in _panelSet.Keys) {
		        state |= _panelSet[id].isActive;
	        }

	        OverallPanelStateCallabck?.Invoke(state);
        }

        /// <summary>
        /// 将指定ID的面板挪到栈顶
        /// </summary>
        /// <param name="id">窗口ID</param>
        private void UpdatePanelOrder(string id) {
	        if (string.IsNullOrEmpty(id) || !_panelSet.ContainsKey(id)) return;
	        var info = RemovePanelFromStack(id);
	        info.panelOrder = _panelStack.Count;
	        info.panelObject.SetupSortingOrder(info.panelOrder + 1);
	        _panelStack.Add(info);
        }

        /// <summary>
        /// 将指定ID的面板沉底
        /// </summary>
        /// <param name="id"></param>
        private void SinkPanel(string id) {
	        if (string.IsNullOrEmpty(id) || !_panelSet.ContainsKey(id)) return;
	        var info = RemovePanelFromStack(id);
	        info.panelObject.SetupSortingOrder(info.panelOrder + 1);
	        _panelStack.Insert(0, info);
        }

        private PanelInfo RemovePanelFromStack(string id) {
	        var info = _panelSet[id];
	        var prevOrder = info.panelOrder;
	        if (prevOrder >= 0 && prevOrder < _panelStack.Count) {
		        _panelStack.RemoveAt(prevOrder);
		        for (var i = prevOrder; i < _panelStack.Count; i++) {
			        _panelStack[i].panelOrder--;
			        _panelStack[i].panelObject
				        .SetupSortingOrder(_panelStack[i].panelOrder + 1);
		        }
	        }

	        info.panelOrder = -1;
	        return info;
        }

        private void ClearPanels() {
	        var pids = new HashSet<string>(_panelSet.Keys);
	        foreach (var pid in pids) {
		        UnregisterPanel(pid);
	        }
        }

        #endregion

		protected override void Release() {
			base.Release();
			ClearWindows();
			ClearPanels();
		}

		/// <summary>
		/// 窗体信息
		/// </summary>
        private class WindowInfo {

            public string objectName;
            public string windowIdentifier;
            public BaseUiWindow windowObject;
            public int windowOrder;
            public bool isStatic;
            public bool isActive;

            public WindowInfo(BaseUiWindow window, bool isStatic) {
                objectName = window.name;
                var id = window.GetComponent<ObjectIdentifier>();
                windowIdentifier = id.oid;
                windowObject = window;
                windowOrder = -1;
                this.isStatic = isStatic;
                isActive = false;
            }
        }

		/// <summary>
		/// 面板信息
		/// </summary>
        private class PanelInfo {

	        public string objectName;
	        public string panelIdentifier;
	        public BaseUiPanel panelObject;
	        public int panelOrder;
	        public bool isStatic;
	        public bool isActive;

	        public PanelInfo(BaseUiPanel panel, bool isStatic) {
		        objectName = panel.name;
		        var id = panel.GetComponent<ObjectIdentifier>();
		        panelIdentifier = id.oid;
		        panelObject = panel;
		        panelOrder = -1;
		        this.isStatic = isStatic;
		        isActive = false;
	        }
        }
    }
}
