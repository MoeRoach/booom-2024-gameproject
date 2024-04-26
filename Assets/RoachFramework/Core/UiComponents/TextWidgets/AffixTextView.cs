using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 附带前后缀的Text辅助组件
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class AffixTextView : BaseUiWidget {
        
        public bool hasPrefix;
        public string prefixText = "";
        public bool hasSuffix;
        public string suffixText = "";

        private Text _contentText;

        public string text {
            set => UpdateText(value);
        }

        private void Start() {
            _contentText = GetComponent<Text>();
        }

        public void SetText(string content) {
            UpdateText(content);
        }

        private async void UpdateText(string str) {
            await UniTask.Yield();
            var result = "";
            if (hasPrefix) {
                result += prefixText;
            }
            
            result += str;
            if (hasSuffix) {
                result += suffixText;
            }
            
            _contentText.text = result;
        }
    }
}
