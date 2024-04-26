using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 场景管理器
    /// </summary>
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class SceneLoader : MonoSingleton<SceneLoader> {

        #region Overall Configs
        public bool debugFlag;
        public float fadeTime = 1f;
        public bool isStartFade;
        public bool isManulContinue;
        #endregion

        protected Image maskImage;
        protected CanvasGroup loaderMask;
        protected Stack<SceneLoaderInfo> loaderStack;
        protected AsyncOperation loaderOperation;
        
        public event Action OnMaskShowDone;
        public event Action OnMaskHideDone;
        protected Action<string> onSceneLoaded;
        protected bool sceneContinue = false;

        protected override void OnAwake() {
            base.OnAwake();
            maskImage = GetComponent<Image>();
            loaderMask = GetComponent<CanvasGroup>();
            loaderStack = new Stack<SceneLoaderInfo>();
        }

        protected override void SetupBroadcastFilter(BroadcastFilter filter) {
            filter.AddFilter(CommonConfigs.BROADCAST_FILTER_SCENE_LOAD);
        }

        protected override void OnStart() {
            base.OnStart();
            if (isStartFade) {
                loaderMask.alpha = 1f;
                StartCoroutine(UpdateMaskFade(false, fadeTime));
            } else {
                loaderMask.alpha = 0f;
                loaderMask.interactable = false;
                maskImage.raycastTarget = false;
            }
        }

        protected IEnumerator UpdateSceneSwitch(SceneLoaderInfo info) {
            if (info.isMaskFade) {
                yield return UpdateMaskFade(true, fadeTime);
            } else {
                loaderMask.interactable = true;
                maskImage.raycastTarget = true;
            }

            yield return ExecuteSceneLoad(info);
        }

        protected IEnumerator UpdateMaskFade(bool toShow, float time) {
            var srcAlpha = loaderMask.alpha;
            var dstAlpha = toShow ? 1f : 0f;
            var timer = 0f;
            loaderMask.interactable = true;
            maskImage.raycastTarget = true;
            while (timer < time) {
                timer += Time.deltaTime;
                loaderMask.alpha = Mathf.Lerp(srcAlpha, dstAlpha, timer / time);
                yield return null;
            }

            loaderMask.interactable = toShow;
            maskImage.raycastTarget = toShow;
            if (toShow) {
                // 遮罩显示的情况
                OnMaskShowDone?.Invoke();
                OnMaskShowDone = null;
            } else {
                // 遮罩隐藏的情况
                OnMaskHideDone?.Invoke();
                OnMaskHideDone = null;
            }
        }

        protected IEnumerator ExecuteSceneLoad(SceneLoaderInfo info) {
            SceneLoaderInfo topScene;
            switch (info.loaderBehavior) {
                case LoaderBehavior.SwitchScene:
                    if (loaderStack.Count > 0) {
                        topScene = loaderStack.Pop();
                        if (topScene.loaderBehavior == LoaderBehavior.AppendScene) {
                            loaderOperation = SceneManager.UnloadSceneAsync(topScene.sceneName);
                            yield return loaderOperation;
                        }
                    }
                    loaderOperation = SceneManager.LoadSceneAsync(info.sceneName);
                    loaderOperation.allowSceneActivation = false;
                    loaderStack.Push(info);
                    yield return WaitForScene(loaderOperation);

                    onSceneLoaded?.Invoke(info.sceneName);
                    yield return WaitForContinue();

                    loaderOperation.allowSceneActivation = true;
                    if (info.isMaskFade) {
                        yield return UpdateMaskFade(false, fadeTime);
                    } else {
                        loaderMask.interactable = false;
                        maskImage.raycastTarget = false;
                    }
                    break;
                case LoaderBehavior.AppendScene:
                    loaderOperation = SceneManager.LoadSceneAsync(info.sceneName, LoadSceneMode.Additive);
                    loaderOperation.allowSceneActivation = false;
                    loaderStack.Push(info);
                    yield return WaitForScene(loaderOperation);

                    onSceneLoaded?.Invoke(info.sceneName);
                    yield return WaitForContinue();

                    loaderOperation.allowSceneActivation = true;
                    if (info.isMaskFade) {
                        yield return UpdateMaskFade(false, fadeTime);
                    } else {
                        loaderMask.interactable = false;
                        maskImage.raycastTarget = false;
                    }
                    break;
                case LoaderBehavior.RemoveScene:
                    topScene = loaderStack.Pop();
                    loaderOperation = SceneManager.UnloadSceneAsync(topScene.sceneName);
                    yield return loaderOperation;
                    break;
                case LoaderBehavior.NoScene:
                    // 仅播放淡出动画以及回调
                    yield return UpdateMaskFade(true, fadeTime);
                    onSceneLoaded?.Invoke(string.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator WaitForContinue() {
            if (!isManulContinue) yield break;
            while (!sceneContinue) {
                yield return null;
            }
        }

        private IEnumerator WaitForScene(AsyncOperation loader) {
            while (loader.progress < 0.9f) {
                yield return null;
            }
        }

        /// <summary>
        /// 继续场景加载进程
        /// </summary>
        public void SceneContinue() {
            sceneContinue = true;
        }

        /// <summary>
        /// 切换当前栈顶场景，自动卸载前一个
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="maskFade">是否淡出淡入遮罩</param>
        /// <param name="callback">加载完成后回调</param>
        public void SwitchScene(string sceneName, bool maskFade = true,
            Action<string> callback = null) {
            var info = new SceneLoaderInfo(sceneName,
                LoaderBehavior.SwitchScene, maskFade);
            onSceneLoaded = callback;
            StartCoroutine(UpdateSceneSwitch(info));
        }

        /// <summary>
        /// 添加场景到栈顶，前一个维持
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="maskFade">是否淡出淡入遮罩</param>
        /// <param name="callback">加载完成后回调</param>
        public void AppendScene(string sceneName, bool maskFade = true,
            Action<string> callback = null) {
            var info = new SceneLoaderInfo(sceneName,
                LoaderBehavior.AppendScene, maskFade);
            onSceneLoaded = callback;
            StartCoroutine(UpdateSceneSwitch(info));
        }

        /// <summary>
        /// 删除当前栈顶场景
        /// </summary>
        public void RemoveScene() {
            var info = loaderStack.Peek();
            info.loaderBehavior = LoaderBehavior.RemoveScene;
            StartCoroutine(UpdateSceneSwitch(info));
        }

        /// <summary>
        /// 请求一次渐出动画
        /// </summary>
        /// <param name="callback">渐出回调</param>
        public void RequestFade(Action<string> callback = null) {
            var info = new SceneLoaderInfo(String.Empty, LoaderBehavior
                .NoScene, true);
            onSceneLoaded = callback;
            StartCoroutine(UpdateSceneSwitch(info));
        }

        public override void ReceiveBroadcast(BroadcastInfo msg) {
            if (msg.Action == CommonConfigs.BROADCAST_FILTER_SCENE_LOAD) {
                var sceneName = msg.GetStringExtra(CommonConfigs.EXTRA_TAG_SCENE_NAME);
                switch (msg.Content) {
                    case CommonConfigs.BROADCAST_CONTENT_SWITCH_SCENE:
                        SwitchScene(sceneName);
                        break;
                    case CommonConfigs.BROADCAST_CONTENT_APPEND_SCENE:
                        AppendScene(sceneName);
                        break;
                    case CommonConfigs.BROADCAST_CONTENT_REMOVE_SCENE:
                        RemoveScene();
                        break;
                }
            }
        }
    }

    public enum LoaderBehavior {
        SwitchScene,
        AppendScene,
        RemoveScene,
        NoScene
    }

    public class SceneLoaderInfo {

        public string sceneName;
        public LoaderBehavior loaderBehavior;
        public bool isMaskFade;

        public SceneLoaderInfo(string name, LoaderBehavior behavior,
            bool maskFade) {
            sceneName = name;
            loaderBehavior = behavior;
            isMaskFade = maskFade;
        }
    }
}
