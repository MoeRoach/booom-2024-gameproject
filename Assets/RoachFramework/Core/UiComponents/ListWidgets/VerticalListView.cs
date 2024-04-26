using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 竖直动态列表组件
    /// </summary>
    public class VerticalListView : BaseUiList {
        
        public float spacing = 0f;

        protected override void InitializeContainer() {
            var posRef = new Vector2(0, 1);
            contentRoot.anchorMin = posRef;
            contentRoot.anchorMax = Vector2.one;
            contentRoot.pivot = posRef;
            // 子项缓存挂载点
            var cacheRootObj = new GameObject("ObjectCacheRoot");
            cacheRoot = cacheRootObj.transform;
            cacheRoot.SetParent(contentRoot.parent);
            cacheRoot.localPosition = Vector3.zero;
            cacheRoot.localScale = Vector3.one;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            var grp = contentRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            grp.padding = paddingOffset;
            grp.spacing = spacing;
            grp.childForceExpandWidth = true;
            grp.childControlHeight = true;
            grp.childControlWidth = true;
            var fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        protected override int GetCurrentPosition() {
            if (itemList.Count <= 0) return 0;
            var itemlayout = itemList[0].GetComponent<LayoutElement>();
            if (itemlayout == null) return 0;
            if (Mathf.Approximately(itemSize, 0f)) {
                itemSize = itemlayout.preferredHeight;
            }
            
            var rowHeight = itemSize + spacing;
            return (int)((Mathf.Abs(contentRoot.localPosition.y) - paddingOffset.top) / rowHeight);
        }
    }
}
