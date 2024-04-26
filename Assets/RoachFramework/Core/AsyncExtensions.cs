using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 异步拓展方法
    /// </summary>
    public static class AsyncExtension {
        /// <summary>
        /// 物体位移的异步方法，Transform组件拓展
        /// </summary>
        /// <param name="trans">作用物体</param>
        /// <param name="target">目标位置</param>
        /// <param name="speed">位移速度</param>
        /// <returns></returns>
        public static async UniTask UpdateTranslate(this Transform trans, Vector3 target, float speed) {
            var start = trans.position;
            var timer = 0f;
            while (timer < 1f) {
                timer += Time.deltaTime * speed;
                trans.position = Vector3.Lerp(start, target, timer);
                await UniTask.Yield();
            }
        }
    }
}
