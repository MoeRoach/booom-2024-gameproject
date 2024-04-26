// File create date:2023/9/10
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
	/// <summary>
	/// 动画片段管理器
	/// </summary>
	public class AnimaManager : BaseSingleton<AnimaManager> {
		
		private bool _isAnimaDataReady;
		private readonly string _uriAnimaDataRoot;
		private AnimaDataRootObject _rootObject;

		private AnimaManager() {
			_uriAnimaDataRoot = AnimaConfigs.URI_ANIMA_DATA_ROOT;
		}
		
		protected override void OnInitialized() { }

		/// <summary>
		/// 动画片段数据库同步加载
		/// </summary>
		public void LoadAnimaData() {
			_rootObject = Resources.Load<AnimaDataRootObject>(_uriAnimaDataRoot);
			_isAnimaDataReady = _rootObject != null;
		}

		/// <summary>
		/// 动画片段数据库异步加载
		/// </summary>
		/// <returns></returns>
		public IEnumerator LoadAnimaDataAsync() {
			var request = Resources.LoadAsync<AnimaDataRootObject>(_uriAnimaDataRoot);
			yield return request;
			_rootObject = request.asset as AnimaDataRootObject;
			_isAnimaDataReady = _rootObject != null;
		}

		/// <summary>
		/// 获取指定动画片段
		/// </summary>
		/// <param name="dbName">数据库名称</param>
		/// <param name="name">动画名称</param>
		/// <param name="index">动画索引</param>
		/// <returns>动画片段</returns>
		public AnimationClip GetAnima(string dbName, string name, int index = -1) {
			if (!_isAnimaDataReady || string.IsNullOrEmpty(dbName) ||
			    string.IsNullOrEmpty(name)) return null;
			var db = _rootObject.GetAnimaDatabase(dbName);
			if (db == null) return null;
			var grp = db.GetAnimaGroupData(name);
			if (grp == null) return null;
			return index >= 0 ? grp.GetClip(index) : grp.GetRandomClip();
		}
	}
}