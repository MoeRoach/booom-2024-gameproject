// File create date:2021/6/25
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 瓦片调色盘对象
/// </summary>
[CreateAssetMenu(fileName = "NewPalette", menuName = "ResManage/Tiles/Palette")]
public class TilesPaletteObject : ScriptableObject  {
	
	public string paletteName;
	public List<string> groupNames = new List<string>();
	public List<TilesGroupData> tilesGroups = new List<TilesGroupData>();
	// Editor编辑界面所需数据
	public bool isExpanded = false;
	
	public bool AddTilesGroup(string grpName, TilesGroupData groupData) {
		if (groupData == null) return false;
		if (!groupNames.Contains(grpName)) {
			tilesGroups.Add(groupData);
			groupNames.Add(grpName);
			return true;
		}

		Debug.LogWarning($"TILES: Cannot Add Tiles Group Data due to the same name [{grpName}].");
		return false;
	}

	public bool CheckGroupData(string grpName) {
		return groupNames.Contains(grpName);
	}

	public void RemoveGroupData(string grpName) {
		if (!CheckGroupData(grpName)) return;
		var index = groupNames.IndexOf(grpName);
		groupNames.RemoveAt(index);
		tilesGroups.RemoveAt(index);
	}

	public TilesGroupData GetTilesGroupData(string grpName) {
		if (!CheckGroupData(grpName)) return null;
		var index = groupNames.IndexOf(grpName);
		var result = tilesGroups[index];
		return result;
	}
}