// File create date:2021/4/29
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 预制体数据库根对象
/// </summary>
[CreateAssetMenu(fileName = "NewRoot", menuName = "ResManage/Prefab/Root")]
public class PrefabDataRootObject : ScriptableObject {

    public List<string> databaseNames = new List<string>();
    public List<PrefabDatabaseObject> prefabDatabases = new List<PrefabDatabaseObject>();

    public bool AddPrefabDatabase(string dbName, PrefabDatabaseObject database) {
        if (database != null) {
            if (!databaseNames.Contains(dbName)) {
                prefabDatabases.Add(database);
                databaseNames.Add(dbName);
                return true;
            } else {
                Debug.LogWarning($"PREFAB: Cannot Add Prefab Database due to the same name [{dbName}].");
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
            prefabDatabases.RemoveAt(index);
        }
    }

    public PrefabDatabaseObject GetPrefabDatabase(string dbName) {
        PrefabDatabaseObject result = null;
        if (CheckDatabase(dbName)) {
            var index = databaseNames.IndexOf(dbName);
            result = prefabDatabases[index];
        }
        return result;
    }
}
