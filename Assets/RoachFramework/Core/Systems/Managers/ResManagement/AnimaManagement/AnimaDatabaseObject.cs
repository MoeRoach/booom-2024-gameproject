// File create date:2023/9/10
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
[CreateAssetMenu(fileName = "NewDatabase", menuName = "ResManage/Animation/Database")]
public class AnimaDatabaseObject : ScriptableObject  {
	
	public string databaseName;
	public List<string> groupNames = new List<string>();
	public List<AnimaGroupData> animaGroups = new List<AnimaGroupData>();
	// Editor编辑界面所需数据
	public bool isExpanded = false;
	
	public bool AddAnimaGroup(string grpName, AnimaGroupData groupData) {
		if (groupData != null) {
			if (!groupNames.Contains(grpName)) {
				animaGroups.Add(groupData);
				groupNames.Add(grpName);
				return true;
			} else {
				Debug.LogWarning($"ANIMA: Cannot Add Anima Group Data due to the same name [{grpName}].");
			}
		}
		return false;
	}

	public bool CheckGroupData(string grpName) {
		return groupNames.Contains(grpName);
	}

	public void RemoveGroupData(string grpName) {
		if (CheckGroupData(grpName)) {
			var index = groupNames.IndexOf(grpName);
			groupNames.RemoveAt(index);
			animaGroups.RemoveAt(index);
		}
	}

	public AnimaGroupData GetAnimaGroupData(string grpName) {
		AnimaGroupData result = null;
		if (CheckGroupData(grpName)) {
			var index = groupNames.IndexOf(grpName);
			result = animaGroups[index];
		}
		return result;
	}
}