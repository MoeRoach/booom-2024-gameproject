// File create date:2021/6/25
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 瓦片管理器
	/// </summary>
	public class TilesManager : BaseSingleton<TilesManager> {
		
		private bool _isTilesDataReady;
		private readonly string _uriTilesDataRoot;
		private TilesDataRootObject _rootObject;

		private TilesManager() {
			_uriTilesDataRoot = TilesConfigs.URI_TILES_DATA_ROOT;
		}
		
		protected override void OnInitialized() { }

		/// <summary>
		/// 瓦片数据库同步加载
		/// </summary>
		public void LoadTilesData() {
			_rootObject = Resources.Load<TilesDataRootObject>(_uriTilesDataRoot);
			_isTilesDataReady = _rootObject != null;
		}

		/// <summary>
		/// 瓦片数据库异步加载
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadTilesDataAsync() {
			var request = Resources.LoadAsync<TilesDataRootObject>(_uriTilesDataRoot);
			yield return request;
			_rootObject = request.asset as TilesDataRootObject;
			_isTilesDataReady = _rootObject != null;
		}

		/// <summary>
		/// 获取指定瓦片
		/// </summary>
		/// <param name="plName">调色盘名称</param>
		/// <param name="name">瓦片名称</param>
		/// <returns>瓦片引用</returns>
		public TileBase GetTile(string plName, string name, int index = -1) {
			if (!_isTilesDataReady || string.IsNullOrEmpty(plName) ||
			    string.IsNullOrEmpty(name)) return null;
			var db = _rootObject.GetTilesPalette(plName);
			if (db == null) return null;
			var grp = db.GetTilesGroupData(name);
			if (grp == null) return null;
			return index >= 0 ? grp.GetTile(index) : grp.GetRandomTile();
		}
	}
}