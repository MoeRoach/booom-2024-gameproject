// File create date:2021/8/23
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
[CreateAssetMenu(fileName = "NewRoot", menuName = "ResManage/Text/Root")]
public class TextDataRootObject : ScriptableObject {

	public List<string> databaseNames = new List<string>();

	public List<TextDatabaseObject> textDatabases =
		new List<TextDatabaseObject>();

	public bool AddTextDatabase(string dbName, TextDatabaseObject database) {
		if (database == null) return false;
		if (!databaseNames.Contains(dbName)) {
			textDatabases.Add(database);
			databaseNames.Add(dbName);
			return true;
		}

		Debug.LogWarning(
			$"TEXT: Cannot Add Text Database due to the same name [{dbName}].");

		return false;
	}
	
	public bool CheckDatabase(string dbName) {
		return databaseNames.Contains(dbName);
	}

	public void RemoveDatabase(string dbName) {
		if (!CheckDatabase(dbName)) return;
		var index = databaseNames.IndexOf(dbName);
		databaseNames.RemoveAt(index);
		textDatabases.RemoveAt(index);
	}

	public TextDatabaseObject GetTextDatabase(string dbName) {
		if (!CheckDatabase(dbName)) return null;
		var index = databaseNames.IndexOf(dbName);
		var result = textDatabases[index];
		return result;
	}
}