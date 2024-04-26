using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 后台异步线程执行器，通用参数
    /// </summary>
    public class AsyncExecutor : MonoBehaviour {
        
        private const int AsyncCallbackToMainThreadPerFrame = 8;
        public delegate OutputParam ExecuteFunc(InputParam param);
        public delegate void CallbackFunc(OutputParam param);

        /// <summary>
        /// 安排异步工作到子线程
        /// </summary>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <param name="callback"></param>
        public static void ScheduleAsyncTask(ExecuteFunc func, InputParam args,
            CallbackFunc callback = null) {
            ThreadPool.QueueUserWorkItem(state => {
                var result = func.Invoke(args);
                if (callback == null) return;
                var method = new CallbackMethod(callback, result);
                Instance.CallbackToMainThread(method);
            });
        }

        public static AsyncExecutor Instance { get; private set; }

        private ConcurrentQueue<CallbackMethod> _callbackQueue;

        private void Awake() {
            _callbackQueue = new ConcurrentQueue<CallbackMethod>();
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        /// <summary>
        /// 将回调加入主线程队列
        /// </summary>
        /// <param name="method"></param>
        public void CallbackToMainThread(CallbackMethod method) {
            _callbackQueue.Enqueue(method);
        }

        private void Update() {
            if (_callbackQueue == null) return;
            // 每帧执行固定数量的回调
            for (var i = 0; i < AsyncCallbackToMainThreadPerFrame; i++) {
                if (_callbackQueue.Count > 0) {
                    if (_callbackQueue.TryDequeue(out var method)) {
                        method.callbackFunc?.Invoke(method.callbackParam);
                    } else {
                        break;
                    }
                } else {
                    break;
                }
            }
        }

        /// <summary>
        /// 子线程输入参数
        /// </summary>
        public class InputParam {

            public object[] args;

            public InputParam(int count) {
                args = new object[count];
            }
        }

        /// <summary>
        /// 子线程输出结果
        /// </summary>
        public class OutputParam {
            public object result;
        }

        /// <summary>
        /// 回调方法结构
        /// </summary>
        public class CallbackMethod {

            public readonly CallbackFunc callbackFunc;
            public readonly OutputParam callbackParam;

            public CallbackMethod(CallbackFunc func, OutputParam param) {
                callbackFunc = func;
                callbackParam = param;
            }
        }
    }
}
