// File create date:2021/6/25
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 瓦片组数据
	/// </summary>
	[System.Serializable]
	public class TilesGroupData {
		
		public string groupName;
		public List<TilesData> groupData;
		// Editor编辑界面所需数据
		public bool isExpanded = false;

		public int DataCount => groupData.Count;
		
		public TilesGroupData(string name) {
			groupName = name;
			groupData = new List<TilesData>();
		}

		public void PutTileData(string dataName, string tileName, TileBase
			obj) {
			var data = new TilesData(dataName, tileName, obj);
			groupData.Add(data);
		}

		public TileBase GetRandomTile() {
			if (groupData.Count <= 0) return null;
			var index = Random.Range(0, groupData.Count);
			return groupData[index].tileObject;
		}

		public TileBase GetTile(int index) {
			return groupData.Count > index ? groupData[index].tileObject : null;
		}
	}
	
	/// <summary>
	/// 瓦片数据
	/// </summary>
	[System.Serializable]
	public class TilesData {
		
		public string dataName;
		public string tileName;
		public TileBase tileObject;

		public TilesData(string dn, string tn, TileBase obj) {
			dataName = dn;
			tileName = tn;
			tileObject = obj;
		}
	}
	
	/// <summary>
	/// 图片精灵配置
	/// </summary>
	public static class TilesConfigs {
		public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
		public const string FOLDER_TILES_DATA = "TilesData";
		public const string FOLDER_TILES_PALETTE = "TilesPalette";
		public const string URI_TILES_DATA_ROOT = "TilesData/TilesDataRoot";
		public const string FILE_TILES_DATA_ROOT = "TilesDataRoot";
		public const string PREFIX_TILES_PALETTE = "TPL_";
	}
}