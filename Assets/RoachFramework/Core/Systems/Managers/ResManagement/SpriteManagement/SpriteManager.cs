// File create date:2021/6/24
using System.Collections;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 图片精灵管理器
	/// </summary>
	public class SpriteManager : BaseSingleton<SpriteManager> {
		
		private bool _isSpriteDataReady;
		private readonly string _uriSpriteDataRoot;
		private SpriteDataRootObject _rootObject;

		private SpriteManager() {
			_uriSpriteDataRoot = SpriteConfigs.URI_SPRITE_DATA_ROOT;
		}
		
		protected override void OnInitialized() { }

		/// <summary>
		/// 图片精灵数据库同步加载
		/// </summary>
		public void LoadSpriteData() {
			_rootObject = Resources.Load<SpriteDataRootObject>(_uriSpriteDataRoot);
			_isSpriteDataReady = _rootObject != null;
		}

		/// <summary>
		/// 图片精灵数据库异步加载
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadSpriteDataAsync() {
			var request = Resources.LoadAsync<SpriteDataRootObject>(_uriSpriteDataRoot);
			yield return request;
			_rootObject = request.asset as SpriteDataRootObject;
			_isSpriteDataReady = _rootObject != null;
		}

		/// <summary>
		/// 获取指定图片精灵
		/// </summary>
		/// <param name="dbName">数据库名称</param>
		/// <param name="name">图片名称</param>
		/// <param name="index">图片索引</param>
		/// <returns>图片精灵</returns>
		public Sprite GetSprite(string dbName, string name, int index = -1) {
			if (!_isSpriteDataReady || string.IsNullOrEmpty(dbName) ||
			    string.IsNullOrEmpty(name)) return null;
			var db = _rootObject.GetSpriteDatabase(dbName);
			if (db == null) return null;
			var grp = db.GetSpriteGroupData(name);
			if (grp == null) return null;
			return index >= 0 ? grp.GetSprite(index) : grp.GetRandomSprite();
		}
	}
}