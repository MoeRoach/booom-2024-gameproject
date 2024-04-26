using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 游戏对象管理器单例
    /// </summary>
    public class ObjectManager : BaseSingleton<ObjectManager> {
        
        private readonly Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, int> _instanceMap = new Dictionary<string, int>();

        protected override void OnInitialized() { }

        public void RegisterObject(string id, GameObject obj) {
            if (_instanceMap.ContainsKey(id)) {
                Debug.LogWarning($"Duplicated Object Id:{id}, will overwrite.");
                _objects.Remove($"{id}[{_instanceMap[id]:X}]");
            }

            _instanceMap[id] = obj.GetInstanceID();
            var oid = $"{id}[{_instanceMap[id]:X}]";
            _objects[oid] = obj;
        }

        public void UnregisterObject(string id) {
            if (!_instanceMap.ContainsKey(id)) {
                Debug.LogWarning($"No Object with Id:{id}.");
                return;
            }
            
            var oid = $"{id}[{_instanceMap[id]:X}]";
            _objects.Remove(oid);
        }

        public GameObject FindGameObject(string id) {
            if (!_instanceMap.ContainsKey(id)) {
                Debug.LogWarning($"No Object with Id:{id}.");
                return null;
            }
            
            var oid = $"{id}[{_instanceMap[id]:X}]";
            return _objects.TryGetElement(oid);
        }

        public TComponent FindComponent<TComponent>(string id) where TComponent : Component {
            if (!_instanceMap.ContainsKey(id)) {
                Debug.LogWarning($"No Object with Id:{id}.");
                return null;
            }
            
            var oid = $"{id}[{_instanceMap[id]:X}]";
            var obj = _objects.TryGetElement(oid);
            return obj != null ? obj.GetComponent<TComponent>() : null;
        }
    }
}
