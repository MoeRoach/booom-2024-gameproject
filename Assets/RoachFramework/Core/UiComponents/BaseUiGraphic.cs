using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoachFramework {
	/// <summary>
	/// UI绘图组件基类
	/// </summary>
    public abstract class BaseGraphic : MaskableGraphic, IPointerEnterHandler, IPointerDownHandler,
	IDragHandler, IPointerUpHandler, IPointerExitHandler {

		protected RectTransform rect;
		protected Vector2 graphPosition;
		protected Vector2 graphPivot;
		protected Vector2 originPosition; // 左下角原点坐标
		protected Vector2 maximumPosition; // 右上角最远点坐标
		protected bool isInit;

		protected sealed override void Awake() {
			base.Awake();
			rect = rectTransform;
			InitPosition();
			OnAwake();
		}

		private async void InitPosition() {
			await UniTask.Yield();
			var r = rect.rect;
			graphPivot = rect.pivot;
			originPosition = new Vector2(-graphPivot.x * r.width,
				-graphPivot.y * r.height);
			maximumPosition = new Vector2(r.width * (1f - graphPivot.x),
				r.height * (1f - graphPivot.y));
			isInit = true;
		}

		protected virtual void OnAwake() { }

		protected sealed override void Start() {
			base.Start();
			OnStart();
		}

		protected virtual void OnStart() { }

		public virtual void NotifyUpdate() {
			UpdateGraphic();
		}

		private async void UpdateGraphic() {
			while (!isInit) await UniTask.Yield();
			SetAllDirty();
		}

		protected sealed override void OnPopulateMesh(VertexHelper vh) {
			vh.Clear();
			DrawGraphic(ref vh);
		}

		protected virtual void DrawGraphic(ref VertexHelper vh) { }

		/// <summary>
		/// 通过两个端点绘制矩形
		/// </summary>
		/// <param name="startPos">起始点</param>
		/// <param name="endPos">终点</param>
		/// <param name="color0">颜色</param>
		/// <param name="lineWidth">线宽</param>
		/// <returns>用于绘制的顶点集</returns>
		protected UIVertex[] GetLine(Vector2 startPos, Vector2 endPos, Color color0,
			float lineWidth = 2.0f) {
			var dis = Vector2.Distance(startPos, endPos);
			var y = lineWidth * 0.5f * (endPos.x - startPos.x) / dis;
			var x = lineWidth * 0.5f * (endPos.y - startPos.y) / dis;
			if (y <= 0) {
				y = -y;
			} else {
				x = -x;
			}

			var vertex = new UIVertex[4];
			vertex[0].position = new Vector3(startPos.x + x, startPos.y + y);
			vertex[1].position = new Vector3(endPos.x + x, endPos.y + y);
			vertex[2].position = new Vector3(endPos.x - x, endPos.y - y);
			vertex[3].position = new Vector3(startPos.x - x, startPos.y - y);
			for (var i = 0; i < vertex.Length; i++) {
				vertex[i].color = color0;
			}

			return vertex;
		}

		/// <summary>
		/// 通过四个顶点绘制矩形
		/// </summary>
		/// <param name="first">第一个顶点</param>
		/// <param name="second">第二个顶点</param>
		/// <param name="third">第三个顶点</param>
		/// <param name="four">第四个顶点</param>
		/// <param name="color0">颜色</param>
		/// <returns>用于绘制的顶点集合</returns>
		protected UIVertex[] GetRect(Vector2 first, Vector2 second, Vector2 third,
			Vector2 four, Color color0) {
			var vertexs = new UIVertex[4];
			vertexs[0] = GetUiVertex(first, color0);
			vertexs[1] = GetUiVertex(second, color0);
			vertexs[2] = GetUiVertex(third, color0);
			vertexs[3] = GetUiVertex(four, color0);
			return vertexs;
		}

		/// <summary>
		/// 构造UIVertex
		/// </summary>
		/// <param name="point">顶点</param>
		/// <param name="color0">颜色</param>
		/// <returns>构造的绘制顶点集合</returns>
		private UIVertex GetUiVertex(Vector2 point, Color color0) {
			var vertex = new UIVertex {
				position = point,
				color = color0,
				uv0 = new Vector2(0, 0)
			};
			return vertex;
		}

		/// <summary>
		/// 构造一个方形表示点位
		/// </summary>
		/// <param name="point">点位的坐标</param>
		/// <param name="color0">点的颜色</param>
		/// <param name="size">点的大小</param>
		/// <returns>用于绘制的顶点集合</returns>
		protected UIVertex[] GetSquarePoint(Vector2 point, Color color0, float size = 2) {
			var squareLeft = point;
			squareLeft.x -= size / 2f;
			var squareRight = point;
			squareRight.x += size / 2f;
			return GetLine(squareLeft, squareRight, color0, size);
		}

		/// <summary>
		/// 绘制虚线
		/// </summary>
		/// <param name="vh">VertexHelper引用对象</param>
		/// <param name="first">起始点</param>
		/// <param name="second">终点</param>
		/// <param name="color0">虚线的颜色</param>
		/// <param name="imaginaryLenght">实部长度</param>
		/// <param name="spaceingWidth">虚部长度</param>
		/// <param name="lineWidth">线宽</param>
		protected void GetImaglinaryLine(ref VertexHelper vh, Vector2 first, Vector2 second,
			Color color0, float imaginaryLenght, float spaceingWidth, float lineWidth = 2f) {
			if (first.y.Equals(second.y)) {
				var indexSecond = first + new Vector2(imaginaryLenght, 0);
				while (indexSecond.x < second.x) {
					vh.AddUIVertexQuad(GetLine(first, indexSecond, color0));
					first = indexSecond + new Vector2(spaceingWidth, 0);
					indexSecond = first + new Vector2(imaginaryLenght, 0);
					if (!(indexSecond.x > second.x)) continue;
					indexSecond = new Vector2(second.x, indexSecond.y);
					vh.AddUIVertexQuad(GetLine(first, indexSecond, color0));
				}
			}

			if (first.x.Equals(second.x)) {
				var indexSecond = first + new Vector2(0, imaginaryLenght);
				while (indexSecond.y < second.y) {
					vh.AddUIVertexQuad(GetLine(first, indexSecond, color0));
					first = indexSecond + new Vector2(0, spaceingWidth);
					indexSecond = first + new Vector2(0, imaginaryLenght);
					if (!(indexSecond.y > second.y)) continue;
					indexSecond = new Vector2(indexSecond.x, second.y);
					vh.AddUIVertexQuad(GetLine(first, indexSecond, color0));
				}
			}
		}

		protected virtual void ProcessPointerEnter(PointerEventData ev) { }

		protected virtual void ProcessPointerExit(PointerEventData ev) { }

		protected virtual void ProcessPointerDrag(PointerEventData ev) { }

		protected virtual void ProcessPointerPress(PointerEventData ev) { }

		protected virtual void ProcessPointerRelease(PointerEventData ev) { }

		public void OnPointerEnter(PointerEventData eventData) {
			ProcessPointerEnter(eventData);
		}

		public void OnPointerDown(PointerEventData eventData) {
			ProcessPointerPress(eventData);
		}

		public void OnDrag(PointerEventData eventData) {
			ProcessPointerDrag(eventData);
		}

		public void OnPointerUp(PointerEventData eventData) {
			ProcessPointerRelease(eventData);
		}

		public void OnPointerExit(PointerEventData eventData) {
			ProcessPointerExit(eventData);
		}
	}
}
