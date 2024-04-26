// File create date:2021/12/11
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {

	[System.Serializable]
	public class MaterialGroupData {
		
		public string groupName;
		public List<MaterialData> groupData;
		// Editor编辑界面所需数据
		public bool isExpanded = false;

		public int DataCount => groupData.Count;

		public MaterialGroupData(string name) {
			groupName = name;
			groupData = new List<MaterialData>();
		}

		public void PutSpriteData(string dataName, string spriteName, Material
			obj) {
			var data = new MaterialData(dataName, spriteName, obj);
			groupData.Add(data);
		}

		public Material GetRandomMaterial() {
			if (groupData.Count > 0) {
				var index = Random.Range(0, groupData.Count);
				return groupData[index].materialObject;
			}
			return null;
		}

		public Material GetMaterial(int index) {
			if (groupData.Count > index) {
				return groupData[index].materialObject;
			}

			return null;
		}
	}
	
	[System.Serializable]
	public class MaterialData {
		
		public string dataName;
		public string materialName;
		public Material materialObject;

		public MaterialData(string dn, string mn, Material obj) {
			dataName = dn;
			materialName = mn;
			materialObject = obj;
		}
	}
	
	/// <summary>
	/// 材质数据配置
	/// </summary>
	public static class MaterialConfigs {
		public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
		public const string FOLDER_MATERIAL_DATA = "MaterialData";
		public const string FOLDER_MATERIAL_DATABASE = "MaterialDatabase";
		public const string URI_MATERIAL_DATA_ROOT = "MaterialData/MaterialDataRoot";
		public const string FILE_MATERIAL_DATA_ROOT = "MaterialDataRoot";
		public const string PREFIX_MATERIAL_DATABASE = "MDB_";
	}
}