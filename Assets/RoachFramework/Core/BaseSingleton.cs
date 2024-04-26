using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 通用泛型单例抽象基类
    /// </summary>
    /// <typeparam name="T">单例类型</typeparam>
    public abstract class BaseSingleton<T> where T : BaseSingleton<T> {

        private static T instance;

        public static T Instance {
            get {
                instance ??= (T) Activator.CreateInstance(typeof(T), true);
                instance.Initialize();
                return instance;
            }
        }

        protected bool isInitialized;

        private void Initialize() {
            if (isInitialized) return;
            OnInitialized();
            isInitialized = true;
        }

        protected abstract void OnInitialized();

    }
}
