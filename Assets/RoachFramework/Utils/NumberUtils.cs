// File create date:1/6/2018
using UnityEngine;

// Created By Yu.Liu
namespace RoachFramework {
    /// <summary>
    /// 数字相关工具类
    /// </summary>
    public static class NumberUtils {
        /// <summary>
        /// 通用随机整数获取工具
        /// </summary>
        /// <param name="max">随机最大值</param>
        /// <param name="min">随机最小值</param>
        /// <returns>整数值</returns>
        public static int RandomInteger(int max, int min = 0) {
            return Random.Range(min, max + 1);
        }
        /// <summary>
        /// 通用随机浮点数获取工具
        /// </summary>
        /// <param name="max">随机最大值</param>
        /// <param name="min">随机最小值</param>
        /// <returns>浮点数值</returns>
        public static float RandomFloat(float max, float min = 0f) {
            return Random.Range(min, max);
        }
    }
    /// <summary>
    /// 数学计算相关工具
    /// </summary>
    public static class MathUtils {
        /// <summary>
        /// 线性插值（float）
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static float LinearInterpolate(float from, float to, float percentage) {
            return (to - from) * percentage + from;
        }

        /// <summary>
        /// 线性插值（Vector2）
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector2 LinearInterpolate(Vector2 from, Vector2 to, float percentage) {
            return new Vector2(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage));
        }

        /// <summary>
        /// 线性插值（Vector3）
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector3 LinearInterpolate(Vector3 from, Vector3 to, float percentage) {
            return new Vector3(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage), LinearInterpolate(from.z, to.z, percentage));
        }

        /// <summary>
        /// 线性插值（Vector4）
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector4 LinearInterpolate(Vector4 from, Vector4 to, float percentage) {
            return new Vector4(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage), LinearInterpolate(from.z, to.z, percentage), LinearInterpolate(from.w, to.w, percentage));
        }

        /// <summary>
        /// 线性插值（Rect）
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Rect LinearInterpolate(Rect from, Rect to, float percentage) {
            return new Rect(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage), LinearInterpolate(from.width, to.width, percentage), LinearInterpolate(from.height, to.height, percentage));
        }

        /// <summary>
        /// 线性插值（Color）
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Color LinearInterpolate(Color from, Color to, float percentage) {
            return new Color(LinearInterpolate(from.r, to.r, percentage), LinearInterpolate(from.g, to.g, percentage), LinearInterpolate(from.b, to.b, percentage), LinearInterpolate(from.a, to.a, percentage));
        }

        //animation curves:
        /// <summary>
        /// 获取曲线映射值
        /// </summary>
        /// <returns>The value evaluated at the percentage of the clip.</returns>
        /// <param name="curve">Curve.</param>
        /// <param name="percentage">Percentage.</param>
        public static float EvaluateCurve(AnimationCurve curve, float percentage) {
            return curve.Evaluate((curve[curve.length - 1].time) * percentage);
        }
        
        /// <summary>
        /// 获取二阶贝塞尔曲线值
        /// </summary>
        /// <param name="p0">起始点</param>
        /// <param name="p1">调整点</param>
        /// <param name="p2">结束点</param>
        /// <param name="t">位置</param>
        /// <returns></returns>
        public static Vector2 EvaluateBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
            var aa = p0 + (p1 - p0) * t;
            var bb = p1 + (p2 - p1) * t;
            return aa + (bb - aa) * t;
        }
    }
}
