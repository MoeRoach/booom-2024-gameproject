using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 基础面板组件抽象基类
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(ObjectIdentifier))]
    [RequireComponent(typeof(UiAnimator))]
    public abstract class BaseUiPanel : BaseUiWidget {
        
	    public bool isStatic; // 是否静态对象，即是否不会随场景加载而销毁
        public bool startShow; // 是否初始显示
		public bool hideRootObject; // 物体本身是否跟随激活状态
		public bool hideRaycast; // 射线检测是否跟随激活状态

		protected Canvas canvas;
		protected GraphicRaycaster raycaster;
		protected ObjectIdentifier idetifier;
		protected UiAnimator uiAnimator;
		protected Dictionary<string, string> args;
		protected Dictionary<string, string> rets;

		public bool IsAnimate => uiAnimator != null && uiAnimator.IsAnimate;
		public bool IsVisible => uiAnimator != null && uiAnimator.IsVisible;

		protected override void PreLoad() {
			base.PreLoad();
			canvas = GetComponent<Canvas>();
			raycaster = GetComponent<GraphicRaycaster>();
			idetifier = GetComponent<ObjectIdentifier>();
			uiAnimator = GetComponent<UiAnimator>();
			args = new Dictionary<string, string>();
			rets = new Dictionary<string, string>();
			uiAnimator.BeforeShow += OnBeforeShow;
			uiAnimator.OnShown += OnAfterShow;
			uiAnimator.BeforeHide += OnBeforeHide;
			uiAnimator.OnHiden += OnAfterHide;
		}

		protected override void PostLoad() {
			base.PostLoad();
			RegisterToManager();
		}

		protected virtual void RegisterToManager() {
			UiManager.Instance.RegisterPanel(this, isStatic);
		}

		protected override void OnLazyStart() {
			base.OnLazyStart();
			if (!startShow) Hide(true);
		}
		
		public virtual void SetupSortingLayer(string ln) {
			canvas.sortingLayerName = ln;
		}

		public virtual void SetupSortingOrder(int order) {
			canvas.sortingOrder = order;
		}

		#region Args Functions

		public void SetupArgs(Dictionary<string, string> data) {
			args = data;
		}

		public void InsertStringArg(string key, string val) {
			args[key] = val;
		}

		protected string GetStringArg(string key) {
			return args.TryGetElement(key);
		}

		public void InsertIntegerArg(string key, int val) {
			args[key] = val.ToString();
		}

		protected int GetIntegerArg(string key) {
			if (!args.ContainsKey(key)) return int.MinValue;
			var chk = int.TryParse(args[key], out var ret);
			return chk ? ret : int.MinValue;
		}

		public void InsertFloatArg(string key, float val) {
			args[key] = val.ToString("f4");
		}

		protected float GetFloatArg(string key) {
			if (!args.ContainsKey(key)) return float.NaN;
			var chk = float.TryParse(args[key], out var ret);
			return chk ? ret : float.NaN;
		}

		public void InsertArg<T>(string key, T val) where T : new() {
			args[key] = JsonConvert.SerializeObject(val);
		}

		protected T GetArg<T>(string key) where T : new() {
			return !args.ContainsKey(key) ? default : JsonConvert.DeserializeObject<T>(args[key]);
		}

		protected void DeleteArg(string key) {
			args.Remove(key);
		}

		#endregion

		#region Rets Functions

		protected void InsertStringRet(string key, string val) {
			rets[key] = val;
		}

		protected string GetStringRet(string key) {
			return rets.TryGetElement(key);
		}

		protected void InsertIntegerRet(string key, int val) {
			rets[key] = val.ToString();
		}

		protected int GetIntegerRet(string key) {
			if (!rets.ContainsKey(key)) return int.MinValue;
			var chk = int.TryParse(rets[key], out var ret);
			return chk ? ret : int.MinValue;
		}

		protected void InsertFloatRet(string key, float val) {
			rets[key] = val.ToString("f4");
		}

		protected float GetFloatRet(string key) {
			if (!rets.ContainsKey(key)) return float.NaN;
			var chk = float.TryParse(rets[key], out var ret);
			return chk ? ret : float.NaN;
		}

		protected void InsertRet<T>(string key, T val) where T : new() {
			rets[key] = JsonConvert.SerializeObject(val);
		}

		protected T GetRet<T>(string key) where T : new() {
			return !rets.ContainsKey(key) ? default : JsonConvert.DeserializeObject<T>(rets[key]);
		}

		protected void DeleteRet(string key) {
			rets.Remove(key);
		}

		protected void ClearRets() {
			rets.Clear();
		}

		#endregion
		
		public virtual void Show(bool instantFlag = false) {
			if (uiAnimator.IsVisible || uiAnimator.IsAnimate) return;
			PreShow();
			NotifyShow();
			if (hideRootObject) {
				gameObject.SetActive(true);
			} else {
				if (!uiAnimator.hasShowAnimation && hideRaycast) {
					canvasGroup.interactable = true;
					canvasGroup.blocksRaycasts = true;
				}
			}

			if (instantFlag) {
				uiAnimator.SetupVisible(true);
				if (!hideRaycast) return;
				canvasGroup.interactable = true;
				canvasGroup.blocksRaycasts = true;
			} else {
				uiAnimator.RequestShowAnimation();
			}
		}

		public virtual void Hide(bool instantFlag = false) {
			if (!uiAnimator.IsVisible || uiAnimator.IsAnimate) return;
			PreHide();
			NotifyHide();
			if (instantFlag) {
				uiAnimator.SetupVisible(false);
				if (!hideRaycast) return;
				canvasGroup.interactable = false;
				canvasGroup.blocksRaycasts = false;
			} else {
				uiAnimator.RequestHideAnimation();
			}
		}
		
		protected virtual void PreShow() { }

		protected virtual void OnBeforeShow() {
			if (!hideRaycast) return;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}

		protected virtual void OnAfterShow() {
			NotifyShown();
			NotifyUpdate();
		}
		
		protected virtual void PreHide() { }
		
		protected virtual void OnBeforeHide() { }

		protected virtual void OnAfterHide() {
			NotifyHidden();
			if (hideRootObject) {
				gameObject.SetActive(false);
				return;
			}
			if (!hideRaycast) return;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		protected virtual void NotifyShow() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_PANEL_NOTIFY, CommonConfigs.BROADCAST_CONTENT_SHOW);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			broadcastService.BroadcastInformation(info);
		}
		
		protected virtual void NotifyShown() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_PANEL_NOTIFY, CommonConfigs.BROADCAST_CONTENT_SHOWN);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			broadcastService.BroadcastInformation(info);
		}
		
		protected virtual void NotifyHide() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_PANEL_NOTIFY, CommonConfigs.BROADCAST_CONTENT_HIDE);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			info.SetupExtras(rets);
			broadcastService.BroadcastInformation(info);
		}
		
		protected virtual void NotifyHidden() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_PANEL_NOTIFY, CommonConfigs.BROADCAST_CONTENT_HIDDEN);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			broadcastService.BroadcastInformation(info);
		}

		protected override void Release() {
			UiManager.Instance.UnregisterPanel(idetifier.oid);
		}
    }
}
