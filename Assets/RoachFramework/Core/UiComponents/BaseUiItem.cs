using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoachFramework {
    /// <summary>
    /// 列表项的抽象基类
    /// </summary>
    public abstract class BaseUiItem : BaseUiWidget, IPointerEnterHandler, IPointerClickHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
        
        protected bool canClick;
        protected bool canLongPress;
        protected bool isPointerDown;
        protected bool isLongPressed;

        protected float longPressTime = 0.6f;
        protected float pressStartTime;

        protected int index;
        protected Action<int> onClick;
        protected Action<int> onLongPress;

        protected sealed override void LoadViews() {
            ItemLoadViews();
        }

        protected abstract void ItemLoadViews();

        protected sealed override void PostLoad() {
            RegisterUpdateFunction(1, UpdateLongPress);
            ItemPostLoad();
        }

        protected virtual void ItemPostLoad() { }

        /// <summary>
        /// 设置当前项目索引
        /// </summary>
        /// <param name="i">索引值</param>
        public void SetIndex(int i) {
            index = i;
        }

        public void OnClick(Action<int> callback) {
            onClick = callback;
            canClick = onClick != null;
        }

        public void OnLongPress(Action<int> callback) {
            onLongPress = callback;
            canLongPress = onLongPress != null;
        }

        public void AllowClick(bool allow) {
            canClick = allow;
        }

        public void AllowLongPress(bool allow) {
            canLongPress = allow;
        }

        public virtual void OnPointerEnter(PointerEventData eventData) { }

        public virtual void OnPointerClick(PointerEventData eventData) {
            if (canClick && !isLongPressed) {
                onClick?.Invoke(index);
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData) {
            pressStartTime = Time.unscaledTime;
            isPointerDown = true;
            isLongPressed = false;
        }

        public virtual void OnPointerUp(PointerEventData eventData) {
            isPointerDown = false;
        }

        public virtual void OnPointerExit(PointerEventData eventData) {
            isPointerDown = false;
        }

        protected void OnLongPress() {
            if (canLongPress) {
                onLongPress?.Invoke(index);
            }
        }

        protected virtual void UpdateLongPress() {
            if (!isPointerDown || isLongPressed) return;
            if (Time.unscaledTime - pressStartTime <= longPressTime) return;
            isLongPressed = true;
            OnLongPress();
        }
    }
}
