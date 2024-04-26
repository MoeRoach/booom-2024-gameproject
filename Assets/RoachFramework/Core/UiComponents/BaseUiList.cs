using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 列表抽象基类，不支持多种子项类型，使用Adapter机制
    /// </summary>
    public abstract class BaseUiList : BaseUiWidget {
        
        public const int InnerUpdateRate = 30;
        public RectOffset paddingOffset = new RectOffset();

        protected BaseAdapter baseAdapter; // 列表适配器
        protected ScrollRect scrollRect; // 滑动组件
        protected RectTransform contentRoot; // 内容根对象
        protected Transform cacheRoot; // 缓存区域根对象

        protected List<GameObject> itemList;
        protected Queue<GameObject> itemCache;
        protected float itemSize; // 子项尺寸，用于计算当前索引

        protected bool isListUpdate = false;

        protected override void LoadViews() {
            scrollRect = GetComponent<ScrollRect>();
            if (scrollRect != null) {
                contentRoot = scrollRect.content;
                InitializeContainer();
            } else {
                LogUtils.LogError("Cannot Find Scroll Rect Component on List Object - " + gameObject.name);
            }
        }

        protected abstract void InitializeContainer();

        protected override void LoadMembers() {
            itemList = new List<GameObject>();
            itemCache = new Queue<GameObject>();
        }

        public sealed override void UpdateViews() {
            if (gameObject.activeInHierarchy && !isListUpdate) {
                UpdateContent();
            }
        }

        protected virtual async void UpdateContent() {
            isListUpdate = true;
            DeleteExtraContent();
            var splitPoint = GetCurrentPosition();
            var cusor = splitPoint;
            var innerCount = 0;
            while (cusor < baseAdapter.GetCount()) {
                if (cusor >= itemList.Count) {
                    itemList.Add(TakeItemFromCache());
                }
                itemList[cusor] = baseAdapter.GetObject(itemList[cusor], cusor);
                SetItemIntoContent(itemList[cusor]);
                cusor++;
                innerCount++;
                if (innerCount < InnerUpdateRate) continue;
                innerCount = 0;
                await UniTask.Yield();
            }
            cusor = 0;
            while (cusor < splitPoint) {
                itemList[cusor] = baseAdapter.GetObject(itemList[cusor], cusor);
                SetItemIntoContent(itemList[cusor]);
                cusor++;
                innerCount++;
                if (innerCount < InnerUpdateRate) continue;
                innerCount = 0;
                await UniTask.Yield();
            }
            isListUpdate = false;
        }

        protected void DeleteExtraContent() {
            if (itemList.Count <= 0) return;
            var delta = itemList.Count - baseAdapter.GetCount();
            if (delta <= 0) return;
            // 表示需要回收一部分
            while (delta > 0) {
                PutItemIntoCache(itemList.GetLast());
                delta--;
            }
        }

        protected abstract int GetCurrentPosition();

        public virtual void SetAdapter(BaseAdapter adapter) {
            baseAdapter = adapter;
            baseAdapter.SetupUpdateReference(this); // 装载列表引用，观察者模式成立
            NotifyUpdate();
        }

        public virtual TAdapter GetAdapter<TAdapter>() where TAdapter : BaseAdapter {
            return baseAdapter as TAdapter;
        }

        protected void SetItemIntoContent(GameObject item) {
            item.transform.SetParent(contentRoot);
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.identity;
            if (Mathf.Approximately(item.transform.localPosition.z, 0f)) return;
            var pos = item.transform.localPosition;
            pos.z = 0f;
            item.transform.localPosition = pos;
        }

        protected GameObject TakeItemFromCache() {
            if (itemCache.Count <= 0) return null;
            var item = itemCache.Dequeue();
            item.SetActive(true);
            return item;
        }

        protected void PutItemIntoCache(GameObject item) {
            item.transform.SetParent(cacheRoot);
            item.SetActive(false);
            itemList.Remove(item);
            itemCache.Enqueue(item);
        }
    }
}
