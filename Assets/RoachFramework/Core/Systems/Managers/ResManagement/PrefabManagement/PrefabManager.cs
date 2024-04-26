// File create date:2021/4/29
// Created By Yu.Liu
using System.Collections;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 预制体管理器
    /// </summary>
    public class PrefabManager : BaseSingleton<PrefabManager> {

        private bool _isPrefabDataReady;
        private readonly string _uriPrefabDataRoot;
        private PrefabDataRootObject _rootObject;

        private PrefabManager() {
            _uriPrefabDataRoot = PrefabConfigs.URI_PREFAB_DATA_ROOT;
        }
        
        protected override void OnInitialized() { }

        /// <summary>
        /// 预制体数据库同步加载
        /// </summary>
        public void LoadPrefabData() {
            _rootObject = Resources.Load<PrefabDataRootObject>(_uriPrefabDataRoot);
            _isPrefabDataReady = _rootObject != null;
        }

        /// <summary>
        /// 预制体数据库异步加载
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadPrefabDataAsync() {
            var request = Resources.LoadAsync<PrefabDataRootObject>(_uriPrefabDataRoot);
            yield return request;
            _rootObject = request.asset as PrefabDataRootObject;
            _isPrefabDataReady = _rootObject != null;
        }

        /// <summary>
        /// 获取预制体
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="name">预制体名称</param>
        /// <param name="index">预制体索引</param>
        /// <returns>预制体引用</returns>
        public GameObject GetPrefab(string dbName, string name, int index = -1) {
            if (!_isPrefabDataReady || string.IsNullOrEmpty(dbName) ||
                string.IsNullOrEmpty(name)) return null;
            var db = _rootObject.GetPrefabDatabase(dbName);
            if (db == null) return null;
            var grp = db.GetPrefabGroupData(name);
            if (grp == null) return null;
            return index >= 0 ? grp.GetPrefab(index) : grp.GetRandomPrefab();
        }

        /// <summary>
        /// 从指定预制体创建新游戏对象
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <param name="name">预制体名</param>
        /// <param name="parent">指定创建对象的父亲</param>
        /// <param name="index">预制体索引</param>
        /// <returns>游戏对象实例引用</returns>
        public GameObject CreateFromPrefab(string dbName, string name, Transform parent = null,
            int index = -1) {
            var prefab = GetPrefab(dbName, name, index);
            if (prefab == null) return null;
            var result = Object.Instantiate(prefab, parent);
            result.transform.localPosition = Vector3.zero;
            result.transform.localScale = Vector3.one;
            return result;
        }
    }
}
