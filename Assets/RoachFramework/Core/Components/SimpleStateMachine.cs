using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 一个可外部配置的，只有简单状态维护功能的状态机
    /// </summary>
    public class SimpleStateMachine {
        public Enum CurrentState { get; protected set; }
        public Enum PreviousState { get; protected set; }
        protected Enum entryState; // 入口状态缓存，用于重置状态机
        protected Dictionary<string, MachineState> machineStates;

        public SimpleStateMachine() {
            machineStates = new Dictionary<string, MachineState>();
        }

        public void ChangeState(Enum state) {
            if (CurrentState == null) return;
            var currStateName = CurrentState.ToString();
            var nextStateName = state.ToString();
            if (machineStates.ContainsKey(nextStateName)) {
                machineStates[currStateName].exitAction?.Invoke();
                PreviousState = CurrentState;
                CurrentState = state;
                machineStates[nextStateName].enterAction?.Invoke();
            } else {
                throw new NotSupportedException("Certain state not configurated!");
            }
        }

        public void UpdateState() {
            var currStateName = CurrentState.ToString();
            if (machineStates.ContainsKey(currStateName)) {
                machineStates[currStateName].updateAction?.Invoke();
            }
        }

        public void EntryState(Enum state) {
            var stateName = state.ToString();
            if (machineStates.ContainsKey(stateName)) {
                if (!state.Equals(entryState)) {
                    entryState = state;
                }
                CurrentState = state;
                PreviousState = null;
            } else {
                throw new NotSupportedException("Entry state not configurated!");
            }
        }

        public void FireTrigger(Enum trigger) {
            var currStateName = CurrentState.ToString();
            if (machineStates.ContainsKey(currStateName)) {
                machineStates[currStateName].FireTrigger(trigger);
            } else {
                throw new NotSupportedException("Current state not configurated!");
            }
        }

        public bool CheckTransition(Enum srcState, Enum dstState) {
            var srcStateName = srcState.ToString();
            return machineStates.ContainsKey(srcStateName) &&
                   machineStates[srcStateName].CheckTransition(dstState);
        }

        public bool CheckTrigger(Enum trigger) {
            var currStateName = CurrentState.ToString();
            return machineStates.ContainsKey(currStateName) &&
                   machineStates[currStateName].CheckTrigger(trigger);
        }

        public MachineState SetupState(Enum state) {
            var stateName = state.ToString();
            if (machineStates.ContainsKey(stateName)) return machineStates[stateName];
            var s = new MachineState(state, this);
            machineStates[stateName] = s;
            return machineStates[stateName];
        }

        public void ResetMachine() {
            EntryState(entryState);
        }

        public class MachineState {
            public string stateName;
            public Enum state;
            public Action enterAction;
            public Action updateAction;
            public Action exitAction;
            private SimpleStateMachine stateMachine;
            private Dictionary<string, StateTransition> transitionSet;

            public MachineState(Enum state, SimpleStateMachine machine) {
                this.state = state;
                stateName = state.ToString();
                stateMachine = machine;
                transitionSet = new Dictionary<string, StateTransition>();
            }

            public MachineState OnUpdateState(Action action) {
                updateAction = action;
                return this;
            }

            public MachineState OnEnterState(Action action) {
                enterAction = action;
                return this;
            }

            public MachineState OnExitState(Action action) {
                exitAction = action;
                return this;
            }

            public MachineState Grant(Enum dstState, params Enum[] triggers) {
                var transitionName = $"{stateName}-{dstState.ToString()}";
                if (transitionSet.ContainsKey(transitionName)) {
                    // 已经有了这个转移，将触发器包含进去
                    var transition = transitionSet[transitionName];
                    foreach (var trigger in triggers) {
                        transition.GrantTrigger(trigger);
                    }
                } else {
                    // 新建一个转移
                    var transition = new StateTransition(state, dstState, stateMachine);
                    foreach (var trigger in triggers) {
                        transition.GrantTrigger(trigger);
                    }
                    transitionSet[transitionName] = transition;
                }
                return this;
            }

            public MachineState TransitionMode(Enum dstState, bool isAnyTransition) {
                var transitionName = $"{stateName}-{dstState.ToString()}";
                if (transitionSet.ContainsKey(transitionName)) {
                    transitionSet[transitionName].isAnyTransitition = isAnyTransition;
                } else {
                    throw new NotSupportedException("Transition not found!");
                }
                return this;
            }

            public bool CheckTransition(Enum dstState) {
                var transitionName = $"{stateName}-{dstState.ToString()}";
                return transitionSet.ContainsKey(transitionName);
            }

            public MachineState Ignore(Enum dstState, Enum trigger = null) {
                var transitionName = $"{stateName}-{dstState.ToString()}";
                if (trigger != null) {
                    if (!transitionSet.ContainsKey(transitionName)) return this;
                    // 已经有了这个转移，将触发器包含进去
                    var transition = transitionSet[transitionName];
                    transition.IgnoreTrigger(trigger);
                } else {
                    transitionSet.Remove(transitionName);
                }
                return this;
            }

            public bool CheckTrigger(Enum trigger) {
                var result = (transitionSet.Count > 0);
                foreach (var transition in transitionSet.Values) {
                    result &= transition.CheckTrigger(trigger);
                }
                return result;
            }

            public void FireTrigger(Enum trigger) {
                foreach (var transition in transitionSet.Values) {
                    transition.FireTrigger(trigger);
                }
            }
        }

        public class StateTransition {
            public Enum srcState;
            public Enum dstState;
            public string transitionName;
            public bool isAnyTransitition = true;
            private SimpleStateMachine stateMachine;
            private Dictionary<string, bool> conditions;
            private bool conditionAny = true;
            private bool conditionAll = true;

            public StateTransition(Enum src, Enum dst, SimpleStateMachine machine) {
                srcState = src;
                dstState = dst;
                transitionName = $"{src.ToString()}-{dst.ToString()}";
                stateMachine = machine;
                conditions = new Dictionary<string, bool>();
            }

            public void GrantTrigger(Enum trigger) {
                conditions[trigger.ToString()] = false;
            }

            public bool CheckTrigger(Enum trigger) {
                return conditions.ContainsKey(trigger.ToString());
            }

            public void IgnoreTrigger(Enum trigger) {
                conditions.Remove(trigger.ToString());
            }

            public void FireTrigger(Enum trigger) {
                var triggerName = trigger.ToString();
                if (!conditions.ContainsKey(triggerName)) return;
                conditions[triggerName] = true;
                conditionAny = true;
                conditionAll = true;
                foreach (var flag in conditions.Values) {
                    conditionAll &= flag;
                }
                if (isAnyTransitition) {
                    AnyTransit();
                } else {
                    AllTransit();
                }
            }

            public bool CheckAny() {
                return conditionAny;
            }

            public bool CheckAll() {
                return conditionAll;
            }

            public void AnyTransit() {
                if (conditionAny) {
                    // 任何条件满足都进行转移
                    stateMachine.ChangeState(dstState);
                }
            }

            public void AllTransit() {
                if (conditionAll) {
                    // 所有条件都满足才转移
                    stateMachine.ChangeState(dstState);
                }
            }
        }
    }
}
