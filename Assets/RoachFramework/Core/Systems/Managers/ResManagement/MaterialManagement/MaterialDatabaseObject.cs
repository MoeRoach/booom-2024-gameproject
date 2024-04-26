// File create date:2021/12/11
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 材质数据库对象
/// </summary>
[CreateAssetMenu(fileName = "NewDatabase", menuName = "ResManage/Material/Database")]
public class MaterialDatabaseObject : ScriptableObject  {
	
	public string databaseName;
	public List<string> groupNames = new List<string>();
	public List<MaterialGroupData> materialGroups = new List<MaterialGroupData>();
	// Editor编辑界面所需数据
	public bool isExpanded = false;
	
	public bool AddMaterialGroup(string grpName, MaterialGroupData groupData) {
		if (groupData == null) return false;
		if (!groupNames.Contains(grpName)) {
			materialGroups.Add(groupData);
			groupNames.Add(grpName);
			return true;
		}

		Debug.LogWarning($"MATERIAL: Cannot Add Material Group Data due to the same name [{grpName}].");
		return false;
	}

	public bool CheckGroupData(string grpName) {
		return groupNames.Contains(grpName);
	}

	public void RemoveGroupData(string grpName) {
		if (!CheckGroupData(grpName)) return;
		var index = groupNames.IndexOf(grpName);
		groupNames.RemoveAt(index);
		materialGroups.RemoveAt(index);
	}

	public MaterialGroupData GetMaterialGroupData(string grpName) {
		if (!CheckGroupData(grpName)) return null;
		var index = groupNames.IndexOf(grpName);
		var result = materialGroups[index];
		return result;
	}
}