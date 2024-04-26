// File create date:2023/9/10
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
[CreateAssetMenu(fileName = "NewRoot", menuName = "ResManage/Anima/Root")]
public class AnimaDataRootObject : ScriptableObject  {
	
	public List<string> databaseNames = new List<string>();
	public List<AnimaDatabaseObject> animaDatabases = new List<AnimaDatabaseObject>();

	public bool AddAnimaDatabase(string dbName, AnimaDatabaseObject database) {
		if (database != null) {
			if (!databaseNames.Contains(dbName)) {
				animaDatabases.Add(database);
				databaseNames.Add(dbName);
				return true;
			} else {
				Debug.LogWarning($"SPRITE: Cannot Add Sprite Database due to the same name [{dbName}].");
			}
		}
		return false;
	}

	public bool CheckDatabase(string dbName) {
		return databaseNames.Contains(dbName);
	}

	public void RemoveDatabase(string dbName) {
		if (CheckDatabase(dbName)) {
			var index = databaseNames.IndexOf(dbName);
			databaseNames.RemoveAt(index);
			animaDatabases.RemoveAt(index);
		}
	}

	public AnimaDatabaseObject GetAnimaDatabase(string dbName) {
		AnimaDatabaseObject result = null;
		if (CheckDatabase(dbName)) {
			var index = databaseNames.IndexOf(dbName);
			result = animaDatabases[index];
		}
		return result;
	}
}