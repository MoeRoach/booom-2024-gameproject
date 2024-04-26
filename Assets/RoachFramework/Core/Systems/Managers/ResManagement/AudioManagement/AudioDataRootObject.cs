// File create date:2021/2/15
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
[CreateAssetMenu(fileName = "NewRoot", menuName = "ResManage/Audio/Root")]
public class AudioDataRootObject : ScriptableObject {

    public List<string> databaseNames = new List<string>();
    public List<AudioDatabaseObject> audioDatabases = new List<AudioDatabaseObject>();

    public bool AddAudioDatabase(string name, AudioDatabaseObject database) {
        if (database != null) {
            if (!databaseNames.Contains(name)) {
                audioDatabases.Add(database);
                databaseNames.Add(name);
                return true;
            } else {
                Debug.LogWarning($"AUDIO: Cannot Add Audio Database due to the same name [{name}].");
            }
        }
        return false;
    }

    public bool CheckDatabase(string name) {
        return databaseNames.Contains(name);
    }

    public void RemoveDatabase(string name) {
        if (CheckDatabase(name)) {
            var index = databaseNames.IndexOf(name);
            databaseNames.RemoveAt(index);
            audioDatabases.RemoveAt(index);
        }
    }

    public AudioDatabaseObject GetAudioDatabase(string name) {
        AudioDatabaseObject result = null;
        if (CheckDatabase(name)) {
            var index = databaseNames.IndexOf(name);
            result = audioDatabases[index];
        }
        return result;
    }
}
