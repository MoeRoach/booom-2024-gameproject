using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RoachFramework {
    /// <summary>
    /// 音频管理器主要工作类，同时提供播放接口
    /// </summary>
    [RequireComponent(typeof(AudioPool))]
    public class AudioManager : MonoSingleton<AudioManager> {

        public bool debugFlag = false;

        #region Static Properties
        private static AudioPool audioPool;
        public static AudioPool Pool {
            get {
                if (audioPool != null) {
                    return audioPool;
                }

                audioPool = Instance.gameObject.GetComponent<AudioPool>();
                if (audioPool == null) {
                    audioPool = Instance.gameObject.AddComponent<AudioPool>();
                }
                return audioPool;
            }
        }

        #endregion

        #region Private Fields
        private bool _isAudioDataReady = false;
        private string _uriAudioDataRoot;
        private AudioDataRootObject _rootObject;
        #endregion

        protected override void OnAwake() {
            base.OnAwake();
            _uriAudioDataRoot = AudioConfigs.URI_AUDIO_DATA_ROOT;
        }

        private void Start() {
            StartCoroutine(UpdateAudioData());
        }

        public AudioController Play(string dbName, string aName, Vector3 pos) {
            if (_isAudioDataReady && !string.IsNullOrEmpty(dbName) &&
                !string.IsNullOrEmpty(aName)) {
                var db = _rootObject.GetAudioDatabase(dbName);
                if (db != null) {
                    var grp = db.GetAudioGroupData(aName);
                    if (grp != null) {
                        var data = grp.GetNextAudioData();
                        var ctrl = Pool.GetController();
                        var mixer = db.databaseMixer;
                        var clip = data?.audioClip;
                        ctrl.SetupAudioMixerGroup(mixer);
                        if (clip != null) {
                            ctrl.SetPosition(pos);
                            ctrl.SetSourceProperties(clip, grp.groupVolume, grp.groupPitch,
                                grp.groupLoop, grp.groupSpatialBlend);
                            ctrl.Play();
                            if (Instance.debugFlag) {
                                Debug.Log($"Play [{dbName}].[{aName}] Audio Group at {pos}");
                            }

                            return ctrl;
                        }

                        if (Instance.debugFlag) {
                            Debug.LogWarning($"Cannot Find Audio Clip of [{dbName}.{aName}]");
                        }
                    } else {
                        if (Instance.debugFlag) {
                            Debug.LogWarning($"Cannot Find Audio Group [{dbName}.{aName}]");
                        }
                    }
                } else {
                    if (Instance.debugFlag) {
                        Debug.LogWarning($"Cannot Find Audio Database [{dbName}]");
                    }
                }
            } else {
                if (Instance.debugFlag) {
                    Debug.LogWarning("Something wrong with the Audio Data Load!");
                }
            }

            return null;
        }

        public AudioController Play(string dbName, string aName, Transform target) {
            if (_isAudioDataReady && !string.IsNullOrEmpty(dbName) &&
                !string.IsNullOrEmpty(aName)) {
                var db = _rootObject.GetAudioDatabase(dbName);
                if (db != null) {
                    var grp = db.GetAudioGroupData(aName);
                    if (grp != null) {
                        var data = grp.GetNextAudioData();
                        var ctrl = Pool.GetController();
                        var mixer = db.databaseMixer;
                        var clip = data?.audioClip;
                        ctrl.SetupAudioMixerGroup(mixer);
                        if (clip != null) {
                            ctrl.SetPosition(target.position);
                            ctrl.SetupFollowTarget(target);
                            ctrl.SetSourceProperties(clip, grp.groupVolume, grp.groupPitch,
                                grp.groupLoop, grp.groupSpatialBlend);
                            ctrl.Play();
                            if (Instance.debugFlag) {
                                Debug.Log(
                                    $"Play [{dbName}].[{aName}] Audio Group following target [{target.name}]");
                            }

                            return ctrl;
                        }

                        if (Instance.debugFlag) {
                            Debug.LogWarning($"Cannot Find Audio Clip of [{dbName}.{aName}]");
                        }
                    } else {
                        if (Instance.debugFlag) {
                            Debug.LogWarning($"Cannot Find Audio Group [{dbName}.{aName}]");
                        }
                    }
                } else {
                    if (Instance.debugFlag) {
                        Debug.LogWarning($"Cannot Find Audio Database [{dbName}]");
                    }
                }
            } else {
                if (Instance.debugFlag) {
                    Debug.LogWarning("Something wrong with the Audio Data Load!");
                }
            }

            return null;
        }

        public AudioController Play(AudioClip clip, Vector3 pos, float volume = 1f, float pitch = 1f, bool loop = false, float spatialBlend = 1f, AudioMixerGroup mixer = null) {
            if (clip != null) {
                if (Instance.debugFlag) {
                    Debug.Log($"Play Clip[{clip.name}] Audio Group at {pos}");
                }
                var ctrl = Pool.GetController();
                ctrl.SetSourceProperties(clip, volume, pitch, loop, spatialBlend);
                ctrl.SetupAudioMixerGroup(mixer);
                ctrl.SetPosition(pos);
                ctrl.Play();
                return ctrl;
            }

            if (Instance.debugFlag) {
                Debug.Log($"No Audio Clip to Play!");
            }
            return null;
        }

        public AudioController Play(AudioClip clip, Transform target, float volume = 1f, float pitch = 1f, bool loop = false, float spatialBlend = 1f, AudioMixerGroup mixer = null) {
            if (clip != null) {
                if (Instance.debugFlag) {
                    Debug.Log($"Play Clip[{clip.name}] Audio Group following target [{target.name}]");
                }
                var ctrl = Pool.GetController();
                ctrl.SetSourceProperties(clip, volume, pitch, loop, spatialBlend);
                ctrl.SetupAudioMixerGroup(mixer);
                ctrl.SetPosition(target.position);
                ctrl.SetupFollowTarget(target);
                ctrl.Play();
                return ctrl;
            }
            
            if (Instance.debugFlag) {
                Debug.Log($"No Audio Clip to Play!");
            }
            return null;
        }

        public void LoadAudioRootData(string uri) {
            _uriAudioDataRoot = uri;
            StartCoroutine(UpdateAudioData());
        }

        public void SetupAudioRootData(AudioDataRootObject root) {
            _rootObject = root;
            _isAudioDataReady = _rootObject != null;
        }

        public void SetupDatabaseVolumn(string dbname, string vname, float percent) {
            if (!_isAudioDataReady) return;
            var db = _rootObject.GetAudioDatabase(dbname);
            if (db == null || db.databaseMixer == null) return;
            var dbvalue = AudioUtils.LinearToDecibel(percent);
            db.databaseMixer.audioMixer.SetFloat(vname, dbvalue);
        }

        protected IEnumerator UpdateAudioData() {
            var request = Resources.LoadAsync<AudioDataRootObject>(_uriAudioDataRoot);
            yield return request;
            _rootObject = request.asset as AudioDataRootObject;
            _isAudioDataReady = _rootObject != null;
        }
    }
}
