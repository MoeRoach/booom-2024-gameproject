using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoachFramework {
	/// <summary>
	/// 广播接收器接口
	/// </summary>
	public interface IBroadcastReceiver {
		public void ReceiveBroadcast(BroadcastInfo info);
	}
	
    /// <summary>
    /// 可拖拽组件接口，提供拖拽回调方法
    /// </summary>
    public interface IDragable {
    	void OnDragBegin(PointerEventData pointer);
    	void OnDragUpdate(PointerEventData pointer);
    	void OnDragFinish(PointerEventData pointer);
    }
    
    /// <summary>
    /// 界面刷新接口，提供同步和隔帧刷新方法
    /// </summary>
    public interface IViewUpdater {
    	void UpdateViews();
    	void NotifyUpdate(bool activate = false);
    }
    
    /// <summary>
    /// 列表数据适配器接口，提供适配器所需的基础方法集
    /// </summary>
    public interface IAdapter {
    	void SetupUpdateReference(IViewUpdater v);
    	int GetCount(); // 获取列表项数目
    	int GetItemId(int index); // 获取项目ID
    	object GetItem(int index); // 获取项目数据
    	GameObject GetObject(GameObject prev, int index); // 生成项目对象
    }
    
    /// <summary>
    /// 工厂类接口
    /// </summary>
    /// <typeparam name="TObject">对象类型</typeparam>
    public interface IFactory<out TObject> {
    	/// <summary>
    	/// 创建对象
    	/// </summary>
    	/// <returns>对象实体</returns>
    	TObject Create();
    }

    /// <summary>
    /// 带标签的工厂类接口
    /// </summary>
    /// <typeparam name="TObject">对象类型</typeparam>
    public interface ITagFactory<out TObject> {
    	/// <summary>
    	/// 根据标签创建对象
    	/// </summary>
    	/// <param name="t">标签</param>
    	/// <returns>对象实体</returns>
    	TObject Create(string t);
    }
    
    /// <summary>
    /// 可入池对象接口
    /// </summary>
    public interface IPoolable {
    	/// <summary>
    	/// 对象是否已经回收
    	/// </summary>
    	bool IsRecycled { get; set; }

    	/// <summary>
    	/// 回收对象时的处理方法
    	/// </summary>
    	void Recycle();
    }
    
    /// <summary>
    /// 可入池标签对象接口
    /// </summary>
    public interface ITagPoolable : IPoolable {
    	/// <summary>
    	/// 获取对象Tag
    	/// </summary>
    	/// <returns>对象Tag</returns>
    	string GetTag();
    }

    /// <summary>
    /// 对象池接口
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface IPool<T> {
    	/// <summary>
    	/// 分配对象
    	/// </summary>
    	/// <returns></returns>
    	T Allocate();

    	/// <summary>
    	/// 回收对象
    	/// </summary>
    	/// <param name="obj">待回收对象实例</param>
    	/// <returns>回收结果</returns>
    	bool Recycle(T obj);
    }
    
    /// <summary>
    /// 标签对象池接口
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface ITagPool<T> {
    	/// <summary>
    	/// 根据标签分配对象
    	/// </summary>
    	/// <param name="t">对象标签</param>
    	/// <returns>对象实例</returns>
    	T Allocate(string t);

    	/// <summary>
    	/// 回收对象
    	/// </summary>
    	/// <param name="obj">待回收对象实例</param>
    	/// <returns>回收结果</returns>
    	bool Recycle(T obj);
    }
}
