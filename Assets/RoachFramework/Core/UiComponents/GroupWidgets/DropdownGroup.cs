using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 下拉选择组
    /// </summary>
    public class DropdownGroup : BaseUiWidget {
        
        public bool isExtraDropdowns = false;
        public Dropdown[] dropdownList;

        private List<List<string>> _dropOptions;
        private List<int> _selectList;
        private int _dropdownCount;

        public event Action OnGroupChange;

        protected override void LoadViews() {
            _selectList = new List<int>();
            _dropOptions = new List<List<string>>();
            if (isExtraDropdowns) {
                _dropdownCount = dropdownList.Length;
                for (var i = 0; i < _dropdownCount; i++) {
                    dropdownList[i].onValueChanged.AddListener(OnSelectChange);
                    _dropOptions.Add(new List<string>());
                    _selectList.Add(0);
                }
            } else {
                _dropdownCount = transform.childCount;
                dropdownList = new Dropdown[_dropdownCount];
                for (var i = 0; i < _dropdownCount; i++) {
                    dropdownList[i] = transform.GetChild(i).GetComponent<Dropdown>();
                    dropdownList[i].onValueChanged.AddListener(OnSelectChange);
                    _dropOptions.Add(new List<string>());
                    _selectList.Add(0);
                }
            }
        }

        protected override void PostLoad() {
            YieldAction(UpdateDropdown);
        }

        /// <summary>
        /// 刷新所有下拉
        /// </summary>
        private void UpdateDropdown() {
            for (var i = 0; i < _dropdownCount; i++) {
                UpdateSingleDropdown(i);
            }
        }

        /// <summary>
        /// 刷新单个下拉
        /// </summary>
        /// <param name="i">索引</param>
        private void UpdateSingleDropdown(int i) {
            var drop = dropdownList[i];
            drop.options.Clear();
            if (i < _dropdownCount) {
                var opts = _dropOptions[i];
                opts.EnumerateAction(str => {
                    var op = new Dropdown.OptionData {text = str};
                    drop.options.Add(op);
                });
            }
            drop.value = 0;
            drop.gameObject.FindComponent<Text>("Label").text = drop.options.Count > 0 ? drop.options[0].text : "--";
        }
        /// <summary>
        /// 收集下拉选项信息
        /// </summary>
        private void GatherSelectInfo() {
            for (var i = 0; i < _dropdownCount; i++) {
                _selectList[i] = dropdownList[i].value;
            }
        }

        private IEnumerator UpdateSelect(int index, int value) {
            yield return null;
            dropdownList[index].value = value;
        }

        /// <summary>
        /// 设置每个下拉列表的选项
        /// </summary>
        /// <param name="lists">所有选项</param>
        public void SetupDropdowns(List<List<string>> lists) {
            _dropOptions = lists;
            YieldAction(UpdateDropdown);
        }
        /// <summary>
        /// 根据索引设置下拉列表选项
        /// </summary>
        /// <param name="list">选项表</param>
        /// <param name="index">索引</param>
        public void SetupDropdownByIndex(List<string> list, int index) {
            _dropOptions[index] = list;
            UpdateSingleDropdown(index);
        }
        /// <summary>
        /// 根据索引设置指定下拉的选项
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">选项</param>
        public void SetupDropdownSelect(int index, int value) {
            if (index >= _dropdownCount) return;
            _selectList[index] = value;
            StartCoroutine(UpdateSelect(index, value));
        }
        /// <summary>
        /// 重置所有选项到初始值
        /// </summary>
        public void ResetSelected() {
            for (var i = 0; i < _selectList.Count; i++) {
                SetupDropdownSelect(i, 0);
            }
        }
        /// <summary>
        /// 获取所有下拉选择数据
        /// </summary>
        /// <returns>选择数据</returns>
        public List<int> GetSelected() {
            GatherSelectInfo();
            return new List<int>(_selectList);
        }
        /// <summary>
        /// 获取下拉列表数量
        /// </summary>
        /// <returns>列表数量</returns>
        public int GetDropdownCount() {
            return _dropdownCount;
        }

        private void OnSelectChange(int select) {
            OnGroupChange?.Invoke();
        }
    }
}
