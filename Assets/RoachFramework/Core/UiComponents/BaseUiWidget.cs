using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// UI控件的抽象基类，所有的UI控件均由此派生
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseUiWidget : BaseGameObject, IViewUpdater {

        public RectTransform ViewRect { get; protected set; }
        protected RectTransform parentRect;
        protected CanvasGroup canvasGroup;
        protected bool updateNotifier; // 数据刷新标志位

        /// <summary>
        /// 实现预初始化
        /// </summary>
        protected sealed override void OnAwake() {
            PreLoad();
            ViewRect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            parentRect = transform.parent as RectTransform;
            LoadMembers();
            LoadViews();
        }
        /// <summary>
        /// 实现初始化
        /// </summary>
        protected sealed override void OnStart() {
            PostLoad();
        }

        /// <summary>
        /// 预加载
        /// </summary>
        protected virtual void PreLoad() { }
        /// <summary>
        /// 加载成员数据
        /// </summary>
        protected virtual void LoadMembers() { }
        /// <summary>
        /// 加载视图组件
        /// </summary>
        protected virtual void LoadViews() { }
        /// <summary>
        /// 后处理
        /// </summary>
        protected virtual void PostLoad() { }
        /// <summary>
        /// 手动刷新，无延时
        /// </summary>
        public virtual void UpdateViews() { }

        /// <summary>
        /// 通知界面进行刷新，延时一帧
        /// </summary>
        public void NotifyUpdate(bool activate = false) {
            if (!gameObject.activeInHierarchy || !gameObject.activeSelf) return;
            if (activate && !gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
            BeforeUpdate();
            updateNotifier = true;
            YieldAction(ViewUpdate);
        }

        protected virtual void BeforeUpdate() { }

        /// <summary>
        /// 更新方法
        /// </summary>
        private void ViewUpdate() {
            UpdateViews();
            updateNotifier = false;
        }

        public void SetupAlpha(float alpha) {
            canvasGroup.alpha = alpha;
        }
    }
}
