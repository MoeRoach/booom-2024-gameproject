using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 通用脚本泛型单例基类
    /// </summary>
    /// <typeparam name="T">单例类型</typeparam>
    public abstract class MonoSingleton<T> : BaseGameObject where T : MonoSingleton<T> {
        public static T Instance { get; protected set; }
        public bool canBeDestroy;

        protected override void OnAwake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this as T;
            if (!canBeDestroy) {
                DontDestroyOnLoad(gameObject);
            }
            
            base.OnAwake();
        }

        protected override void Release() {
            base.Release();
            if (Instance == this) {
                Instance = null;
            }
        }
    }
}
