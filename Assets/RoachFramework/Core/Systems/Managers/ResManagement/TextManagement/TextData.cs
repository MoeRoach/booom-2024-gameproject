// File create date:2021/8/23
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 字符串数据对象
	/// </summary>
	[System.Serializable]
	public class TextData {

		public string dataName;
		public string fileName;
		public TextAsset textAsset;

		public TextData(string dn, string fn, TextAsset asset) {
			dataName = dn;
			fileName = fn;
			textAsset = asset;
		}
	}

	/// <summary>
	/// 字符串数据配置
	/// </summary>
	public static class TextConfigs {
		public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
		public const string FOLDER_TEXT_DATA = "TextData";
		public const string FOLDER_TEXT_DATABASE = "TextDatabase";
		public const string URI_TEXT_DATA_ROOT = "TextData/TextDataRoot";
		public const string FILE_TEXT_DATA_ROOT = "TextDataRoot";
		public const string PREFIX_SPRITE_DATABASE = "TDB_";
	}
}
