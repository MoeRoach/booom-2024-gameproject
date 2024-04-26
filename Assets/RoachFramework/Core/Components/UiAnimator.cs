using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// UI动画控制器
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class UiAnimator : MonoBehaviour {

        public bool hasShowAnimation;
        public UiAnimationData showAnimation = new UiAnimationData();
        public event Action BeforeShow;
        public event Action OnShown;
        public bool hasHideAnimation;
        public UiAnimationData hideAnimation = new UiAnimationData();
        public event Action BeforeHide;
        public event Action OnHiden;
        
        public bool independentDeltaTime;

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        
        private Vector2 _showMoveFrom;
        private Vector2 _showMoveTo;
        private Vector2 _hideMoveFrom;
        private Vector2 _hideMoveTo;

        private Vector3 _showRotateFrom;
        private Vector3 _showRotateTo;
        private Vector3 _hideRotateFrom;
        private Vector3 _hideRotateTo;

        private Vector3 _showScaleFrom;
        private Vector3 _showScaleTo;
        private Vector3 _hideScaleFrom;
        private Vector3 _hideScaleTo;

        private float _showFadeFrom;
        private float _showFadeTo;
        private float _hideFadeFrom;
        private float _hideFadeTo;

        private float _showAnimTime;
        private float _hideAnimTime;

        private float DeltaTime =>
            independentDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;

        private bool HasShowAnimation => hasShowAnimation &&
                                         (showAnimation.hasMove || showAnimation.hasRotate ||
                                          showAnimation.hasScale || showAnimation.hasFade);

        private bool HasHideAnimation => hasHideAnimation &&
                                         (hideAnimation.hasMove || hideAnimation.hasRotate ||
                                          hideAnimation.hasScale || hideAnimation.hasFade);
        
        public bool IsVisible { get; private set; }
        public bool IsAnimate { get; private set; }

        private void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start() {
            InitializeParameters();
        }

        private void InitializeParameters() {
            IsVisible = true;
            IsAnimate = false;
            _showAnimTime = 0f;
            _showMoveFrom = showAnimation.moveAnimation.moveFrom;
            _showMoveTo = showAnimation.moveAnimation.moveTo;
            if (showAnimation.moveAnimation.relativeAnim) {
                var anchoredPosition = _rectTransform.anchoredPosition;
                _showMoveFrom += anchoredPosition;
                _showMoveTo += anchoredPosition;
            }

            showAnimation.moveAnimation.active = showAnimation.hasMove;
            if (showAnimation.moveAnimation.active) {
                var showMoveTime = showAnimation.moveAnimation.startDelay +
                                   showAnimation.moveAnimation.animDuration;
                _showAnimTime = _showAnimTime < showMoveTime ? showMoveTime : _showAnimTime;
            }

            _hideAnimTime = 0f;
            _hideMoveFrom = hideAnimation.moveAnimation.moveFrom;
            _hideMoveTo = hideAnimation.moveAnimation.moveTo;
            if (hideAnimation.moveAnimation.relativeAnim) {
                var anchoredPosition = _rectTransform.anchoredPosition;
                _hideMoveFrom += anchoredPosition;
                _hideMoveTo += anchoredPosition;
            }

            hideAnimation.moveAnimation.active = hideAnimation.hasMove;
            if (hideAnimation.moveAnimation.active) {
                var hideMoveTime = hideAnimation.moveAnimation.startDelay +
                                   hideAnimation.moveAnimation.animDuration;
                _hideAnimTime = _hideAnimTime < hideMoveTime ? hideMoveTime : _hideAnimTime;
            }

            _showRotateFrom = showAnimation.rotateAnimation.rotateFrom;
            _showRotateTo = showAnimation.rotateAnimation.rotateTo;
            if (showAnimation.rotateAnimation.relativeAnim) {
                var localEulerAngles = _rectTransform.localEulerAngles;
                _showRotateFrom += localEulerAngles;
                _showRotateTo += localEulerAngles;
            }

            showAnimation.rotateAnimation.active = showAnimation.hasRotate;
            if (showAnimation.rotateAnimation.active) {
                var showRotateTime = showAnimation.rotateAnimation.startDelay +
                                   showAnimation.rotateAnimation.animDuration;
                _showAnimTime = _showAnimTime < showRotateTime ? showRotateTime : _showAnimTime;
            }
            
            _hideRotateFrom = hideAnimation.rotateAnimation.rotateFrom;
            _hideRotateTo = hideAnimation.rotateAnimation.rotateTo;
            if (hideAnimation.rotateAnimation.relativeAnim) {
                var localEulerAngles = _rectTransform.localEulerAngles;
                _hideRotateFrom += localEulerAngles;
                _hideRotateTo += localEulerAngles;
            }

            hideAnimation.rotateAnimation.active = hideAnimation.hasRotate;
            if (hideAnimation.rotateAnimation.active) {
                var hideRotateTime = hideAnimation.rotateAnimation.startDelay +
                                     hideAnimation.rotateAnimation.animDuration;
                _hideAnimTime = _hideAnimTime < hideRotateTime ? hideRotateTime : _hideAnimTime;
            }

            _showScaleFrom = showAnimation.scaleAnimation.scaleFrom;
            _showScaleTo = showAnimation.scaleAnimation.scaleTo;
            if (showAnimation.scaleAnimation.relativeAnim) {
                var localScale = _rectTransform.localScale;
                _showScaleFrom += localScale;
                _showScaleTo += localScale;
            }

            showAnimation.scaleAnimation.active = showAnimation.hasScale;
            if (showAnimation.scaleAnimation.active) {
                var showScaleTime = showAnimation.scaleAnimation.startDelay +
                                     showAnimation.scaleAnimation.animDuration;
                _showAnimTime = _showAnimTime < showScaleTime ? showScaleTime : _showAnimTime;
            }
            
            _hideScaleFrom = hideAnimation.scaleAnimation.scaleFrom;
            _hideScaleTo = hideAnimation.scaleAnimation.scaleTo;
            if (hideAnimation.scaleAnimation.relativeAnim) {
                var localScale = _rectTransform.localScale;
                _hideScaleFrom += localScale;
                _hideScaleTo += localScale;
            }

            hideAnimation.scaleAnimation.active = hideAnimation.hasScale;
            if (hideAnimation.scaleAnimation.active) {
                var hideScaleTime = hideAnimation.scaleAnimation.startDelay +
                                     hideAnimation.scaleAnimation.animDuration;
                _hideAnimTime = _hideAnimTime < hideScaleTime ? hideScaleTime : _hideAnimTime;
            }

            _showFadeFrom = showAnimation.fadeAnimation.fadeFrom;
            _showFadeTo = showAnimation.fadeAnimation.fadeTo;
            if (showAnimation.fadeAnimation.relativeAnim) {
                var alpha = _canvasGroup.alpha;
                _showFadeFrom += alpha;
                _showFadeTo += alpha;
            }

            showAnimation.fadeAnimation.active = showAnimation.hasFade;
            if (showAnimation.fadeAnimation.active) {
                var showFadeTime = showAnimation.fadeAnimation.startDelay +
                                    showAnimation.fadeAnimation.animDuration;
                _showAnimTime = _showAnimTime < showFadeTime ? showFadeTime : _showAnimTime;
            }
            
            _hideFadeFrom = hideAnimation.fadeAnimation.fadeFrom;
            _hideFadeTo = hideAnimation.fadeAnimation.fadeTo;
            if (hideAnimation.fadeAnimation.relativeAnim) {
                var alpha = _canvasGroup.alpha;
                _hideFadeFrom += alpha;
                _hideFadeTo += alpha;
            }

            hideAnimation.fadeAnimation.active = hideAnimation.hasFade;
            if (hideAnimation.fadeAnimation.active) {
                var hideFadeTime = hideAnimation.fadeAnimation.startDelay +
                                    hideAnimation.fadeAnimation.animDuration;
                _hideAnimTime = _hideAnimTime < hideFadeTime ? hideFadeTime : _hideAnimTime;
            }
        }

        public void SetupVisible(bool visible) {
            if (IsAnimate) return;
            InitializeParameters();
            if (visible) {
                FinalizeShowAnimation();
            } else {
                FinalizeHideAnimation();
            }
            IsVisible = visible;
        }

        public void RequestShowAnimation() {
            if (IsVisible || IsAnimate) return;
            InitializeParameters();
            UpdateShowAnimation();
        }

        private async void UpdateShowAnimation() {
            IsAnimate = true;
            if (HasShowAnimation) {
                var timer = 0f;
                BeforeShow?.Invoke();
                while (timer < _showAnimTime) {
                    timer += DeltaTime;
                    var moveAnim = showAnimation.moveAnimation;
                    if (moveAnim.active) {
                        UpdateMoveAnimation(moveAnim.startDelay, moveAnim.animDuration, true,
                            moveAnim.animationEase, timer);
                    }

                    var rotateAnim = showAnimation.rotateAnimation;
                    if (rotateAnim.active) {
                        UpdateRotateAnimation(rotateAnim.startDelay, rotateAnim.animDuration,
                            true, rotateAnim.animationEase, timer);
                    } else {
                        _rectTransform.localEulerAngles = Vector3.zero;
                    }

                    var scaleAnim = showAnimation.scaleAnimation;
                    if (scaleAnim.active) {
                        UpdateScaleAnimation(scaleAnim.startDelay, scaleAnim.animDuration, true,
                            scaleAnim.animationEase, timer);
                    } else {
                        _rectTransform.localScale = Vector3.one;
                    }

                    var fadeAnim = showAnimation.fadeAnimation;
                    if (fadeAnim.active) {
                        UpdateFadeAnimation(fadeAnim.startDelay, fadeAnim.animDuration, true,
                            fadeAnim.animationEase, timer);
                    } else {
                        _canvasGroup.alpha = 1f;
                    }

                    await UniTask.Yield();
                }
            }
            FinalizeShowAnimation();
            IsVisible = true;
            IsAnimate = false;
        }

        private void FinalizeShowAnimation() {
            OnShown?.Invoke();
            if (!HasShowAnimation) return;
            if (showAnimation.hasMove) {
                _rectTransform.anchoredPosition = _showMoveTo;
            }

            if (showAnimation.hasRotate) {
                _rectTransform.localEulerAngles = _showRotateTo;
            }

            if (showAnimation.hasScale) {
                _rectTransform.localScale = _showScaleTo;
            }

            if (showAnimation.hasFade) {
                _canvasGroup.alpha = _showFadeTo;
            }
        }

        public void RequestHideAnimation() {
            if (!IsVisible || IsAnimate) return;
            InitializeParameters();
            UpdateHideAnimation();
        }

        private async void UpdateHideAnimation() {
            IsAnimate = true;
            if (HasHideAnimation) {
                var timer = 0f;
                BeforeHide?.Invoke();
                while (timer < _hideAnimTime) {
                    timer += DeltaTime;
                    var moveAnim = hideAnimation.moveAnimation;
                    if (moveAnim.active) {
                        UpdateMoveAnimation(moveAnim.startDelay, moveAnim.animDuration, false,
                            moveAnim.animationEase, timer);
                    }

                    var rotateAnim = hideAnimation.rotateAnimation;
                    if (rotateAnim.active) {
                        UpdateRotateAnimation(rotateAnim.startDelay, rotateAnim.animDuration,
                            false, rotateAnim.animationEase, timer);
                    } else {
                        _rectTransform.localEulerAngles = Vector3.zero;
                    }

                    var scaleAnim = hideAnimation.scaleAnimation;
                    if (scaleAnim.active) {
                        UpdateScaleAnimation(scaleAnim.startDelay, scaleAnim.animDuration, false,
                            scaleAnim.animationEase, timer);
                    } else {
                        _rectTransform.localScale = Vector3.one;
                    }

                    var fadeAnim = hideAnimation.fadeAnimation;
                    if (fadeAnim.active) {
                        UpdateFadeAnimation(fadeAnim.startDelay, fadeAnim.animDuration, false,
                            fadeAnim.animationEase, timer);
                    } else {
                        _canvasGroup.alpha = 1f;
                    }

                    await UniTask.Yield();
                }
            }
            FinalizeHideAnimation();
            IsVisible = false;
            IsAnimate = false;
        }

        private void FinalizeHideAnimation() {
            OnHiden?.Invoke();
            if (!HasHideAnimation) return;
            if (hideAnimation.hasMove) {
                _rectTransform.anchoredPosition = _hideMoveTo;
            }

            if (hideAnimation.hasRotate) {
                _rectTransform.localEulerAngles = _hideRotateTo;
            }

            if (hideAnimation.hasScale) {
                _rectTransform.localScale = _hideScaleTo;
            }

            if (hideAnimation.hasFade) {
                _canvasGroup.alpha = _hideFadeTo;
            }
        }

        private void UpdateMoveAnimation(float delay, float duration, bool isShow,
            AnimationEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime <= 0f) return;
            var startPos = isShow ? _showMoveFrom : _hideMoveFrom;
            var finishPos = isShow ? _showMoveTo : _hideMoveTo;
            if (animTime <= duration) {
                var amount = CalculateAmountByEase(animTime / duration, ease);
                _rectTransform.anchoredPosition =
                    Vector2.LerpUnclamped(startPos, finishPos, amount);
            } else {
                _rectTransform.anchoredPosition = finishPos;
            }
        }

        private void UpdateRotateAnimation(float delay, float duration, bool isShow,
            AnimationEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime <= 0f) return;
            var startAngle = isShow ? _showRotateFrom : _hideRotateFrom;
            var finishAngle = isShow ? _showRotateTo : _hideRotateTo;
            if (animTime <= duration) {
                var amount = CalculateAmountByEase(animTime / duration, ease);
                _rectTransform.localEulerAngles =
                    Vector3.LerpUnclamped(startAngle, finishAngle, amount);
            } else {
                _rectTransform.localEulerAngles = finishAngle;
            }
        }

        private void UpdateScaleAnimation(float delay, float duration, bool isShow,
            AnimationEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime <= 0f) return;
            var startScale = isShow ? _showScaleFrom : _hideScaleFrom;
            var finishScale = isShow ? _showScaleTo : _hideScaleTo;
            if (animTime <= duration) {
                var amount = CalculateAmountByEase(animTime / duration, ease);
                _rectTransform.localScale =
                    Vector3.LerpUnclamped(startScale, finishScale, amount);
            } else {
                _rectTransform.localScale = finishScale;
            }
        }

        private void UpdateFadeAnimation(float delay, float duration, bool isShow,
            AnimationEase ease, float timer) {
            var animTime = timer - delay;
            if (animTime <= 0f) return;
            var startFade = isShow ? _showFadeFrom : _hideFadeFrom;
            var finishFade = isShow ? _showFadeTo : _hideFadeTo;
            if (animTime <= duration) {
                var amount = CalculateAmountByEase(animTime / duration, ease);
                _canvasGroup.alpha = Mathf.Lerp(startFade, finishFade, amount);
            } else {
                _canvasGroup.alpha = finishFade;
            }
        }

        private float CalculateAmountByEase(float normalizeTime, AnimationEase animEase) {
            switch (animEase) {
                case AnimationEase.Sin:
                    return (Mathf.Sin((normalizeTime - 0.5f) * Mathf.PI) + 1f) / 2f;
                case AnimationEase.LinearBounceIn: // 开头Bounce
                    if (normalizeTime < 0.2f) {
                        return -normalizeTime;
                    } else {
                        return normalizeTime * 1.5f - 0.5f;
                    }
                case AnimationEase.LinearBounceOut:
                    if (normalizeTime < 0.8f) {
                        return normalizeTime * 1.5f;
                    } else {
                        return -normalizeTime + 2f;
                    }
                case AnimationEase.Linear:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animEase), animEase, null);
            }

            return normalizeTime;
        }
    }
    
    public enum AnimationEase {
        Linear,
        Sin,
        LinearBounceIn,
        LinearBounceOut
    }
    /// <summary>
    /// UI动画数据
    /// </summary>
    [Serializable]
    public class UiAnimationData {

        public bool hasMove;
        public MoveAnimation moveAnimation;
        public bool hasRotate;
        public RotateAnimation rotateAnimation;
        public bool hasScale;
        public ScaleAnimation scaleAnimation;
        public bool hasFade;
        public FadeAnimation fadeAnimation;

        public UiAnimationData() {
            hasMove = false;
            hasRotate = false;
            hasScale = false;
            hasFade = false;
            moveAnimation = new MoveAnimation();
            rotateAnimation = new RotateAnimation();
            scaleAnimation = new ScaleAnimation();
            fadeAnimation = new FadeAnimation();
        }
    }
    [Serializable]
    public abstract class BaseAnimationData {

        public bool active;
        public float startDelay;
        public float animDuration;
        public bool relativeAnim;
        public AnimationEase animationEase;
    }
    [Serializable]
    public class MoveAnimation : BaseAnimationData {

        public Vector2 moveFrom;
        public Vector2 moveTo;

        public MoveAnimation() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animationEase = AnimationEase.Linear;
            moveFrom = Vector2.zero;
            moveTo = Vector2.zero;
        }
    }
    [Serializable]
    public class RotateAnimation : BaseAnimationData {

        public Vector3 rotateFrom;
        public Vector3 rotateTo;

        public RotateAnimation() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animationEase = AnimationEase.Linear;
            rotateFrom = Vector3.zero;
            rotateTo = Vector3.zero;
        }
    }
    [Serializable]
    public class ScaleAnimation : BaseAnimationData {

        public Vector3 scaleFrom;
        public Vector3 scaleTo;

        public ScaleAnimation() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animationEase = AnimationEase.Linear;
            scaleFrom = Vector3.zero;
            scaleTo = Vector3.zero;
        }
    }
    [Serializable]
    public class FadeAnimation : BaseAnimationData {

        public float fadeFrom;
        public float fadeTo;

        public FadeAnimation() {
            startDelay = 0f;
            animDuration = 0.25f;
            relativeAnim = true;
            animationEase = AnimationEase.Linear;
            fadeFrom = 0f;
            fadeTo = 1f;
        }
    }
}
