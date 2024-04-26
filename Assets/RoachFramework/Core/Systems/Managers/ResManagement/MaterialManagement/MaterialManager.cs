// File create date:2021/12/11
using System;
using System.Collections;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 材质管理器
	/// </summary>
	public class MaterialManager : BaseSingleton<MaterialManager> {
		
		private bool _isMaterialDataReady;
		private readonly string _uriMaterialDataRoot;
		private MaterialDataRootObject _rootObject;

		private MaterialManager() {
			_uriMaterialDataRoot = MaterialConfigs.URI_MATERIAL_DATA_ROOT;
		}
		
		protected override void OnInitialized() { }
		
		/// <summary>
		/// 材质数据库同步加载
		/// </summary>
		public void LoadMaterialData() {
			_rootObject = Resources.Load<MaterialDataRootObject>(_uriMaterialDataRoot);
			_isMaterialDataReady = _rootObject != null;
		}

		/// <summary>
		/// 材质数据库异步加载
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadMaterialDataAsync() {
			var request = Resources.LoadAsync<MaterialDataRootObject>(_uriMaterialDataRoot);
			yield return request;
			_rootObject = request.asset as MaterialDataRootObject;
			_isMaterialDataReady = _rootObject != null;
		}

		/// <summary>
		/// 获取指定材质实例
		/// </summary>
		/// <param name="dbName">数据库名称</param>
		/// <param name="name">材质名称</param>
		/// <param name="index">材质索引</param>
		/// <returns>材质实例</returns>
		public Material GetMaterial(string dbName, string name, int index = -1) {
			if (!_isMaterialDataReady || string.IsNullOrEmpty(dbName) ||
			    string.IsNullOrEmpty(name)) return null;
			var db = _rootObject.GetMaterialDatabase(dbName);
			if (db == null) return null;
			var grp = db.GetMaterialGroupData(name);
			if (grp == null) return null;
			return index >= 0 ? grp.GetMaterial(index) : grp.GetRandomMaterial();
		}
	}
}