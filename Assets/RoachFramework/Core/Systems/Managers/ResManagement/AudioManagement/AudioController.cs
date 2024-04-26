using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RoachFramework {
    /// <summary>
    /// 音频播放控制器
    /// </summary>
    public class AudioController : MonoBehaviour {

        #region Static Properties
        public const string NAME_AUDIO_CONTROLLER = "AudioController";
        public const float IDLE_TIMEOUT_LENGTH = 30f;
        private static List<AudioController> controllers = new List<AudioController>();
        #endregion

        public AudioSource source { get; private set; }

        private Transform _audioTransform;
        private Transform _followTarget;

        public bool inUse { get; private set; }
        public float playProgress { get; private set; }
        public bool isPause { get; private set; }
        public bool isMute { get; private set; }
        public float lastPlayTime { get; private set; }
        public bool isPlaying { get; private set; }
        public bool autoPause { get; private set; }
        public bool muted { get; private set; }
        public bool paused { get; private set; }
        public float idleTime => Time.realtimeSinceStartup - lastPlayTime;

        private void Reset() {
            ResetController();
        }

        private void Awake() {
            controllers.Add(this);
            _audioTransform = transform;
            source = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            ResetController();
        }

        private void Update() {
            if (isMute || isPause || source.isPlaying) {
                lastPlayTime = Time.realtimeSinceStartup;
            }
            if (isMute != muted) {
                source.mute = isMute;
                muted = isMute;
            }
            if (isPause != paused) {
                if (isPause) {
                    if (source.isPlaying) {
                        source.Pause();
                    }
                } else {
                    source.UnPause();
                }
                paused = isPause;
            }
            UpdatePlayProgress();
            if (playProgress >= 1f) {
                Stop();
                playProgress = 0f;
            } else {
                autoPause = inUse && isPlaying && !source.isPlaying && playProgress > 0f;
                if (inUse && !autoPause && !source.isPlaying && !isPause && isMute) {
                    Stop();
                } else {
                    FollowTarget();
                }
            }
            if (idleTime > IDLE_TIMEOUT_LENGTH) {
                Kill();
            }
        }

        private void OnDestroy() {
            controllers.Remove(this);
        }

        public void Kill() {
            source.Stop();
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Kill AudioController[{name}]");
            }
            Destroy(gameObject);
        }

        public void Mute() {
            isMute = true;
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Mute AudioController[{name}]");
            }
        }

        public void Pause() {
            isPause = true;
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Pause AudioController[{name}]");
            }
        }

        public void Play() {
            inUse = true;
            isPause = false;
            isPlaying = true;
            source.Play();
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Play AudioController[{name}]");
            }
        }

        public void SetupFollowTarget(Transform target) {
            _followTarget = target;
        }

        public void SetupAudioMixerGroup(AudioMixerGroup group) {
            if (group != null) {
                source.outputAudioMixerGroup = group;
            }
        }

        public void SetPosition(Vector3 pos) {
            _audioTransform.position = pos;
        }

        public void SetSourceProperties(AudioClip clip, float volume, float pitch, bool loop, float spatialBlend) {
            if (clip != null) {
                source.clip = clip;
                source.volume = volume;
                source.pitch = pitch;
                source.loop = loop;
                source.spatialBlend = spatialBlend;
            } else {
                Stop();
            }
        }

        public void Stop() {
            Unpause();
            Unmute();
            source.Stop();
            isPlaying = false;
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Stop AudioController[{name}]");
            }
            AudioPool.Instance.PutController(this);
            ResetController();
        }

        public void Unmute() {
            isMute = false;
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Unmute AudioController[{name}]");
            }
        }

        public void Unpause() {
            isPause = false;
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"Unpause AudioController[{name}]");
            }
        }

        private void FollowTarget() {
            if (_followTarget == null) return;
            _audioTransform.position = _followTarget.position;
            if (AudioManager.Instance.debugFlag) {
                Debug.Log($"{name} is following the GameObject[{_followTarget.name}]");
            }
        }

        private void UpdatePlayProgress() {
            if (source != null && source.clip != null) {
                playProgress = Mathf.Clamp01(source.time / source.clip.length);
            }
        }

        private void ResetController() {
            inUse = false;
            isPause = false;
            _followTarget = null;
            lastPlayTime = Time.realtimeSinceStartup;
        }

        public static AudioController GetController() {
            var controller = new GameObject(NAME_AUDIO_CONTROLLER, typeof(AudioSource), typeof(AudioController)).GetComponent<AudioController>();
            return controller;
        }

        public static void KillAll() {
            if (AudioManager.Instance.debugFlag) {
                Debug.Log("Kill All AudioControllers");
            }
            RemoveNullsFromList();
            foreach (var ac in controllers) {
                ac.Kill();
            }
        }

        public static void MuteAll() {
            if (AudioManager.Instance.debugFlag) {
                Debug.Log("Mute All AudioControllers");
            }
            RemoveNullsFromList();
            foreach (var ac in controllers) {
                ac.Mute();
            }
        }

        public static void PauseAll() {
            if (AudioManager.Instance.debugFlag) {
                Debug.Log("Pause All AudioControllers");
            }
            RemoveNullsFromList();
            foreach (var ac in controllers) {
                ac.Pause();
            }
        }

        public static void StopAll() {
            if (AudioManager.Instance.debugFlag) {
                Debug.Log("Stop All AudioControllers");
            }
            RemoveNullsFromList();
            foreach (var ac in controllers) {
                ac.Stop();
            }
        }

        public static void UnmuteAll() {
            if (AudioManager.Instance.debugFlag) {
                Debug.Log("Unmute All AudioControllers");
            }
            RemoveNullsFromList();
            foreach (var ac in controllers) {
                ac.Unmute();
            }
        }

        public static void UnpauseAll() {
            if (AudioManager.Instance.debugFlag) {
                Debug.Log("Unpause All AudioControllers");
            }
            RemoveNullsFromList();
            foreach (var ac in controllers) {
                ac.Unpause();
            }
        }

        public static void RemoveNullsFromList() {
            IEnumerable<AudioController> tempSet = controllers;
            controllers = new List<AudioController>();
            foreach (var ctrl in tempSet) {
                if (ctrl != null) {
                    controllers.Add(ctrl);
                }
            }
        }
    }
}
