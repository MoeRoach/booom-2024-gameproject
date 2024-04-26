using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoachFramework {
    /// <summary>
    /// 拖曳功能响应器
    /// </summary>
    public class DragableTrigger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        private IDragable dragable;

        public void SetupDragable(IDragable id) {
            dragable = id;
        }
        
        public void OnBeginDrag(PointerEventData eventData) {
            dragable?.OnDragBegin(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            dragable?.OnDragUpdate(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            dragable?.OnDragFinish(eventData);
        }
    }
}
