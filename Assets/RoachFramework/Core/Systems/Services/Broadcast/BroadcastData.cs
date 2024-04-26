using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 广播消息数据类
    /// </summary>
    public class BroadcastInfo {

        /// <summary>
        /// 广播行为标签
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// 广播内容
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 广播额外数据，纯字符串表示
        /// </summary>
        private readonly Dictionary<string, string> _extras;

        private BroadcastInfo(string action, string content = "Default Message") {
            Action = action;
            Content = content;
            _extras = new Dictionary<string, string>();
        }

        /// <summary>
        /// 设置数据集
        /// </summary>
        /// <param name="data">外部数据集</param>
        public void SetupExtras(Dictionary<string, string> data) {
            foreach (var key in data.Keys) {
                _extras[key] = data[key];
            }
        }

        /// <summary>
        /// 检查数据集中是否存在指定键
        /// </summary>
        /// <param name="key">键名称</param>
        /// <returns>是否包含</returns>
        public bool ContainsExtra(string key) {
            return _extras.ContainsKey(key);
        }

        /// <summary>
        /// 放入一个布尔型数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="flag">布尔</param>
        public void PutBooleanExtra(string key, bool flag) {
            _extras[key] = flag.ToString();
        }

        /// <summary>
        /// 放入一个整型数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="num">数字</param>
        public void PutIntegerExtra(string key, int num) {
            _extras[key] = num.ToString();
        }

        /// <summary>
        /// 放入一个字符串数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="str">字符串</param>
        public void PutStringExtra(string key, string str) {
            _extras[key] = str;
        }

        /// <summary>
        /// 放入一个浮点型数据
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="flt">浮点数字</param>
        public void PutFloatExtra(string key, float flt) {
            _extras[key] = flt.ToString("F4");
        }

        /// <summary>
        /// 放入一个复杂对象数据，Json序列化
        /// </summary>
        /// <param name="key">数据标识</param>
        /// <param name="obj">对象</param>
        public void PutComplexExtra<T>(string key, T obj) {
            _extras[key] = JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 获取一个储存的布尔数据
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>布尔</returns>
        public bool GetBooleanExtra(string key) {
            if (key != null && _extras.ContainsKey(key)) {
                return bool.Parse(_extras[key]);
            }
            return false;
        }

        /// <summary>
        /// 获取一个储存的数字
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>数字</returns>
        public int GetIntegerExtra(string key) {
            if (key != null && _extras.ContainsKey(key)) {
                return int.Parse(_extras[key]);
            }
            return int.MinValue;
        }

        /// <summary>
        /// 获取一个储存的字符串
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>字符串</returns>
        public string GetStringExtra(string key) {
            if (key != null && _extras.ContainsKey(key)) {
                return _extras[key];
            }
            return null;
        }

        /// <summary>
        /// 获取一个储存的浮点数字
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>浮点数</returns>
        public float GetFloatExtra(string key) {
            if (key != null && _extras.ContainsKey(key)) {
                return float.Parse(_extras[key]);
            }
            return float.NaN;
        }

        /// <summary>
        /// 获取一个储存的复杂类对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">标识</param>
        /// <returns>对象</returns>
        public T GetComplexExtra<T>(string key) {
            if (key != null && _extras.ContainsKey(key)) {
                return JsonConvert.DeserializeObject<T>(_extras[key]);
            }
            return default;
        }

        /// <summary>
        /// 创建一个新的广播数据对象
        /// </summary>
        /// <param name="action">广播行为</param>
        /// <param name="content">广播内容</param>
        /// <returns></returns>
        public static BroadcastInfo Create(string action,
            string content = "Default Message") {
            return new BroadcastInfo(action, content);
        }
    }
    
    /// <summary>
    /// 广播行为过滤器
    /// </summary>
    public class BroadcastFilter : IEnumerable<string> {

        private readonly HashSet<string> _filters; // 过滤器标签集合

        /// <summary>
        /// 初始化过滤器标签
        /// </summary>
        /// <param name="args">所有可用标签</param>
        public BroadcastFilter(params string[] args) {
            _filters = new HashSet<string>();
            foreach (var arg in args) {
                _filters.Add(arg);
            }
        }

        /// <summary>
        /// 添加一个过滤器标签
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(string filter) {
            _filters.Add(filter);
        }

        /// <summary>
        /// 添加一系列过滤器标签
        /// </summary>
        /// <param name="filters"></param>
        public void AddFilter(params string[] filters) {
            foreach (var filter in filters) {
                _filters.Add(filter);
            }
        }

        /// <summary>
        /// 获取过滤器标签枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<string> GetEnumerator() {
            return _filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _filters.GetEnumerator();
        }
    }
}
