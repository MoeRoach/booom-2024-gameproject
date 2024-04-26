using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
	/// 正方形网格坐标辅助方法类
	/// </summary>
	public static class CoordinateUtils {
		
		public const int SQR_DIR_LEFT = 0;
		public const int SQR_DIR_LEFT_TOP = 1;
		public const int SQR_DIR_TOP = 2;
		public const int SQR_DIR_RIGHT_TOP = 3;
		public const int SQR_DIR_RIGHT = 4;
		public const int SQR_DIR_RIGHT_BOT = 5;
		public const int SQR_DIR_BOT = 6;
		public const int SQR_DIR_LEFT_BOT = 7;

		public static string CompileSquare(int x, int y) {
			return $"{x},{y}";
		}

		public static Square DecompileSquare(string id) {
			if (string.IsNullOrEmpty(id)) {
				return new Square();
			}

			var split = id.Split(',');
			var x = int.Parse(split[0]);
			var y = int.Parse(split[1]);
			return new Square(x, y);
		}

		/// <summary>
		/// 获取反向索引
		/// </summary>
		/// <param name="dir">参照方向</param>
		/// <returns>反向索引值</returns>
		public static int GetReverseDir(int dir) {
			dir += 4;
			if (dir >= 8) {
				dir -= 8;
			}

			return dir;
		}
		
		public static Square CoordinateToSquare(int x, int y) {
			return new Square(x, y);
		}

		public static Square CoordinateToSquare(Vector2Int coord) {
			return CoordinateToSquare(coord.x, coord.y);
		}

		/// <summary>
		/// 获取两个平面坐标点之间的距离
		/// </summary>
		/// <param name="src">起始点</param>
		/// <param name="dst">终点</param>
		/// <returns>距离值</returns>
		public static int GetSquareDistance(Square src, Square dst) {
			var delta = dst - src;
			return (Mathf.Abs(delta.x) + Mathf.Abs(delta.y)) / 2;
		}

		/// <summary>
		/// 检查两个单元的邻接关系
		/// </summary>
		/// <param name="a">A单元</param>
		/// <param name="b">B单元</param>
		/// <returns>表示B单元为A单元的何种邻居，不是邻居的返回-1</returns>
		public static int CheckUnitAdjcent(Square a, Square b) {
			var xDelta = b.x - a.x;
			var yDelta = b.y - a.y;
			if (Mathf.Abs(xDelta) > 1 || Mathf.Abs(yDelta) > 1) {
				// 表示间隔至少一个格子，非邻居
				return -1;
			}

			return xDelta switch {
				-1 when yDelta == 0 => SQR_DIR_LEFT,
				-1 when yDelta == 1 => SQR_DIR_LEFT_TOP,
				-1 when yDelta == -1 => SQR_DIR_LEFT_BOT,
				0 when yDelta == 1 => SQR_DIR_TOP,
				0 when yDelta == -1 => SQR_DIR_BOT,
				1 when yDelta == 0 => SQR_DIR_RIGHT,
				1 when yDelta == 1 => SQR_DIR_RIGHT_TOP,
				1 when yDelta == -1 => SQR_DIR_RIGHT_BOT,
				_ => -1
			};
		}

		/// <summary>
		/// 检查两个坐标之间的方向关系
		/// </summary>
		/// <param name="src">源坐标</param>
		/// <param name="dst">目的坐标</param>
		/// <returns>方向</returns>
		public static int CheckCoordinateDir(Square src, Square dst) {
			var xDelta = dst.x - src.x;
			var yDelta = dst.y - src.y;
			switch (xDelta) {
				case 0 when yDelta == 0:
					return -1;
				case 0:
					return yDelta > 0 ? SQR_DIR_TOP : SQR_DIR_BOT;
			}

			if (yDelta == 0) {
				return xDelta > 0 ? SQR_DIR_RIGHT : SQR_DIR_LEFT;
			}

			if (xDelta > 0) {
				if (yDelta > 0) {
					return xDelta >= yDelta ? SQR_DIR_RIGHT : SQR_DIR_TOP;
				}

				return xDelta > Mathf.Abs(yDelta) ? SQR_DIR_RIGHT : SQR_DIR_BOT;
			}

			if (yDelta > 0) {
				return Mathf.Abs(xDelta) > yDelta ? SQR_DIR_LEFT : SQR_DIR_TOP;
			}

			return Mathf.Abs(xDelta) >= Mathf.Abs(yDelta) ? SQR_DIR_LEFT : SQR_DIR_BOT;
		}

		public static Square MoveNextSquare(Square origin, int dir) {
			var result = new Square(origin.x, origin.y);
			switch (dir) {
				case SQR_DIR_LEFT:
					result.x += -1;
					result.y += 0;
					break;
				case SQR_DIR_LEFT_TOP:
					result.x += -1;
					result.y += 1;
					break;
				case SQR_DIR_TOP:
					result.x += 0;
					result.y += 1;
					break;
				case SQR_DIR_RIGHT_TOP:
					result.x += 1;
					result.y += 1;
					break;
				case SQR_DIR_RIGHT:
					result.x += 1;
					result.y += 0;
					break;
				case SQR_DIR_RIGHT_BOT:
					result.x += 1;
					result.y += -1;
					break;
				case SQR_DIR_BOT:
					result.x += 0;
					result.y += -1;
					break;
				case SQR_DIR_LEFT_BOT:
					result.x += -1;
					result.y += -1;
					break;
			}

			return result;
		}
		
		public static Square MoveDistance(Square src, int dir, int dist) {
			var offset = new Square();
			var halfDist = dist / 2;
			switch (dir) {
				case SQR_DIR_LEFT:
					offset.x = -dist;
					break;
				case SQR_DIR_LEFT_TOP:
					offset.x = -halfDist;
					offset.y = dist - halfDist;
					break;
				case SQR_DIR_TOP:
					offset.y = dist;
					break;
				case SQR_DIR_RIGHT_TOP:
					offset.x = halfDist;
					offset.y = dist - halfDist;
					break;
				case SQR_DIR_RIGHT:
					offset.x = dist;
					break;
				case SQR_DIR_RIGHT_BOT:
					offset.x = halfDist;
					offset.y = -(dist - halfDist);
					break;
				case SQR_DIR_BOT:
					offset.y += -dist;
					break;
				case SQR_DIR_LEFT_BOT:
					offset.x = -halfDist;
					offset.y = -(dist - halfDist);
					break;
			}

			return src + offset;
		}

		public static bool CheckDirDiagonal(int dir) {
			return dir == SQR_DIR_LEFT_TOP || dir == SQR_DIR_LEFT_BOT || dir ==
				SQR_DIR_RIGHT_TOP || dir == SQR_DIR_RIGHT_BOT;
		}
		
		public static Vector3 SquareToWorld(Square sq) {
			return SquareToWorld(sq.x, sq.y);
		}
	
		public static Vector3 SquareToWorld(int x, int y) {
			return new Vector3(x + 0.5f, y + 0.5f, 0f);
		}

		public static Vector3 SquareToTilemap(Square sq) {
			return SquareToTilemap(sq.x, sq.y);
		}
	
		public static Vector3 SquareToTilemap(int x, int y) {
			return new Vector3(x, y, 0f);
		}

		public static Square WorldToSquare(Vector3 pos) {
			return new Square(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
		}

		public static Vector3Int SquareToTileCoord(Square sq) {
			return new Vector3Int(sq.x, sq.y, 0);
		}
	}
	
	/// <summary>
	/// 六边形三轴坐标系辅助方法
	/// </summary>
	public static class HexagonUtils {
		
		public const int HEX_DIR_LEFT = 0;
		public const int HEX_DIR_LEFT_TOP = 1;
		public const int HEX_DIR_RIGHT_TOP = 2;
		public const int HEX_DIR_RIGHT = 3;
		public const int HEX_DIR_RIGHT_BOT = 4;
		public const int HEX_DIR_LEFT_BOT = 5;

		public static string CompileCubic(Cubic c) {
			return CompileCubic(c.x, c.y, c.z);
		}

		public static Cubic DecompileCubic(string id) {
			if (string.IsNullOrEmpty(id)) {
				return new Cubic();
			}

			var split = id.Split(',');
			var x = int.Parse(split[0]);
			var y = int.Parse(split[1]);
			var z = int.Parse(split[2]);
			return new Cubic(x, y, z);
		}
		
		public static string CompileCubic(int x, int y, int z) {
			return $"{x},{y},{z}";
		}
		
		public static string CompileCoordinate(Vector2Int v) {
			return CompileCoordinate(v.x, v.y);
		}

		public static string CompileCoordinate(int x, int y) {
			return $"{x},{y}";
		}
		
		/// <summary>
		/// 获取反向索引
		/// </summary>
		/// <param name="dir">参照方向</param>
		/// <returns>反向索引值</returns>
		public static int GetReverseDir(int dir) {
			dir += 3;
			if (dir >= 6) {
				dir -= 6;
			}
			return dir;
		}

		/// <summary>
		/// 平面坐标转化为三轴坐标
		/// </summary>
		/// <param name="x">X坐标值</param>
		/// <param name="y">Y坐标值</param>
		/// <returns>三轴坐标值</returns>
		public static Cubic CoordinateToCubic(int x, int y) {
			var result = new Cubic();
			result.From(x, y);
			return result;
		}

		/// <summary>
		/// 三轴坐标转化为平面坐标
		/// </summary>
		/// <param name="x">X坐标值</param>
		/// <param name="y">Y坐标值</param>
		/// <param name="z">Z坐标值</param>
		/// <returns>平面坐标值</returns>
		public static Vector2Int CubicToCoordinate(int x, int y, int z) {
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
		/// 获取两个三轴坐标点之间的距离
		/// </summary>
		/// <param name="src">起始点</param>
		/// <param name="dst">终点</param>
		/// <returns>距离值</returns>
		public static int GetCubicDistance(Cubic src, Cubic dst) {
			var delta = dst - src;
			return (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) + Mathf.Abs(delta.z)) / 2;
		}
		
		/// <summary>
		/// 检查两个单元的邻接关系
		/// </summary>
		/// <param name="a">A单元</param>
		/// <param name="b">B单元</param>
		/// <returns>表示B单元为A单元的何种邻居，不是邻居的返回-1</returns>
		public static int CheckUnitAdjcent(Cubic a, Cubic b) {
			var xDelta = b.x - a.x;
			var yDelta = b.y - a.y;
			var zDelta = b.z - a.z;
			if (Mathf.Abs(xDelta) > 1 || Mathf.Abs(yDelta) > 1 || Mathf.Abs(zDelta) > 1) {
				// 表示间隔至少一个格子，非邻居
				return -1;
			}

			return xDelta switch {
				-1 when yDelta == 1 && zDelta == 0 => HEX_DIR_LEFT,
				-1 when yDelta == 0 && zDelta == 1 => HEX_DIR_LEFT_BOT,
				0 when yDelta == 1 && zDelta == -1 => HEX_DIR_LEFT_TOP,
				0 when yDelta == -1 && zDelta == 1 => HEX_DIR_RIGHT_BOT,
				1 when yDelta == 0 && zDelta == -1 => HEX_DIR_RIGHT_TOP,
				1 when yDelta == -1 && zDelta == 0 => HEX_DIR_RIGHT,
				_ => -1
			};
		}

		public static Cubic MoveNextCubic(Cubic origin, int dir, int distance = 1) {
			var result = new Cubic(origin.x, origin.y, origin.z);
			switch (dir) {
				case HEX_DIR_LEFT:
					result.x += -1 * distance;
					result.y += 1 * distance;
					result.z += 0;
					break;
				case HEX_DIR_LEFT_TOP:
					result.x += 0;
					result.y += 1 * distance;
					result.z += -1 * distance;
					break;
				case HEX_DIR_RIGHT_TOP:
					result.x += 1 * distance;
					result.y += 0;
					result.z += -1 * distance;
					break;
				case HEX_DIR_RIGHT:
					result.x += 1 * distance;
					result.y += -1 * distance;
					result.z += 0;
					break;
				case HEX_DIR_RIGHT_BOT:
					result.x += 0;
					result.y += -1 * distance;
					result.z += 1 * distance;
					break;
				case HEX_DIR_LEFT_BOT:
					result.x += -1 * distance;
					result.y += 0;
					result.z += 1 * distance;
					break;
			}

			return result;
		}

		/// <summary>
		/// 获取任意两点间的方向关系，仅识别六个正向
		/// </summary>
		/// <param name="src">起始点</param>
		/// <param name="dst">终点</param>
		/// <returns>方向，非正向则返回-1</returns>
		public static int GetRelativeDir(Cubic src, Cubic dst) {
			var delta = dst - src;
			if (delta.x == 0) {
				// 左上和右下
				return delta.y > 0 ? HEX_DIR_LEFT_TOP : HEX_DIR_RIGHT_BOT;
			}

			if (delta.y == 0) {
				// 右上和左下
				return delta.x > 0 ? HEX_DIR_RIGHT_TOP : HEX_DIR_LEFT_BOT;
			}

			if (delta.z == 0) {
				// 左和右
				return delta.y > 0 ? HEX_DIR_LEFT : HEX_DIR_RIGHT;
			}

			return -1;
		}
	}
}
