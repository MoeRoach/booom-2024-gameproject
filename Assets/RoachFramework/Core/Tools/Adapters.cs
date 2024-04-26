using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 适配器基类
    /// </summary>
    public abstract class BaseAdapter : IAdapter {
        /// <summary>
        /// 列表刷新回调接口引用
        /// </summary>
        protected IViewUpdater observer;
        /// <summary>
        /// 返回需要显示的数据项目个数
        /// </summary>
        /// <returns>项目个数</returns>
        public abstract int GetCount();
        /// <summary>
        /// 通过索引值获取对应的数据项
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>数据项</returns>
        public abstract object GetItem(int index);
        /// <summary>
        /// 通过索引值获取项目唯一ID
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>唯一ID</returns>
        public abstract int GetItemId(int index);
        /// <summary>
        /// 通过索引值获取对应的游戏展示对象，存在重用机制
        /// </summary>
        /// <param name="prev">重用对象</param>
        /// <param name="index">索引值</param>
        /// <returns>更新后的对象</returns>
        public abstract GameObject GetObject(GameObject prev, int index);

        /// <summary>
        /// 设置观察者引用接口和名称
        /// </summary>
        /// <param name="v">观察者更新接口</param>
        public void SetupUpdateReference(IViewUpdater v) {
            observer = v;
        }

        /// <summary>
        /// 通知列表更新
        /// </summary>
        public virtual void NotifyDataChanged() {
            observer?.NotifyUpdate();
        }
    }
}
