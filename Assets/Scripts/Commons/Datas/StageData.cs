// File create date:2024/4/21
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class StageData {
	
	public Square size;
	public Dictionary<string, SquareTile> tiles;
	public List<Square> tileList;
	public SquareTile this[string id] => tiles.TryGetElement(id);
	public SquareTile this[Square c] => tiles.TryGetElement(c.Sid);
	public SquareTile this[int x, int y] => tiles.TryGetElement(CoordinateUtils.CompileSquare(x, y));
	
	public StageData() {
		size = new Square();
		tiles = new Dictionary<string, SquareTile>();
		tileList = new List<Square>();
	}
	
	public void Reset() {
		tiles.Clear();
		tileList.Clear();
	}

	public bool CheckTileExist(Square sq) {
		return tiles.ContainsKey(sq.Sid);
	}

	public bool CheckAreaIntact(Square pivot, Square area) {
		for (var x = 0; x < area.x; x++) {
			for (var y = 0; y < area.y; y++) {
				var sq = new Square(pivot.x + x, pivot.y + y);
				if (!tiles.ContainsKey(sq.Sid)) return false;
			}
		}

		return true;
	}

	public Square RandomTileCoord() {
		var i = NumberUtils.RandomInteger(tileList.Count - 1);
		return tileList[i];
	}
}

/// <summary>
/// 正方形网格瓦片信息
/// </summary>
public class SquareTile {
	
	public Square coord;
	public TileProperty property;
	public TileDecorate decorate;

	public string Sid => coord.Sid;
		
	public SquareTile() {
		coord = new Square();
		property = new TileProperty();
		decorate = new TileDecorate();
	}

	public SquareTile(int x, int y) {
		coord = new Square(x, y);
		property = new TileProperty();
		decorate = new TileDecorate();
	}
}

/// <summary>
/// 网格单元基础属性
/// </summary>
public struct TileProperty {
	public string palette;
	public int surface;
}

public struct TileDecorate {
	public string palette;
	public int index;
}

public class StageEntityData {

	private Dictionary<int, Square> _pawnCoords;
	private Dictionary<string, List<int>> _pawnMap;
	
	private Dictionary<int, Square> _objectCoords;
	private Dictionary<string, List<int>> _objectMap;
	
	private Dictionary<int, Square> _facilityCoords;
	private Dictionary<string, List<int>> _facilityMap;

	public StageEntityData() {
		_pawnCoords = new Dictionary<int, Square>();
		_pawnMap = new Dictionary<string, List<int>>();
		_objectCoords = new Dictionary<int, Square>();
		_objectMap = new Dictionary<string, List<int>>();
		_facilityCoords = new Dictionary<int, Square>();
		_facilityMap = new Dictionary<string, List<int>>();
	}

	public bool CheckCoordOccupy(Square sq) {
		return CheckCoordOccupy(sq.x, sq.y);
	}
	
	public bool CheckCoordOccupy(int x, int y) {
		var tid = CoordinateUtils.CompileSquare(x, y);
		if (_pawnMap.ContainsKey(tid) && _pawnMap[tid].Count > 0) return true;
		if (_objectMap.ContainsKey(tid) && _objectMap[tid].Count > 0) return true;
		if (_facilityMap.ContainsKey(tid) && _facilityMap[tid].Count > 0) return true;
		return false;
	}

	#region Pawn Functions

	public void RegisterPawn(BasePawnController pawn) {
		_pawnCoords[pawn.Id] = pawn.Coord.Clone();
		for (var x = 0; x < pawn.Area.x; x++) {
			for (var y = 0; y < pawn.Area.y; y++) {
				var sid = CoordinateUtils.CompileSquare(pawn.Coord.x + x, pawn.Coord.y + y);
				if (!_pawnMap.ContainsKey(sid)) _pawnMap[sid] = new List<int>();
				_pawnMap[sid].Add(pawn.Id);
			}
		}
	}

	public void UpdatePawn(BasePawnController pawn) {
		if (!_pawnCoords.ContainsKey(pawn.Id)) return;
		var prev = _pawnCoords[pawn.Id];
		var curr = pawn.Coord;
		if (prev.Equals(curr)) return;
		var map = new HashSet<string>();
		for (var x = 0; x < pawn.Area.x; x++) {
			for (var y = 0; y < pawn.Area.y; y++) {
				var prevId = CoordinateUtils.CompileSquare(prev.x + x, prev.y + y);
				var currId = CoordinateUtils.CompileSquare(curr.x + x, curr.y + y);
				map.Add(currId);
				if (!_pawnMap.ContainsKey(prevId)) continue;
				_pawnMap[prevId].Remove(pawn.Id);
			}
		}
		
		foreach (var tile in map) {
			if (!_pawnMap.ContainsKey(tile)) _pawnMap[tile] = new List<int>();
			_pawnMap[tile].Add(pawn.Id);
		}

		_pawnCoords[pawn.Id].x = curr.x;
		_pawnCoords[pawn.Id].y = curr.y;
	}

	public void UnregisterPawn(BasePawnController pawn) {
		if (!_pawnCoords.ContainsKey(pawn.Id)) return;
		_pawnCoords.Remove(pawn.Id);
		for (var x = 0; x < pawn.Area.x; x++) {
			for (var y = 0; y < pawn.Area.y; y++) {
				var sid = CoordinateUtils.CompileSquare(pawn.Coord.x + x, pawn.Coord.y + y);
				var map = _pawnMap.TryGetElement(sid);
				map?.Remove(pawn.Id);
			}
		}
	}

	public bool CheckCoordOccupyByPawn(Square sq) {
		return CheckCoordOccupyByPawn(sq.x, sq.y);
	}
	
	public bool CheckCoordOccupyByPawn(int x, int y) {
		var tid = CoordinateUtils.CompileSquare(x, y);
		if (_pawnMap.ContainsKey(tid) && _pawnMap[tid].Count > 0) return true;
		return false;
	}

	#endregion

	#region Object Functions

	public void RegisterObject(BaseObjectControl obj) {
		_objectCoords[obj.Id] = obj.Coord.Clone();
		foreach (var sq in obj.Area) {
			var sid = CoordinateUtils.CompileSquare(obj.Coord.x + sq.x, obj.Coord.y + sq.y);
			if (!_objectMap.ContainsKey(sid)) _objectMap[sid] = new List<int>();
			_objectMap[sid].Add(obj.Id);
		}
	}

	public void UpdateObject(BaseObjectControl obj) {
		if (!_objectCoords.ContainsKey(obj.Id)) return;
		var prev = _objectCoords[obj.Id];
		var curr = obj.Coord;
		if (prev.Equals(curr)) return;
		var map = new HashSet<string>();
		foreach (var sq in obj.Area) {
			var prevId = CoordinateUtils.CompileSquare(prev.x + sq.x, prev.y + sq.y);
			var currId = CoordinateUtils.CompileSquare(curr.x + sq.x, curr.y + sq.y);
			map.Add(currId);
			if (!_objectMap.ContainsKey(prevId)) continue;
			_objectMap[prevId].Remove(obj.Id);
		}
		
		foreach (var tile in map) {
			if (!_objectMap.ContainsKey(tile)) _objectMap[tile] = new List<int>();
			_objectMap[tile].Add(obj.Id);
		}

		_objectCoords[obj.Id].x = curr.x;
		_objectCoords[obj.Id].y = curr.y;
	}

	public void UnregisterObject(BaseObjectControl obj) {
		if (!_objectCoords.ContainsKey(obj.Id)) return;
		_objectCoords.Remove(obj.Id);
		foreach (var sq in obj.Area) {
			var sid = CoordinateUtils.CompileSquare(obj.Coord.x + sq.x, obj.Coord.y + sq.y);
			var map = _objectMap.TryGetElement(sid);
			map?.Remove(obj.Id);
		}
	}
	
	public bool CheckCoordOccupyByObject(Square sq) {
		return CheckCoordOccupyByObject(sq.x, sq.y);
	}
	
	public bool CheckCoordOccupyByObject(int x, int y) {
		var tid = CoordinateUtils.CompileSquare(x, y);
		if (_objectMap.ContainsKey(tid) && _objectMap[tid].Count > 0) return true;
		return false;
	}

	#endregion
	
	#region Facility Functions

	public void RegisterFacility(BaseFacilityControl fac) {
		_facilityCoords[fac.Id] = fac.Coord.Clone();
		foreach (var sq in fac.Area) {
			var sid = CoordinateUtils.CompileSquare(fac.Coord.x + sq.x, fac.Coord.y + sq.y);
			if (!_facilityMap.ContainsKey(sid)) _facilityMap[sid] = new List<int>();
			_facilityMap[sid].Add(fac.Id);
		}
	}

	public void UpdateFacility(BaseFacilityControl fac) {
		if (!_facilityCoords.ContainsKey(fac.Id)) return;
		var prev = _facilityCoords[fac.Id];
		var curr = fac.Coord;
		if (prev.Equals(curr)) return;
		var map = new HashSet<string>();
		foreach (var sq in fac.Area) {
			var prevId = CoordinateUtils.CompileSquare(prev.x + sq.x, prev.y + sq.y);
			var currId = CoordinateUtils.CompileSquare(curr.x + sq.x, curr.y + sq.y);
			map.Add(currId);
			if (!_facilityMap.ContainsKey(prevId)) continue;
			_facilityMap[prevId].Remove(fac.Id);
		}
		
		foreach (var tile in map) {
			if (!_facilityMap.ContainsKey(tile)) _facilityMap[tile] = new List<int>();
			_facilityMap[tile].Add(fac.Id);
		}

		_facilityCoords[fac.Id].x = curr.x;
		_facilityCoords[fac.Id].y = curr.y;
	}

	public void UnregisterFacility(BaseFacilityControl fac) {
		if (!_facilityCoords.ContainsKey(fac.Id)) return;
		_facilityCoords.Remove(fac.Id);
		foreach (var sq in fac.Area) {
			var sid = CoordinateUtils.CompileSquare(fac.Coord.x + sq.x, fac.Coord.y + sq.y);
			var map = _facilityMap.TryGetElement(sid);
			map?.Remove(fac.Id);
		}
	}
	
	public bool CheckCoordOccupyByFacility(Square sq) {
		return CheckCoordOccupyByFacility(sq.x, sq.y);
	}
	
	public bool CheckCoordOccupyByFacility(int x, int y) {
		var tid = CoordinateUtils.CompileSquare(x, y);
		if (_facilityMap.ContainsKey(tid) && _facilityMap[tid].Count > 0) return true;
		return false;
	}

	#endregion
}