using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 水平列表组件
    /// </summary>
    public class HorizontalListView : BaseUiList {
        
        public float spacing = 0f;

        protected override void InitializeContainer() {
            var posRef = new Vector2(0, 1);
            contentRoot.anchorMin = Vector2.zero;
            contentRoot.anchorMax = posRef;
            contentRoot.pivot = posRef;
            // 子项缓存挂载点
            var cacheRootObj = new GameObject("ObjectCacheRoot");
            cacheRoot = cacheRootObj.transform;
            cacheRoot.SetParent(contentRoot.parent);
            cacheRoot.localPosition = Vector3.zero;
            cacheRoot.localScale = Vector3.one;
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            var grp = contentRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            grp.padding = paddingOffset;
            grp.spacing = spacing;
            grp.childForceExpandHeight = true;
            grp.childControlHeight = true;
            grp.childControlWidth = true;
            var fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        protected override int GetCurrentPosition() {
            if (itemList.Count <= 0) return 0;
            var itemlayout = itemList[0].GetComponent<LayoutElement>();
            if (itemlayout == null) return 0;
            if (Mathf.Approximately(itemSize, 0f)) {
                itemSize = itemlayout.preferredWidth;
            }
            
            var colWidth = itemSize + spacing;
            return (int)((Mathf.Abs(contentRoot.localPosition.x) - paddingOffset.left) / colWidth);
        }
    }
}
