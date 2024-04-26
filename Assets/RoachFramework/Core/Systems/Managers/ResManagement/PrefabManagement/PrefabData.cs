// File create date:2021/4/29
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
    /// <summary>
    /// 预制体组数据
    /// </summary>
    [System.Serializable]
    public class PrefabGroupData {

        public string groupName;
        public List<PrefabData> groupData;
        // Editor编辑界面所需数据
        public bool isExpanded = false;

        public int DataCount => groupData.Count;

        public PrefabGroupData(string name) {
            groupName = name;
            groupData = new List<PrefabData>();
        }

        public void PutPrefabData(string dataName, string prefabName, GameObject obj) {
            var data = new PrefabData(dataName, prefabName, obj);
            groupData.Add(data);
        }

        public GameObject GetRandomPrefab() {
            if (groupData.Count <= 0) return null;
            var index = Random.Range(0, groupData.Count);
            return groupData[index].prefabObject;
        }

        public GameObject GetPrefab(int index) {
            return groupData.Count > index ? groupData[index].prefabObject : null;
        }
    }

    /// <summary>
    /// 预制体数据
    /// </summary>
    [System.Serializable]
    public class PrefabData {

        public string dataName;
        public string prefabName;
        public GameObject prefabObject;

        public PrefabData(string dn, string pn, GameObject obj) {
            dataName = dn;
            prefabName = pn;
            prefabObject = obj;
        }
    }

    /// <summary>
    /// 预制体配置
    /// </summary>
    public static class PrefabConfigs {
        public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
        public const string FOLDER_PREFAB_DATA = "PrefabData";
        public const string FOLDER_PREFAB_DATABASE = "PrefabDatabase";
        public const string URI_PREFAB_DATA_ROOT = "PrefabData/PrefabDataRoot";
        public const string FILE_PREFAB_DATA_ROOT = "PrefabDataRoot";
        public const string PREFIX_PREFAB_DATABASE = "PDB_";
    }
}
