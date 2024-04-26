// File create date:2021/6/24
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 图片精灵组数据
	/// </summary>
	[System.Serializable]
	public class SpriteGroupData {
		
		public string groupName;
		public List<SpriteData> groupData;
		// Editor编辑界面所需数据
		public bool isExpanded = false;

		public int DataCount => groupData.Count;

		public SpriteGroupData(string name) {
			groupName = name;
			groupData = new List<SpriteData>();
		}

		public void PutSpriteData(string dataName, string spriteName, Sprite
			obj) {
			var data = new SpriteData(dataName, spriteName, obj);
			groupData.Add(data);
		}

		public Sprite GetRandomSprite() {
			if (groupData.Count <= 0) return null;
			var index = Random.Range(0, groupData.Count);
			return groupData[index].spriteObject;
		}

		public Sprite GetSprite(int index) {
			return groupData.Count > index ? groupData[index].spriteObject : null;
		}
	}
	
	/// <summary>
	/// 图片精灵数据
	/// </summary>
	[System.Serializable]
	public class SpriteData {

		public string dataName;
		public string spriteName;
		public Sprite spriteObject;

		public SpriteData(string dn, string pn, Sprite obj) {
			dataName = dn;
			spriteName = pn;
			spriteObject = obj;
		}
	}

	/// <summary>
	/// 图片精灵配置
	/// </summary>
	public static class SpriteConfigs {
		public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
		public const string FOLDER_SPRITE_DATA = "SpriteData";
		public const string FOLDER_SPRITE_DATABASE = "SpriteDatabase";
		public const string URI_SPRITE_DATA_ROOT = "SpriteData/SpriteDataRoot";
		public const string FILE_SPRITE_DATA_ROOT = "SpriteDataRoot";
		public const string PREFIX_SPRITE_DATABASE = "SDB_";
	}
}