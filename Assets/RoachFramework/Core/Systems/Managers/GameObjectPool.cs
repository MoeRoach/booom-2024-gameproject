using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 游戏对象池，采用工厂模式进行对象创建
    /// </summary>
    public class GameObjectPool : BaseSingleton<GameObjectPool>, ITagPool<GameObject> {

    	private Dictionary<string, Queue<GameObject>> _pool;
    	public event Action<string, GameObject> OnGetObject;
    	public event Action<string, GameObject> OnPutObject;
    	private ITagFactory<GameObject> _factory;

        protected override void OnInitialized() {
            _pool = new Dictionary<string, Queue<GameObject>>();
        }

        public void SetupFactory(ITagFactory<GameObject> factory) {
    		_factory = factory;
    	}

    	public GameObject Allocate(string t) {
    		if(!isInitialized)
    			throw new NotImplementedException("GameObject Pool not Initialized!");
    		if(_factory == null)
    			throw new NotImplementedException("GameObject Factory not found!");

    		GameObject ret = null;
    		if (_pool.ContainsKey(t) && _pool[t].Count > 0) {
    			CleanObjectPool(t);
    			ret = _pool[t].Count > 0 ? _pool[t].Dequeue() : _factory.Create(t);
    		} else {
    			ret = _factory.Create(t);
    		}

    		if (ret == null) {
    			Debug.LogWarning("Cannot Get Available GameObject!");
    			return ret;
    		}
    		var ip = ret.GetComponent<ITagPoolable>();
    		if (ip != null)
    			ip.IsRecycled = false;
    		OnGetObject?.Invoke(t, ret);
    		return ret;
    	}

    	private void CleanObjectPool(string t) {
    		var queue = _pool[t];
    		var count = queue.Count;
    		for (var i = 0; i < count; i++) {
    			var ob = queue.Dequeue();
    			if (ob == null) continue;
    			queue.Enqueue(ob);
    		}
    	}

    	public bool Recycle(GameObject obj) {
    		var ip = obj.GetComponent<ITagPoolable>();
    		var t = ip?.GetTag() ?? obj.tag;
    		
    		if (string.IsNullOrEmpty(t)) {
    			Debug.LogWarning("Cannot Cache GameObject without tag!");
    			return false;
    		}
    		
    		if(!_pool.ContainsKey(t))
    			_pool[t] = new Queue<GameObject>();
    		_pool[t].Enqueue(obj);

    		if (ip != null) {
    			ip.Recycle();
    			ip.IsRecycled = true;
    		}
    		
    		OnPutObject?.Invoke(t, obj);
    		return true;
    	}
    }
}
