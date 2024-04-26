﻿// File create date:2020/10/27
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
    /// <summary>
    /// Animator状态机代理，可以通过属性标签搜索各个状态的处理方法
    /// </summary>
    public class AnimatorStateMachine : MonoBehaviour {

        public bool isDebug;
        public bool autoUpdate;

        public Animator Animator => animator;

        protected Dictionary<string, HashSet<Action<string>>> dispatchTable =
            new Dictionary<string, HashSet<Action<string>>>();

        protected Animator animator;

        protected Dictionary<int, HashSet<Action>> stateUpdateMethods =
            new Dictionary<int, HashSet<Action>>();

        protected Dictionary<int, HashSet<Action>> stateEnterMethods =
            new Dictionary<int, HashSet<Action>>();

        protected Dictionary<int, HashSet<Action>> stateExitMethods =
            new Dictionary<int, HashSet<Action>>();

        protected Dictionary<int, string> hashToAnimString;
        protected int[] lastStateLayers;

        public int CurrentState => lastStateLayers[0];

        public event Action OnAnimatorMoveEvent; 

        private void Awake() {
            animator = GetComponent<Animator>();
            lastStateLayers = new int[animator.layerCount];
            DiscoverStateMethods();
        }

        private void Update() {
            if (autoUpdate) {
                StateMachineUpdate();
            }
        }
        
        private void OnAnimatorMove() {
            OnAnimatorMoveEvent?.Invoke();
        }

        private void OnValidate() {
            DiscoverStateMethods();
        }

        protected virtual void StateMachineUpdate() {
            for (var layer = 0; layer < lastStateLayers.Length; layer++) {
                var lastState = lastStateLayers[layer];
                var stateId = animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;
                if (lastState != stateId) {
                    if (stateExitMethods.ContainsKey(lastState)) {
                        foreach (var action in stateExitMethods[lastState]) {
                            action.Invoke();
                        }
                    }

                    if (stateEnterMethods.ContainsKey(stateId)) {
                        foreach (var action in stateEnterMethods[stateId]) {
                            action.Invoke();
                        }

                    }
                }

                if (stateUpdateMethods.ContainsKey(stateId)) {
                    foreach (var action in stateUpdateMethods[stateId]) {
                        action.Invoke();
                    }

                }

                lastStateLayers[layer] = stateId;
            }

        }

        private void DiscoverStateMethods() {

            hashToAnimString = new Dictionary<int, string>();
            var components = gameObject.GetComponents<MonoBehaviour>();

            var enterStateMethods = new List<StateMethod>();
            var updateStateMethods = new List<StateMethod>();
            var exitStateMethods = new List<StateMethod>();

            foreach (var component in components) {
                if (component == null) continue;

                var type = component.GetType();
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.DeclaredOnly |
                                              BindingFlags.InvokeMethod);

                foreach (var method in methods) {
                    var attributes =
                        method.GetCustomAttributes(typeof(StateUpdateMethod), true);
                    foreach (StateUpdateMethod attribute in attributes) {
                        var parameters = method.GetParameters();
                        if (parameters.Length != 0) continue;
                        var sm = CreateStateMethod(attribute.state, method, component);
                        InsertStateUpdateMethod(sm.stateHash, sm.method);
                    }

                    attributes = method.GetCustomAttributes(typeof(StateEnterMethod), true);
                    foreach (StateEnterMethod attribute in attributes) {
                        var parameters = method.GetParameters();
                        if (parameters.Length != 0) continue;
                        var sm = CreateStateMethod(attribute.state, method, component);
                        InsertStateEnterMethod(sm.stateHash, sm.method);
                    }

                    attributes = method.GetCustomAttributes(typeof(StateExitMethod), true);
                    foreach (StateExitMethod attribute in attributes) {
                        var parameters = method.GetParameters();
                        if (parameters.Length != 0) continue;
                        var sm = CreateStateMethod(attribute.state, method, component);
                        InsertStateExitMethod(sm.stateHash, sm.method);
                    }
                }
            }
        }

        protected virtual StateMethod CreateStateMethod(string state, MethodInfo method,
            MonoBehaviour component) {
            var stateHash = Animator.StringToHash(state);
            hashToAnimString[stateHash] = state;
            var stateMethod = new StateMethod();
            stateMethod.stateHash = stateHash;
            stateMethod.method = () => {
                method.Invoke(component, null);
            };
            return stateMethod;
        }

        protected void InsertStateEnterMethod(int hash, Action method) {
            if (!stateEnterMethods.ContainsKey(hash)) {
                stateEnterMethods[hash] = new HashSet<Action>();
            }

            stateEnterMethods[hash].Add(method);
        }

        protected void InsertStateUpdateMethod(int hash, Action method) {
            if (!stateUpdateMethods.ContainsKey(hash)) {
                stateUpdateMethods[hash] = new HashSet<Action>();
            }

            stateUpdateMethods[hash].Add(method);
        }

        protected void InsertStateExitMethod(int hash, Action method) {
            if (!stateExitMethods.ContainsKey(hash)) {
                stateExitMethods[hash] = new HashSet<Action>();
            }

            stateExitMethods[hash].Add(method);
        }

        public void RegisterStateEnterMethod(string path, Action method) {
            var hash = Animator.StringToHash(path);
            hashToAnimString[hash] = path;
            InsertStateEnterMethod(hash, method);
        }

        public void RegisterStateUpdateMethod(string path, Action method) {
            var hash = Animator.StringToHash(path);
            hashToAnimString[hash] = path;
            InsertStateUpdateMethod(hash, method);
        }

        public void RegisterStateExitMethod(string path, Action method) {
            var hash = Animator.StringToHash(path);
            hashToAnimString[hash] = path;
            InsertStateExitMethod(hash, method);
        }

        public void RegisterReceiver(string en, Action<string> callback) {
            if (!string.IsNullOrEmpty(en)) {
                if (!dispatchTable.ContainsKey(en)) {
                    dispatchTable[en] = new HashSet<Action<string>>();
                }
                dispatchTable[en].Add(callback);
            } else {
                Debug.LogWarning("Cannot register Animation Event Receiver with empty name!");
            }
        }

        public void UnregisterReceiver(Action<string> callback) {
            foreach (var en in dispatchTable.Keys) {
                dispatchTable[en].Remove(callback);
            }
        }

        public void DispatchAnimationEvent(string en) {
            if (isDebug) Debug.Log("Animator Event: " + en);
            if (string.IsNullOrEmpty(en) || !dispatchTable.ContainsKey(en)) return;
            var callbacks = dispatchTable[en];
            foreach (var func in callbacks) {
                func.Invoke(en);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StateUpdateMethod : Attribute {

        public string state;

        public StateUpdateMethod(string state) {
            this.state = state;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StateEnterMethod : Attribute {

        public string state;

        public StateEnterMethod(string state) {
            this.state = state;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StateExitMethod : Attribute {

        public string state;

        public StateExitMethod(string state) {
            this.state = state;
        }
    }

    public class StateMethod {
        public int stateHash;
        public Action method;
    }
}
