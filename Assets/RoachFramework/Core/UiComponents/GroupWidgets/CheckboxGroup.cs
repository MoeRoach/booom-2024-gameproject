using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 通用选择组，支持单选和多选
    /// </summary>
    public class CheckboxGroup : BaseUiWidget {
        
        public bool singleSelect = true;
        public bool canSwitchOff = false;
        public bool isExtraToggles = false;
        public Toggle[] toggleList;

        private int _toggleCount; // 选项个数
        private List<bool> _toggleStatus; // 选中情况
        private int _selectToggleCount; // 选中个数

        private bool _callbackSwitch = true;

        public event Action<int> OnGroupChange;

        protected override void LoadViews() {
            _toggleStatus = new List<bool>();
            if (isExtraToggles) {
                _toggleCount = toggleList.Length;
                for (var i = 0; i < _toggleCount; i++) {
                    toggleList[i].onValueChanged.AddListener(OnToggleChange);
                    _toggleStatus.Add(toggleList[i].isOn);
                }
            } else {
                _toggleCount = transform.childCount;
                toggleList = new Toggle[_toggleCount];
                for (var i = 0; i < transform.childCount; i++) {
                    toggleList[i] = transform.GetChild(i).GetComponent<Toggle>();
                    toggleList[i].onValueChanged.AddListener(OnToggleChange);
                    _toggleStatus.Add(toggleList[i].isOn);
                }
            }
            if (singleSelect) {
                var grp = gameObject.GetComponent<ToggleGroup>();
                if (grp == null) {
                    grp = gameObject.AddComponent<ToggleGroup>();
                }
                for (var i = 0; i < _toggleCount; i++) {
                    toggleList[i].group = grp;
                }
                grp.allowSwitchOff = canSwitchOff;
            }
        }

        /// <summary>
        /// 设置一个选项被选中
        /// </summary>
        /// <param name="index">选项索引</param>
        /// <param name="allowCallback">是否允许设置触发回调</param>
        public void SetToggleSelected(int index, bool allowCallback = true) {
            if (index < _toggleCount) {
                if (!toggleList[index].isOn) {
                    _callbackSwitch = allowCallback;
                    toggleList[index].isOn = true;
                } else {
                    if (OnGroupChange != null && allowCallback) {
                        OnGroupChange(index);
                    }
                }
            }
        }
        /// <summary>
        /// 获取当前选中项（限单选）
        /// </summary>
        /// <returns>当前选中</returns>
        public int GetToggleSelected() {
            if (singleSelect && toggleList != null) {
                for (var i = 0; i < _toggleCount; i++) {
                    if (toggleList[i].isOn) {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取指定选中项
        /// </summary>
        /// <param name="index">指定项索引</param>
        /// <returns>当前选中</returns>
        public bool GetSingleToggle(int index) {
            if (toggleList != null) {
                return toggleList[index].isOn;
            }
            return false;
        }
        /// <summary>
        /// 获取当前选中项（全部）
        /// </summary>
        /// <returns>当前选中</returns>
        public List<bool> GetToggles() {
            GatherToggleInfo();
            return _toggleStatus;
        }
        /// <summary>
        /// 获取选项数量
        /// </summary>
        /// <returns></returns>
        public int GetToggleCount() {
            return _toggleCount;
        }
        /// <summary>
        /// 重置选中情况
        /// </summary>
        /// <param name="index">默认索引</param>
        /// <exception cref="IndexOutOfRangeException">索引越界</exception>
        public void ResetGroup(int index = 0) {
            if (index < 0) {
                throw new IndexOutOfRangeException();
            }
            for (var i = 0; i < toggleList.Length; i++) {
                if (canSwitchOff) {
                    // 可以取消的话直接全部关闭即可
                    toggleList[i].isOn = false;
                } else {
                    // 不能取消的必须有默认
                    toggleList[i].isOn = i == index;
                }
            }
        }
        /// <summary>
        /// 收集选项组信息
        /// </summary>
        private void GatherToggleInfo() {
            _selectToggleCount = 0;
            for (var i = 0; i < _toggleCount; i++) {
                _toggleStatus[i] = toggleList[i].isOn;
                if (_toggleStatus[i]) {
                    _selectToggleCount++;
                }
            }
        }

        private void OnToggleChange(bool isOn) {
            if (singleSelect) {
                // 单选组，回调用索引
                if (_callbackSwitch) {
                    GatherToggleInfo();
                    if (isOn || _selectToggleCount == 0) {
                        OnGroupChange?.Invoke(GetToggleSelected());
                    }
                } else {
                    _callbackSwitch = true;
                }
            } else {
                // 多选组，回调用个数
                if (_callbackSwitch) {
                    GatherToggleInfo();
                    OnGroupChange?.Invoke(_selectToggleCount);
                } else {
                    _callbackSwitch = true;
                }
            }
        }
    }
}
