using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 框架拓展的游戏对象脚本基类
    /// </summary>
    public abstract class BaseGameObject : MonoBehaviour, IBroadcastReceiver {
        
        protected BroadcastService broadcastService;

        protected readonly SortedDictionary<int, List<Action>> updateFunctionSet = new SortedDictionary<int, List<Action>>();
        protected readonly List<Action<GameObject>> destroyFunctionSet = new List<Action<GameObject>>();
        protected readonly Dictionary<string, CancellationTokenSource> asyncTaskCancelTokens =
            new Dictionary<string, CancellationTokenSource>();

        #region Lifetime Functions

        private void Awake() {
            broadcastService = ServiceProvider.Instance.ProvideService<BroadcastService>(
                BroadcastService.SERVICE_NAME);
            OnAwake();
        }
        
        protected virtual void OnAwake() { }

        private void Start() {
            var filter = new BroadcastFilter();
            SetupBroadcastFilter(filter);
            broadcastService.RegisterBroadcastReceiver(filter, this);
            NextFrameAction(OnLazyStart);
            OnStart();
        }
        
        protected virtual void SetupBroadcastFilter(BroadcastFilter filter) { }
        
        protected virtual void OnStart() { }
        
        protected virtual void OnLazyStart() { }

        private void Update() {
            OnUpdate();
            CallFunctions();
        }
        
        /// <summary>
        /// 调用所有注册的更新方法
        /// </summary>
        private void CallFunctions() {
            if (updateFunctionSet.Count <= 0) return;
            foreach (var pair in updateFunctionSet) {
                if (pair.Value == null || pair.Value.Count <= 0) continue;
                for (var i = 0; i < pair.Value.Count; i++) {
                    pair.Value[i].Invoke();
                }
            }
        }
        
        protected virtual void OnUpdate() { }

        private void LateUpdate() {
            OnLateUpdate();
        }
        
        protected virtual void OnLateUpdate() { }

        private void FixedUpdate() {
            OnFixedUpdate();
        }
        
        protected virtual void OnFixedUpdate() { }

        #endregion
        
        /// <summary>
        /// 注册帧更新方法到集合
        /// </summary>
        /// <param name="priority">方法优先级，数字越小越早运行</param>
        /// <param name="func">更新方法</param>
        protected void RegisterUpdateFunction(int priority, Action func) {
            var functions = updateFunctionSet.ContainsKey(priority) ? updateFunctionSet[priority] : new List<Action>();
            functions.Add(func);
            updateFunctionSet[priority] = functions;
        }

        /// <summary>
        /// 反注册帧更新方法
        /// </summary>
        /// <param name="func">更新方法</param>
        protected void UnregisterUpdateFunction(Action func) {
            var priorityList = new List<int>(updateFunctionSet.Keys);
            foreach (var p in priorityList) {
                updateFunctionSet[p].Remove(func);
            }
        }
        
        /// <summary>
        /// 注册一个对象销毁回调方法
        /// </summary>
        /// <param name="callback">方法委托</param>
        public void RegisterDestroyCallback(Action<GameObject> callback) {
            if (destroyFunctionSet.Contains(callback)) return;
            destroyFunctionSet.Add(callback);
        }

        /// <summary>
        /// 反注册一个对象销毁回调方法
        /// </summary>
        /// <param name="callback">方法委托</param>
        public void UnregisterDestroyCallback(Action<GameObject> callback) {
            destroyFunctionSet.Remove(callback);
        }

        #region Async Operations

        /// <summary>
        /// 帧末尾异步操作，通常可以看做下一帧
        /// </summary>
        /// <param name="act">执行方法</param>
        protected async void YieldAction(Action act) {
            await UniTask.Yield();
            act?.Invoke();
        }

        /// <summary>
        /// 下一帧异步操作
        /// </summary>
        /// <param name="act">执行方法</param>
        protected async void NextFrameAction(Action act) {
            await UniTask.NextFrame();
            act?.Invoke();
        }

        /// <summary>
        /// 延迟操作
        /// </summary>
        /// <param name="act">执行方法</param>
        /// <param name="time">延迟时间(s)</param>
        protected async void DelayAction(Action act, float time) {
            var ms = Mathf.RoundToInt(time * 1000f);
            await UniTask.Delay(ms);
            act?.Invoke();
        }

        /// <summary>
        /// 异步任务
        /// </summary>
        /// <param name="tn">任务名称</param>
        /// <param name="factory">任务构造工厂方法</param>
        protected async void AsyncTask(string tn, Func<CancellationTokenSource, UniTask> factory) {
            if (asyncTaskCancelTokens.ContainsKey(tn)) {
                Debug.LogWarning($"Another task with the same name: {tn} is executing.");
                asyncTaskCancelTokens[tn].Cancel();
                asyncTaskCancelTokens.Remove(tn);
            }

            try {
                var cts = new CancellationTokenSource();
                asyncTaskCancelTokens[tn] = cts;
                await factory.Invoke(cts);
            } catch (OperationCanceledException e) {
                Debug.LogWarning($"Task with name: {tn} has been canceled.");
                Console.WriteLine(e.Message);
                OnTaskCanceled(tn);
            } finally {
                asyncTaskCancelTokens.Remove(tn);
            }
        }
        
        /// <summary>
        /// 任务取消回调
        /// </summary>
        /// <param name="tn">任务名称</param>
        protected virtual void OnTaskCanceled(string tn) { }

        /// <summary>
        /// 通过名称获取任务取消Token
        /// </summary>
        /// <param name="tn">任务名称</param>
        /// <returns>取消Token</returns>
        protected CancellationTokenSource GetTaskToken(string tn) {
            return asyncTaskCancelTokens[tn];
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="tn">任务名称</param>
        protected void CancelTask(string tn) {
            if (!asyncTaskCancelTokens.ContainsKey(tn)) return;
            asyncTaskCancelTokens[tn].Cancel();
        }

        #endregion

        public virtual void ReceiveBroadcast(BroadcastInfo info) { }
        
        /// <summary>
        /// 通过完整路径寻找子物体组件的快捷方法
        /// 注意路径即可以使用/分隔符，也可以使用.分隔符
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="path">完整路径</param>
        /// <returns>目标组件</returns>
        protected T FindComponent<T>(string path) where T : Component {
            path = path.Replace('.', '/');
            return gameObject.FindComponent<T>(path);
        }

        /// <summary>
        /// 通过完整路径寻找子物体的快捷方法
        /// 注意路径即可以使用/分隔符，也可以使用.分隔符
        /// </summary>
        /// <param name="path">完整路径</param>
        /// <returns>目标物体对象</returns>
        protected GameObject FindGameObject(string path) {
            path = path.Replace('.', '/');
            return gameObject.FindObject(path);
        }

        /// <summary>
        /// 设置目标对象的激活状态
        /// </summary>
        /// <param name="active">是否激活</param>
        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// 摧毁对象时释放资源
        /// </summary>
        protected virtual void Release() { }

        /// <summary>
        /// 取消所有记录的异步操作
        /// </summary>
        protected virtual void CancelAllTasks() {
            foreach (var key in asyncTaskCancelTokens.Keys) {
                asyncTaskCancelTokens[key]?.Cancel();
            }
            
            asyncTaskCancelTokens.Clear();
        }
        
        private void OnDestroy() {
            broadcastService.UnregisterBroadcastReceiver(this);
            destroyFunctionSet.EnumerateAction(act => act.Invoke(gameObject));
            CancelAllTasks();
            Release();
        }
    }
}
