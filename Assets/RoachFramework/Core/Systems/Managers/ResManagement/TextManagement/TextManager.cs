// File create date:2021/8/23
using System.Collections;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 文本资源管理器
	/// </summary>
	public class TextManager : BaseSingleton<TextManager> {
		
		private bool _isTextDataReady;
		private readonly string _uriTextDataRoot;
		private TextDataRootObject _rootObject;

		private TextManager() {
			_uriTextDataRoot = TextConfigs.URI_TEXT_DATA_ROOT;
		}
		
		protected override void OnInitialized() { }

		/// <summary>
		/// 文本数据库同步加载
		/// </summary>
		public void LoadTextData() {
			_rootObject = Resources.Load<TextDataRootObject>(_uriTextDataRoot);
			_isTextDataReady = _rootObject != null;
		}

		/// <summary>
		/// 文本数据库异步加载
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadSpriteDataAsync() {
			var request = Resources.LoadAsync<TextDataRootObject>
			(_uriTextDataRoot);
			yield return request;
			_rootObject = request.asset as TextDataRootObject;
			_isTextDataReady = _rootObject != null;
		}

		/// <summary>
		/// 获取指定文本资源
		/// </summary>
		/// <param name="dbName">数据库名称</param>
		/// <param name="name">文本文件名称</param>
		/// <returns>文本内容</returns>
		public string GetText(string dbName, string name) {
			if (!_isTextDataReady || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(name))
				return string.Empty;
			var db = _rootObject.GetTextDatabase(dbName);
			if (db == null) return string.Empty;
			var data = db.GetTextData(name);
			if (data == null) return string.Empty;
			return data.textAsset != null ? data.textAsset.text : string.Empty;
		}
	}
}
