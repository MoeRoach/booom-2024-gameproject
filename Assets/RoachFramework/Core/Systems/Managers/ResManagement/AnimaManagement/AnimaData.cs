// File create date:2023/9/10
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {

	/// <summary>
	/// 图片精灵组数据
	/// </summary>
	[System.Serializable]
	public class AnimaGroupData {
		
		public string groupName;
		public List<AnimaData> groupData;
		// Editor编辑界面所需数据
		public bool isExpanded = false;

		public int DataCount => groupData.Count;

		public AnimaGroupData(string name) {
			groupName = name;
			groupData = new List<AnimaData>();
		}

		public void PutClipData(string dataName, string animaName, AnimationClip clip) {
			var data = new AnimaData(dataName, animaName, clip);
			groupData.Add(data);
		}

		public AnimationClip GetRandomClip() {
			if (groupData.Count <= 0) return null;
			var index = Random.Range(0, groupData.Count);
			return groupData[index].animaClip;
		}

		public AnimationClip GetClip(int index) {
			return groupData.Count > index ? groupData[index].animaClip : null;
		}
	}
	
	/// <summary>
	/// 图片精灵数据
	/// </summary>
	[System.Serializable]
	public class AnimaData {

		public string dataName;
		public string animaName;
		public AnimationClip animaClip;

		public AnimaData(string dn, string an, AnimationClip clip) {
			dataName = dn;
			animaName = an;
			animaClip = clip;
		}
	}

	/// <summary>
	/// 图片精灵配置
	/// </summary>
	public static class AnimaConfigs {
		public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
		public const string FOLDER_ANIMA_DATA = "AnimaData";
		public const string FOLDER_ANIMA_DATABASE = "AnimaDatabase";
		public const string URI_ANIMA_DATA_ROOT = "AnimaData/AnimaDataRoot";
		public const string FILE_ANIMA_DATA_ROOT = "AnimaDataRoot";
		public const string PREFIX_ANIMA_DATABASE = "ADB_";
	}
}