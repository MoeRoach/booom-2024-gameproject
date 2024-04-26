// File create date:2021/8/23
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
[CreateAssetMenu(fileName = "NewDatabase", menuName = "ResManage/Text/Database")]
public class TextDatabaseObject : ScriptableObject {

	public string databaseName;
	public List<string> dataNames = new List<string>();
	public List<TextData> textDatas = new List<TextData>();

	public bool isExpanded = false;
	
	public bool AddTextData(string name, TextData textData) {
		if (textData != null) {
			if (!dataNames.Contains(name)) {
				textDatas.Add(textData);
				dataNames.Add(name);
				return true;
			}
			Debug.LogWarning($"TEXT: Cannot Add Text Data due to the same name [{name}].");
		}

		return false;
	}
	
	public bool CheckTextData(string name) {
		return dataNames.Contains(name);
	}

	public void RemoveTextData(string name) {
		if (CheckTextData(name)) {
			var index = dataNames.IndexOf(name);
			dataNames.RemoveAt(index);
			textDatas.RemoveAt(index);
		}
	}

	public TextData GetTextData(string name) {
		TextData result = null;
		if (CheckTextData(name)) {
			var index = dataNames.IndexOf(name);
			result = textDatas[index];
		}
		return result;
	}
}