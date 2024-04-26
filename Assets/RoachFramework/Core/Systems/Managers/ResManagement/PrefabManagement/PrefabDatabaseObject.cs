// File create date:2021/4/29
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 预制体数据库对象
/// </summary>
[CreateAssetMenu(fileName = "NewDatabase", menuName = "ResManage/Prefab/Database")]
public class PrefabDatabaseObject : ScriptableObject {

    public string databaseName;
    public List<string> groupNames = new List<string>();
    public List<PrefabGroupData> prefabGroups = new List<PrefabGroupData>();
    // Editor编辑界面所需数据
    public bool isExpanded = false;

    public bool AddPrefabGroup(string grpName, PrefabGroupData groupData) {
        if (groupData != null) {
            if (!groupNames.Contains(grpName)) {
                prefabGroups.Add(groupData);
                groupNames.Add(grpName);
                return true;
            } else {
                Debug.LogWarning($"PREFAB: Cannot Add Prefab Group Data due to the same name [{grpName}].");
            }
        }
        return false;
    }

    public bool CheckGroupData(string grpName) {
        return groupNames.Contains(grpName);
    }

    public void RemoveGroupData(string grpName) {
        if (!CheckGroupData(grpName)) return;
        var index = groupNames.IndexOf(grpName);
        groupNames.RemoveAt(index);
        prefabGroups.RemoveAt(index);
    }

    public PrefabGroupData GetPrefabGroupData(string grpName) {
        if (!CheckGroupData(grpName)) return null;
        var index = groupNames.IndexOf(grpName);
        var result = prefabGroups[index];
        return result;
    }
}
