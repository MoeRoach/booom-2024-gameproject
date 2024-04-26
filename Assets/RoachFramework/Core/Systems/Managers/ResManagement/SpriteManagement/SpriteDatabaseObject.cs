// File create date:2021/6/24
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
/// <summary>
/// 图片精灵数据库对象
/// </summary>
[CreateAssetMenu(fileName = "NewDatabase", menuName = "ResManage/Sprite/Database")]
public class SpriteDatabaseObject : ScriptableObject  {
	
	public string databaseName;
	public List<string> groupNames = new List<string>();
	public List<SpriteGroupData> spriteGroups = new List<SpriteGroupData>();
	// Editor编辑界面所需数据
	public bool isExpanded = false;
	
	public bool AddSpriteGroup(string grpName, SpriteGroupData groupData) {
		if (groupData != null) {
			if (!groupNames.Contains(grpName)) {
				spriteGroups.Add(groupData);
				groupNames.Add(grpName);
				return true;
			} else {
				Debug.LogWarning($"SPRITE: Cannot Add Sprite Group Data due to the same name [{grpName}].");
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
		spriteGroups.RemoveAt(index);
	}

	public SpriteGroupData GetSpriteGroupData(string grpName) {
		if (!CheckGroupData(grpName)) return null;
		var index = groupNames.IndexOf(grpName);
		var result = spriteGroups[index];
		return result;
	}
}