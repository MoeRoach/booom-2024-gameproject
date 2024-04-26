using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoachFramework {
    /// <summary>
    /// 自适应网格布局组件
    /// </summary>
    public class FlexibleGridLayout : LayoutGroup {
        
        public enum FitType {
			Uniform,
			Width,
			Height,
			FixRow,
			FixCol
		}

		public FitType fitType;
		public bool fitX;
		public bool fitY;
		
		public int rowCount;
		public int colCount;
		
		public Vector2 cellSize;
		public Vector2 spacing;

		public override void CalculateLayoutInputHorizontal() {
			base.CalculateLayoutInputHorizontal();
			var sqrtCount = Mathf.Sqrt(transform.childCount);
			switch (fitType) {
				case FitType.Width:
					colCount = Mathf.CeilToInt(sqrtCount);
					rowCount = Mathf
						.CeilToInt(transform.childCount / (float) colCount);
					break;
				case FitType.Height:
					rowCount = Mathf.CeilToInt(sqrtCount);
					colCount = Mathf
						.CeilToInt(transform.childCount / (float) rowCount);
					break;
				case FitType.Uniform:
					rowCount = Mathf.CeilToInt(sqrtCount);
					colCount = Mathf.CeilToInt(sqrtCount);
					break;
				case FitType.FixCol:
					rowCount = Mathf
						.CeilToInt(transform.childCount / (float) colCount);
					break;
				case FitType.FixRow:
					colCount = Mathf
						.CeilToInt(transform.childCount / (float) rowCount);
					break;
				default:
					return;
			}
			var rect = rectTransform.rect;
			var containWidth = rect.width;
			var containHeight = rect.height;
			var pad = padding;
			var cellWidth = (containWidth - pad.left - pad.right) / colCount -
			                spacing.x * (colCount - 1) / colCount;
			var cellHeight = (containHeight - pad.top - pad.bottom) / rowCount -
			                 spacing.y * (rowCount - 1) / rowCount;
			cellSize.x = fitX ? cellWidth : cellSize.x;
			cellSize.y = fitY ? cellHeight : cellSize.y;
			for (var i = 0; i < transform.childCount; i++) {
				var r = i / colCount;
				var c = i % colCount;
				var item = rectChildren[i];
				var xPos = (cellSize.x + spacing.x) * c + pad.left;
				var yPos = (cellSize.y + spacing.y) * r + pad.top;
				SetChildAlongAxis(item, 0, xPos, cellSize.x);
				SetChildAlongAxis(item, 1, yPos, cellSize.y);
			}
		}

		public override void CalculateLayoutInputVertical() { }

		public override void SetLayoutHorizontal() { }

		public override void SetLayoutVertical() { }
    }
}
