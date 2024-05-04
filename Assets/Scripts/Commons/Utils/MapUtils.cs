// File create date:2024/4/22
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public static class MapUtils {
	
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
	
	public static Square MoveToDistance(Square src, int dir, int dist) {
		var offset = new Square();
		var halfDist = dist / 2;
		switch (dir) {
			case CoordinateUtils.SQR_DIR_LEFT:
				offset.x = -dist;
				break;
			case CoordinateUtils.SQR_DIR_LEFT_TOP:
				offset.x = -halfDist;
				offset.y = dist - halfDist;
				break;
			case CoordinateUtils.SQR_DIR_TOP:
				offset.y = dist;
				break;
			case CoordinateUtils.SQR_DIR_RIGHT_TOP:
				offset.x = halfDist;
				offset.y = dist - halfDist;
				break;
			case CoordinateUtils.SQR_DIR_RIGHT:
				offset.x = dist;
				break;
			case CoordinateUtils.SQR_DIR_RIGHT_BOT:
				offset.x = halfDist;
				offset.y = -(dist - halfDist);
				break;
			case CoordinateUtils.SQR_DIR_BOT:
				offset.y += -dist;
				break;
			case CoordinateUtils.SQR_DIR_LEFT_BOT:
				offset.x = -halfDist;
				offset.y = -(dist - halfDist);
				break;
		}

		return src + offset;
	}
	
	public static string CompileSquareWithOriginDir(int x, int y, Square o, int dir) {
		var sq = new Square(x, y);
		if (dir == CoordinateUtils.SQR_DIR_RIGHT) {
			sq += o;
		} else if (dir == CoordinateUtils.SQR_DIR_TOP) {
			sq = new Square(sq.y, sq.x);
			sq += o;
		} else if (dir == CoordinateUtils.SQR_DIR_LEFT) {
			sq = new Square(-sq.x, sq.y);
			sq += o;
		} else {
			sq = new Square(sq.y, -sq.x);
			sq += o;
		}

		return sq.Sid;
	}

	public static int GetDirectionWithVector(Vector3 delta) {
		if (delta.x > 0f) {
			return CoordinateUtils.SQR_DIR_RIGHT;
		}

		if (delta.x < 0) {
			return CoordinateUtils.SQR_DIR_LEFT;
		}

		if (delta.y > 0f) {
			return CoordinateUtils.SQR_DIR_TOP;
		}

		return CoordinateUtils.SQR_DIR_BOT;
	}
}