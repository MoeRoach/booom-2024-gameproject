using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 音频组件缓冲池
    /// </summary>
    public class AudioPool : MonoSingleton<AudioPool> {

        public bool autoKillIdleControllers = true;
        public float controllerIdleKillDuration = 5f;
        public float idleCheckInterval = 1f;
        public int minimumNumberOfControllers = 3;

        private List<AudioController> _audioPool;
        private Coroutine _idleCheckCoroutine;
        private WaitForSecondsRealtime _idleCheckIntervalWaitTime;
        private AudioController _tempController;

        protected override void OnAwake() {
            base.OnAwake();
            _audioPool = new List<AudioController>();
            if (autoKillIdleControllers) {
                StartIdleCheckInterval();
            }
        }

        private void OnDisable() {
            StopIdleCheckInterval();
        }

        public void ClearPool(bool keepMinCount = false) {
            if (keepMinCount) {
                RemoveNullsFromThePool();
                if (_audioPool.Count <= minimumNumberOfControllers) {
                    if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                        Debug.Log($"Clear Pool, {_audioPool.Count} Controllers Available");
                    }
                } else {
                    var killCount = 0;
                    for (var i = _audioPool.Count - 1; i >= minimumNumberOfControllers; i--) {
                        var ctrl = _audioPool[i];
                        _audioPool.Remove(ctrl);
                        ctrl.Kill();
                        killCount++;
                    }
                    if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                        Debug.Log($"Clear Pool, Killed {killCount} Controllers, {_audioPool.Count} Controllers Available");
                    }
                }
            } else {
                AudioController.KillAll();
                _audioPool.Clear();
                if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                    Debug.Log($"Clear Pool, Killed All Controllers, {_audioPool.Count} Controllers Available");
                }
            }
        }

        public AudioController GetController() {
            RemoveNullsFromThePool();
            AudioController ctrl;
            if (_audioPool.Count > 0) {
                ctrl = _audioPool[0];
                _audioPool.Remove(ctrl);
                ctrl.gameObject.SetActive(true);
            } else {
                ctrl = AudioController.GetController();
                ctrl.transform.SetParent(transform);
            }
            return ctrl;
        }

        public void PopulatePool(int count) {
            RemoveNullsFromThePool();
            if (count <= 0) return;
            for (var i = 0; i < count; i++) {
                PutController(AudioController.GetController());
            }
            if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                Debug.Log($"Populate Pool, Add {count} Controllers to the Pool, {_audioPool.Count} Controllers Available");
            }
        }

        public void PutController(AudioController ctrl) {
            if (ctrl == null) return;
            ctrl.gameObject.SetActive(false);
            ctrl.transform.SetParent(transform);
            if (!_audioPool.Contains(ctrl)) {
                _audioPool.Add(ctrl);
            }
            if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                Debug.Log($"Put AudioController[{ctrl.name}] in the Pool, {_audioPool.Count} Controllers Available");
            }
        }

        private void RemoveNullsFromThePool() {
            IEnumerable<AudioController> tempSet = _audioPool;
            _audioPool = new List<AudioController>();
            foreach (var ctrl in tempSet) {
                if (ctrl != null) {
                    _audioPool.Add(ctrl);
                }
            }
        }

        // -- Private Methods

        private void StartIdleCheckInterval() {
            if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                Debug.Log("Start Idle Check");
            }
            _idleCheckIntervalWaitTime = new WaitForSecondsRealtime(idleCheckInterval < 0f ? 0f : idleCheckInterval);
            _idleCheckCoroutine = StartCoroutine(KillIdleControllers());
        }

        private void StopIdleCheckInterval() {
            if (AudioManager.Instance != null && AudioManager.Instance.debugFlag) {
                Debug.Log("Stop Idle Check");
            }

            if (_idleCheckCoroutine == null) return;
            StopCoroutine(_idleCheckCoroutine);
            _idleCheckCoroutine = null;
        }

        // -- Coroutine
        private IEnumerator KillIdleControllers() {
            while (autoKillIdleControllers) {
                yield return _idleCheckIntervalWaitTime;
                RemoveNullsFromThePool();
                var minControllerCount = minimumNumberOfControllers > 0 ? minimumNumberOfControllers : 0;
                var controllerKillDuration = controllerIdleKillDuration > 0f ? controllerIdleKillDuration : 0f;
                if (_audioPool.Count <= minControllerCount) continue;
                for (var i = _audioPool.Count - 1; i >= minControllerCount; i--) {
                    _tempController = _audioPool[i];
                    if (!(_tempController.idleTime >= controllerKillDuration)) continue;
                    _audioPool.Remove(_tempController);
                    _tempController.Kill();
                }
            }
            _idleCheckCoroutine = null;
        }
    }
}
