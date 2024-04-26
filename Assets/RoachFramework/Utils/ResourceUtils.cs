using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 资源加载工具
    /// </summary>
    public class ResourceUtils {
        private const int CacheLimit = 64; // 缓存数量
        /// <summary>
        /// 资源缓存
        /// </summary>
        private static readonly Dictionary<string, UnityEngine.Object> resourceCache =
            new Dictionary<string, UnityEngine.Object>();
        private static Dictionary<string, UnityEngine.Object> ResourceCache {
            get {
                if (resourceCache.Count < CacheLimit) return resourceCache;
                var keyList = new List<string>(resourceCache.Keys);
                for (var i = 0; i < 10; i++) {
                    resourceCache.Remove(keyList[i]);
                }
                return resourceCache;
            }
        }
        private static readonly Dictionary<string, Sprite> staticSpriteCache = new Dictionary<string, Sprite>();
        /// <summary>
        /// 从Resource文件中加载Sprite，可以加载Multiple类型的Sprite，通过索引确定位置
        /// </summary>
        /// <param name="uri">Sprite的路径</param>
        /// <param name="index">分割型Sprite的索引</param>
        /// <returns>Sprite对象</returns>
        public static Sprite LoadSprite(string uri, int index = -1) {
            Sprite result;
            if (!TextUtils.HasData(uri)) return null;
            if (index == -1) { // 加载单个Sprite
                result = ResourceCache.TryGetElement(uri) as Sprite;
                if (result != null) return result;
                result = Resources.Load<Sprite>(uri);
                ResourceCache[uri] = result;
            } else { // 加载多个Sprite中的一个
                result = ResourceCache.TryGetElement(uri + "_" + index) as Sprite;
                if (result != null) return result;
                var spSet = Resources.LoadAll<Sprite>(uri);
                result = spSet[index];
                for (var i = 0; i < spSet.Length; i++) {
                    var key = uri + "_" + i;
                    if (!ResourceCache.ContainsKey(key)) {
                        ResourceCache[key] = spSet[i];
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 从Resource文件中加载Sprite，静态缓存，禁止加载分割型Sprite
        /// </summary>
        /// <param name="uri">Sprite的路径</param>
        /// <returns>Sprite对象</returns>
        public static Sprite LoadSpriteStatic(string uri) {
            if (!TextUtils.HasData(uri)) return null;
            var result = staticSpriteCache.TryGetElement(uri);
            if (result != null) return result;
            result = Resources.Load<Sprite>(uri);
            staticSpriteCache[uri] = result;
            return result;
        }
        /// <summary>
        /// 从预制对象创建新物体
        /// </summary>
        /// <typeparam name="T">预制对象的类型</typeparam>
        /// <param name="uri">预制对象的URI</param>
        /// <param name="parent">创建对象的父对象，为空则直接创建在根部</param>
        /// <returns>新建对象</returns>
        public static T CreateFromPrefab<T>(string uri, Transform parent = null) where T : UnityEngine.Object {
            if (!TextUtils.HasData(uri)) throw new NoSuchResourceException();
            var prefab = ResourceCache.TryGetElement(uri) as T;
            if (prefab == null) {
                prefab = Resources.Load<T>(uri);
                ResourceCache[uri] = prefab;
            }
            return parent != null ? UnityEngine.Object.Instantiate(prefab, parent) : UnityEngine.Object.Instantiate(prefab);
        }
        /// <summary>
        /// 从Resource目录加载指定资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="uri">资源URI</param>
        /// <returns>资源对象</returns>
        public static T LoadResource<T>(string uri) where T : UnityEngine.Object {
            if (!TextUtils.HasData(uri)) throw new NoSuchResourceException();
            var res = ResourceCache.TryGetElement(uri) as T;
            if (res != null) return res;
            res = Resources.Load<T>(uri);
            ResourceCache[uri] = res;
            return res;
        }
        /// <summary>
        /// 找不到指定URI对应资源的异常
        /// </summary>
        public class NoSuchResourceException : ApplicationException {
            private string error;
            //无参数构造函数
            public NoSuchResourceException() {
                error = "There is no such target, please check the uri string!";
            }
            //带一个字符串参数的构造函数，作用：当程序员用Exception类获取异常信息而非NoSuchUriException时把自定义异常信息传递过去
            public NoSuchResourceException(string msg) : base(msg) {
                error = msg;
            }
            public string GetError() {
                return error;
            }
        }
    }
}
