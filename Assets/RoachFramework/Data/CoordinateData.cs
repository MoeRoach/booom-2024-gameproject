using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
	/// 正方形网格坐标表示
	/// </summary>
	public class Square : IEquatable<Square> {
		
		public static Square Zero => new Square();
		public static Square One => new Square(1, 1);
		
		public int x;
		public int y;

		public string Sid => CoordinateUtils.CompileSquare(x, y);
		public int Clength => Mathf.Abs(x) + Mathf.Abs(y);
		public float Magnitude => Mathf.Sqrt(x * x + y * y);

		public Square() {
			x = 0;
			y = 0;
		}
		
		public Square(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public void From(Vector2Int v) {
			x = v.x;
			y = v.y;
		}

		public static implicit operator Vector2Int(Square sq) {
			return new Vector2Int(sq.x, sq.y);
		}
		
		public static implicit operator Square(Vector2Int v) {
			return new Square(v.x, v.y);
		}

		/// <summary>
		/// 重载三轴坐标的加法操作
		/// </summary>
		/// <param name="left">左操作数</param>
		/// <param name="right">右操作数</param>
		/// <returns>结果</returns>
		public static Square operator +(Square left, Square right) {
			return new Square() {
				x = left.x + right.x,
				y = left.y + right.y,
			};
		}

		/// <summary>
		/// 重载三轴坐标的减法操作
		/// </summary>
		/// <param name="left">左操作数</param>
		/// <param name="right">右操作数</param>
		/// <returns>结果</returns>
		public static Square operator -(Square left, Square right) {
			return new Square() {
				x = left.x - right.x,
				y = left.y - right.y,
			};
		}

		/// <summary>
		/// 重载坐标乘法
		/// </summary>
		/// <param name="origin">原坐标</param>
		/// <param name="multi">乘数</param>
		/// <returns>新坐标</returns>
		public static Square operator *(Square origin, int multi) {
			return new Square(origin.x * multi, origin.y * multi);
		}
		
		/// <summary>
		/// 重载坐标除法
		/// </summary>
		/// <param name="origin">原坐标</param>
		/// <param name="div">除数</param>
		/// <returns>新坐标</returns>
		public static Square operator /(Square origin, int div) {
			return new Square(origin.x / div, origin.y / div);
		}

		/// <summary>
		/// 克隆一个相同坐标对象
		/// </summary>
		/// <returns></returns>
		public Square Clone() {
			return new Square(x, y);
		}

		public bool Equals(Square other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return x == other.x && y == other.y;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Square) obj);
		}

		public override int GetHashCode() {
			return HashCode.Combine(x, y);
		}
	}
	
	/// <summary>
	/// 六边形三轴坐标表示
	/// </summary>
	public class Cubic : IEquatable<Cubic> {
		
		public int x;
		public int y;
		public int z;

		public string Cid => HexagonUtils.CompileCubic(x, y, z);

		public Cubic() {
			x = 0;
			y = 0;
			z = 0;
		}

		public Cubic(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		
		/// <summary>
		/// 重载三轴坐标的加法操作
		/// </summary>
		/// <param name="left">左操作数</param>
		/// <param name="right">右操作数</param>
		/// <returns>结果</returns>
		public static Cubic operator +(Cubic left, Cubic right) {
			return new Cubic() {
				x = left.x + right.x,
				y = left.y + right.y,
				z = left.z + right.z
			};
		}

		/// <summary>
		/// 重载三轴坐标的减法操作
		/// </summary>
		/// <param name="left">左操作数</param>
		/// <param name="right">右操作数</param>
		/// <returns>结果</returns>
		public static Cubic operator -(Cubic left, Cubic right) {
			return new Cubic() {
				x = left.x - right.x,
				y = left.y - right.y,
				z = left.z - right.z
			};
		}

		/// <summary>
		/// 重载坐标乘法
		/// </summary>
		/// <param name="origin">原坐标</param>
		/// <param name="multi">乘数</param>
		/// <returns>新坐标</returns>
		public static Cubic operator *(Cubic origin, int multi) {
			return new Cubic(origin.x * multi, origin.y * multi, origin.z * multi);
		}
		
		/// <summary>
		/// 重载坐标除法
		/// </summary>
		/// <param name="origin">原坐标</param>
		/// <param name="div">除数</param>
		/// <returns>新坐标</returns>
		public static Cubic operator /(Cubic origin, int div) {
			return new Cubic(origin.x / div, origin.y / div, origin.z / div);
		}
		
		public static implicit operator Vector2Int(Cubic c) {
			return c.To();
		}

		public static implicit operator Cubic(Vector2Int v) {
			return ConvertFrom(v);
		}

		/// <summary>
		/// 平面坐标转化为立方坐标
		/// </summary>
		/// <param name="x">x坐标</param>
		/// <param name="y">y坐标</param>
		/// <returns>立方坐标</returns>
		public static Cubic ConvertFrom(int x, int y) {
			var cubic = new Cubic();
			cubic.From(x, y);
			return cubic;
		}

		/// <summary>
		/// 平面坐标转化为立方坐标
		/// </summary>
		/// <param name="coord">向量坐标</param>
		/// <returns>立方坐标</returns>
		public static Cubic ConvertFrom(Vector2Int coord) {
			return ConvertFrom(coord.x, coord.y);
		}
		
		/// <summary>
		/// 从平面映射坐标转化为三轴坐标
		/// </summary>
		/// <param name="coord">平面坐标</param>
		public void From(Vector2Int coord) {
			// Point-Top
			x = coord.x + ((coord.y & 1) + coord.y) / 2;
			z = -coord.y;
			y = -x - z;
			// Flat-Top
			// x = coord.x - ((coord.y & 1) - coord.y) / 2;
			// z = -coord.y;
			// y = -x - z;
		}
		
		/// <summary>
		/// 从平面映射坐标转化为三轴坐标
		/// </summary>
		/// <param name="vx">平面坐标x</param>
		/// <param name="vy">平面坐标y</param>
		public void From(int vx, int vy) {
			// Point-Top
			x = vx + ((vy & 1) + vy) / 2;
			z = -vy;
			y = -x - z;
			// Flat-Top
			// x = vx - ((vy & 1) - vy) / 2;
			// z = -vy;
			// y = -x - z;
		}

		/// <summary>
		/// 将当前三轴坐标转化为平面映射坐标
		/// </summary>
		/// <returns>平面坐标</returns>
		public Vector2Int To() {
			var result = Vector2Int.zero;
			// Point-Top
			result.x = x + (z - (z & 1)) / 2;
			result.y = -z;
			// Flat-Top
			// result.x = x + (z - (z & 1)) / 2;
			// result.y = -z;
			return result;
		}

		/// <summary>
		/// 克隆一个相同的坐标对象
		/// </summary>
		/// <returns></returns>
		public Cubic Clone() {
			return new Cubic(x, y, z);
		}

		public bool Equals(Cubic other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return x == other.x && y == other.y && z == other.z;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Cubic) obj);
		}

		public override int GetHashCode() {
			return HashCode.Combine(x, y, z);
		}
	}
}
