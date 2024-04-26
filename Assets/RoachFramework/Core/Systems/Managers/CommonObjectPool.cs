using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 通用对象池
    /// </summary>
    /// <typeparam name="TObject">入池对象类型</typeparam>
    public class CommonObjectPool<TObject> : BaseSingleton<CommonObjectPool<TObject>>,
        IPool<TObject> where TObject : IPoolable {

        private Queue<TObject> _pool;
        public event Action<TObject> OnGetObject;
        public event Action<TObject> OnPutObject;
        private IFactory<TObject> _factory;

        protected override void OnInitialized() {
            _pool = new Queue<TObject>();
        }

        public void SetupFactory(IFactory<TObject> factory) {
            _factory = factory;
        }

        public TObject Allocate() {
            if(!isInitialized)
                throw new NotImplementedException("GameObject Pool not Initialized!");
            if(_factory == null)
                throw new NotImplementedException("GameObject Factory not found!");

            var ret = _pool.Count > 0 ? _pool.Dequeue() : _factory.Create();
            if (ret != null) {
                ret.IsRecycled = false;
            }
            OnGetObject?.Invoke(ret);
            return ret;
        }

        public bool Recycle(TObject obj) {
            if (obj == null) return false;
            _pool.Enqueue(obj);
            obj.Recycle();
            obj.IsRecycled = true;
            OnPutObject?.Invoke(obj);
            return true;
        }
    }
}
