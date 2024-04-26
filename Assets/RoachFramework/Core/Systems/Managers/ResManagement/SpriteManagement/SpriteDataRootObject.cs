// File create date:2021/6/24
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 图片精灵数据库根对象
/// </summary>
[CreateAssetMenu(fileName = "NewRoot", menuName = "ResManage/Sprite/Root")]
public class SpriteDataRootObject : ScriptableObject  {
	
	public List<string> databaseNames = new List<string>();
	public List<SpriteDatabaseObject> spriteDatabases = new List<SpriteDatabaseObject>();

	public bool AddSpriteDatabase(string dbName, SpriteDatabaseObject database) {
		if (database != null) {
			if (!databaseNames.Contains(dbName)) {
				spriteDatabases.Add(database);
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
			spriteDatabases.RemoveAt(index);
		}
	}

	public SpriteDatabaseObject GetSpriteDatabase(string dbName) {
		SpriteDatabaseObject result = null;
		if (CheckDatabase(dbName)) {
			var index = databaseNames.IndexOf(dbName);
			result = spriteDatabases[index];
		}
		return result;
	}
}