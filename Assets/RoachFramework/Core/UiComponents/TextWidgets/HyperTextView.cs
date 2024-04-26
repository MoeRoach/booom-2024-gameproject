using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 可以包含超链接的文本组件
    /// </summary>
    public class HyperTextView : Text, IPointerClickHandler {
        /// <summary>
        /// 超链接信息
        /// </summary>
        private class HyperLinkInfo {
            public int startIndex; // 开始索引
            public int endIndex; // 结束索引
            public string rValue; // 链接信息
            public string iValue; // 文本信息
            public Color color; // 颜色信息
            public readonly List<Rect> boxList = new List<Rect>(); // 可相应点击区域信息
        }

        private static readonly Regex hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
        private static readonly Regex colorRegex = new Regex(@"<color=([^>\n\s]+)>(.*?)(</color>)", RegexOptions.Singleline);
        private readonly List<HyperLinkInfo> hyperLinkInfoList = new List<HyperLinkInfo>();
        private Action<string, string> clickCallback;
        private string fixText = string.Empty;
        private Color innerTextColor = Color.blue;

        public void SetupClickCallback(Action<string, string> callback) {
            clickCallback = callback;
        }

        public void SetupLinkTextColor(string cstr) {
            if (!cstr.StartsWith("#")) {
                cstr = $"#{cstr}";
            }

            ColorUtility.TryParseHtmlString(cstr, out var nowColor);
            innerTextColor = nowColor;
        }

        public void OnPointerClick(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out var ap);
            foreach (var info in hyperLinkInfoList) {
                for (var i = 0; i < info.boxList.Count; i++) {
                    if (!info.boxList[i].Contains(ap)) continue;
                    clickCallback?.Invoke(info.rValue, info.iValue);
                    return;
                }
            }
        }
        
        protected override void OnPopulateMesh(VertexHelper toFill) {
            base.OnPopulateMesh(toFill);
            InitHyperlinkInfo();
            InitHyperlinkBox(toFill);
            DrawUnderline(toFill);
        }

        private void InitHyperlinkInfo() {
            fixText = GetOutputText(text);
        }

        private void InitHyperlinkBox(VertexHelper toFill) {
            var vert = new UIVertex();
            foreach (var info in hyperLinkInfoList) {
                info.boxList.Clear();
                var startVertex = info.startIndex * 4;
                var endVertex = info.endIndex * 4;
                if (startVertex >= toFill.currentVertCount) continue;
                toFill.PopulateUIVertex(ref vert, startVertex);
                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);
                for (var i = startVertex; i < endVertex; i++) {
                    if (i >= toFill.currentVertCount) break;
                    toFill.PopulateUIVertex(ref vert, i);
                    vert.color = info.color;
                    toFill.SetUIVertex(vert, i);
                    pos = vert.position;
                    var encapsulate = true;
                    if (i - startVertex != 0 && (i - startVertex) % 4 == 0) {
                        var lastv = new UIVertex();
                        toFill.PopulateUIVertex(ref lastv, i - 4);
                        var lastPos = lastv.position;
                        if (pos.x < lastPos.x && pos.y < lastPos.y) {
                            info.boxList.Add(new Rect(bounds.min, bounds.size));
                            bounds = new Bounds(pos, Vector3.zero);
                            encapsulate = false;
                        }
                    }

                    if (encapsulate) bounds.Encapsulate(pos);
                }

                info.boxList.Add(new Rect(bounds.min, bounds.size));
            }
        }

        private void DrawUnderline(VertexHelper vh) {
            foreach (var info in hyperLinkInfoList) {
                foreach (var rect in info.boxList) {
                    var lb = new Vector2(rect.min.x, rect.min.y);
                    var rb = new Vector2(rect.max.x, rect.min.y);
                    MeshUnderline(vh, lb, rb, info.color);
                }
            }
        }

        private void MeshUnderline(VertexHelper vh, Vector2 startPos, Vector2 endPos, Color lc) {
            var ext = rectTransform.rect.size;
            var setting = GetGenerationSettings(ext);
            var textGenerator = new TextGenerator();
            textGenerator.Populate("-", setting);
            var lineVerts = textGenerator.verts;
            var posArr = new Vector3[4];
            posArr[0] = startPos + new Vector2(-8f, 0f);
            posArr[1] = endPos + new Vector2(8f, 0f);
            posArr[2] = endPos + new Vector2(8f, -4f);
            posArr[3] = startPos + new Vector2(-8f, -4f);
            var tempVerts = new UIVertex[4];
            for (var i = 0; i < 4; i++) {
                tempVerts[i] = lineVerts[i];
                tempVerts[i].color = lc;
                tempVerts[i].position = posArr[i];
            }
            
            vh.AddUIVertexQuad(tempVerts);
        }

        private string GetOutputText(string str) {
            var builder = new StringBuilder();
            hyperLinkInfoList.Clear();
            var index = 0;
            var colorMatches = colorRegex.Matches(str);
            var linkMatches = hrefRegex.Matches(str);
            foreach (Match cm in colorMatches) {
                var isMatch = false;
                foreach (Match lm in linkMatches) {
                    if (!lm.Groups[2].Value.Contains(cm.Groups[2].Value)) continue;
                    isMatch = true;
                    break;
                }

                if (isMatch) continue;
                str = str.Replace($"<color={cm.Groups[1].Value}>{cm.Groups[2].Value}</color>", cm.Groups[2].Value);
            }

            linkMatches = hrefRegex.Matches(str);
            foreach (Match match in linkMatches) {
                var append = str.Substring(index, match.Index - index);
                builder.Append(append);
                builder = builder.Replace(" ", "");
                builder = builder.Replace("\n", "");
                var startIndex = builder.Length;
                var urlGroup = match.Groups[1];
                var titleGroup = match.Groups[2];
                var colorMatch = colorRegex.Match(titleGroup.Value);
                if (colorMatch.Groups.Count > 3) {
                    titleGroup = colorMatch.Groups[2];
                }

                var colorGroup = colorMatch.Groups[1];
                builder.Append(titleGroup.Value);
                var linkInfo = new HyperLinkInfo {
                    startIndex = startIndex, endIndex = (startIndex + titleGroup.Length),
                    rValue = urlGroup.Value, iValue = titleGroup.Value,
                    color = string.IsNullOrEmpty(colorGroup.Value) ? innerTextColor : FunctionExts.CompileColor(colorGroup.Value)
                };

                index = match.Index + match.Length;
                hyperLinkInfoList.Add(linkInfo);
            }

            builder.Append(str.Substring(index, str.Length - index));
            return builder.ToString();
        }
    }
}
