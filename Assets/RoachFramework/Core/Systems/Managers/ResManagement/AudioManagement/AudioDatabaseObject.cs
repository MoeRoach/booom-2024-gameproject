// File create date:2021/2/15
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
using UnityEngine.Audio;
// Created By Yu.Liu
[CreateAssetMenu(fileName = "NewDatabase", menuName = "ResManage/Audio/Database")]
public class AudioDatabaseObject : ScriptableObject {

    public string databaseName;
    public AudioMixerGroup databaseMixer;
    public List<string> groupNames = new List<string>();
    public List<AudioGroupData> audioGroups = new List<AudioGroupData>();
    // Editor编辑界面所需数据
    public bool isExpanded = false;

    public bool AddAudioGroup(string grp, AudioGroupData groupData) {
        if (groupData == null) return false;
        if (!groupNames.Contains(grp)) {
            audioGroups.Add(groupData);
            groupNames.Add(grp);
            return true;
        }
        Debug.LogWarning($"AUDIO: Cannot Add Audio Group Data due to the same name [{grp}].");
        return false;
    }

    public bool CheckGroupData(string grp) {
        return groupNames.Contains(grp);
    }

    public void RemoveGroupData(string grp) {
        if (!CheckGroupData(grp)) return;
        var index = groupNames.IndexOf(grp);
        groupNames.RemoveAt(index);
        audioGroups.RemoveAt(index);
    }

    public AudioGroupData GetAudioGroupData(string grp) {
        if (!CheckGroupData(grp)) return null;
        var index = groupNames.IndexOf(grp);
        var result = audioGroups[index];
        return result;
    }
}
