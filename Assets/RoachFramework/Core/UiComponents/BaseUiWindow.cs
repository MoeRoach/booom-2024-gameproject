using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoachFramework {
    /// <summary>
    /// 窗口组件抽象基类
    /// </summary>
    public abstract class BaseUiWindow : BaseUiPanel, IDragable {
        
		public bool isPopup; // 是否支持弹出
		public bool isDragable; // 是否支持拖拽
		public DragableTrigger dragableTrigger;

		protected RectTransform rootTransform;
		protected Vector2 currentPointerPos; // 拖拽时的指针位置缓存
		protected bool isDragging; // 是否正在拖动

		protected virtual Camera UiCamera => null;

		protected override void PreLoad() {
			base.PreLoad();
			hideRaycast = false;
			hideRootObject = true;
		}

		protected override void LoadViews() {
			base.LoadViews();
			if (isDragable && dragableTrigger != null) {
				dragableTrigger.SetupDragable(this);
			}
		}

		protected override void RegisterToManager() {
			UiManager.Instance.RegisterWindow(this, isStatic);
		}

		/// <summary>
		/// 展示窗体
		/// </summary>
		public virtual void ShowWindow() {
			Show(!isPopup);
		}

		public virtual void ShowWindowAtPosition(Vector2 pos) {
			ViewRect.anchoredPosition = pos;
			ShowWindow();
		}

		/// <summary>
		/// 关闭窗体
		/// </summary>
		public virtual void DismissWindow() {
			Hide(!isPopup);
		}

		protected override void NotifyShow() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_WINDOW_NOTIFY, CommonConfigs.BROADCAST_CONTENT_SHOW);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			broadcastService.BroadcastInformation(info);
		}
		
		protected override void NotifyShown() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_WINDOW_NOTIFY, CommonConfigs.BROADCAST_CONTENT_SHOWN);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			broadcastService.BroadcastInformation(info);
		}
		
		protected override void NotifyHide() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_WINDOW_NOTIFY, CommonConfigs.BROADCAST_CONTENT_HIDE);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			info.SetupExtras(rets);
			broadcastService.BroadcastInformation(info);
		}
		
		protected override void NotifyHidden() {
			var info = BroadcastInfo.Create(CommonConfigs.BROADCAST_FILTER_WINDOW_NOTIFY, CommonConfigs.BROADCAST_CONTENT_HIDDEN);
			info.PutStringExtra(CommonConfigs.EXTRA_TAG_IDENTIFIER, idetifier.oid);
			broadcastService.BroadcastInformation(info);
		}

		public void OnDragBegin(PointerEventData pointer) {
			isDragging = RectTransformUtility.ScreenPointToLocalPointInRectangle(
				rootTransform, pointer.position, UiCamera, out currentPointerPos);
		}

		public void OnDragUpdate(PointerEventData pointer) {
			if (!isDragging) return;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform,
				pointer.position, UiCamera, out var pos)) return;
			var delta = pos - currentPointerPos;
			var nextPos = ViewRect.anchoredPosition + delta;
			ViewRect.anchoredPosition = nextPos;
			currentPointerPos = pos;
		}

		public void OnDragFinish(PointerEventData pointer) {
			isDragging = false;
		}

		protected override void Release() {
			UiManager.Instance.UnregisterWindow(idetifier.oid);
		}
    }
}
