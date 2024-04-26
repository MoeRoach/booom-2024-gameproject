// File create date:2021/12/11
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 材质数据库根对象
/// </summary>
[CreateAssetMenu(fileName = "NewRoot", menuName = "ResManage/Material/Root")]
public class MaterialDataRootObject : ScriptableObject  {
	
	public List<string> databaseNames = new List<string>();
	public List<MaterialDatabaseObject> spriteDatabases = new List<MaterialDatabaseObject>();

	public bool AddSpriteDatabase(string dbName, MaterialDatabaseObject database) {
		if (database != null) {
			if (!databaseNames.Contains(dbName)) {
				spriteDatabases.Add(database);
				databaseNames.Add(dbName);
				return true;
			} else {
				Debug.LogWarning($"MATERIAL: Cannot Add Material Database due to the same name [{dbName}].");
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

	public MaterialDatabaseObject GetMaterialDatabase(string dbName) {
		MaterialDatabaseObject result = null;
		if (CheckDatabase(dbName)) {
			var index = databaseNames.IndexOf(dbName);
			result = spriteDatabases[index];
		}
		return result;
	}
}