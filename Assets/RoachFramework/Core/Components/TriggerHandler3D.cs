using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 通用3D触发处理区
    /// </summary>
    public class TriggerHandler3D : MonoBehaviour {

        protected Dictionary<string, HashSet<Action<GameObject>>> enterTriggerHandlers;
        protected Dictionary<string, HashSet<Action<GameObject>>> stayTriggerHandlers;
        protected Dictionary<string, HashSet<Action<GameObject>>> exitTriggerHandlers;
        protected HashSet<GameObject> objectInArea;
        public bool IsAreaEmpty => objectInArea == null || objectInArea.Count <= 0;
        
        private void Awake() {
            enterTriggerHandlers = new Dictionary<string, HashSet<Action<GameObject>>>();
            stayTriggerHandlers = new Dictionary<string, HashSet<Action<GameObject>>>();
            exitTriggerHandlers = new Dictionary<string, HashSet<Action<GameObject>>>();
            objectInArea = new HashSet<GameObject>();
        }

        public void RegisterEnterTriggerHandler(Action<GameObject> callback, string t = "") {
            if (!enterTriggerHandlers.ContainsKey(t)) {
                enterTriggerHandlers[t] = new HashSet<Action<GameObject>>();
            }

            enterTriggerHandlers[t].Add(callback);
        }

        public void UnregisterEnterTriggerHandler(Action<GameObject> callback, string t = "") {
            if (!enterTriggerHandlers.ContainsKey(t)) return;
            enterTriggerHandlers[t].Remove(callback);
        }
        
        public void RegisterStayTriggerHandler(Action<GameObject> callback, string t = "") {
            if (!stayTriggerHandlers.ContainsKey(t)) {
                stayTriggerHandlers[t] = new HashSet<Action<GameObject>>();
            }

            stayTriggerHandlers[t].Add(callback);
        }

        public void UnregisterStayTriggerHandler(Action<GameObject> callback, string t = "") {
            if (!stayTriggerHandlers.ContainsKey(t)) return;
            stayTriggerHandlers[t].Remove(callback);
        }
        
        public void RegisterExitTriggerHandler(Action<GameObject> callback, string t = "") {
            if (!exitTriggerHandlers.ContainsKey(t)) {
                exitTriggerHandlers[t] = new HashSet<Action<GameObject>>();
            }

            exitTriggerHandlers[t].Add(callback);
        }

        public void UnregisterExitTriggerHandler(Action<GameObject> callback, string t = "") {
            if (!exitTriggerHandlers.ContainsKey(t)) return;
            exitTriggerHandlers[t].Remove(callback);
        }

        public bool CheckObjectInArea(GameObject obj) {
            return objectInArea.Contains(obj);
        }
        
        protected virtual void OnStayTriggerUpdate() { }

        private void OnTriggerEnter(Collider other) {
            var t = string.IsNullOrEmpty(other.tag) ? string.Empty : other.tag;
            if (!enterTriggerHandlers.ContainsKey(t)) return;
            objectInArea.Add(other.gameObject);
            foreach (var action in enterTriggerHandlers[t]) {
                action.Invoke(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other) {
            var t = string.IsNullOrEmpty(other.tag) ? string.Empty : other.tag;
            if (!stayTriggerHandlers.ContainsKey(t)) return;
            OnStayTriggerUpdate();
            foreach (var action in stayTriggerHandlers[t]) {
                action.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other) {
            var t = string.IsNullOrEmpty(other.tag) ? string.Empty : other.tag;
            if (!exitTriggerHandlers.ContainsKey(t)) return;
            objectInArea.Remove(other.gameObject);
            foreach (var action in exitTriggerHandlers[t]) {
                action.Invoke(other.gameObject);
            }
        }
    }
}
