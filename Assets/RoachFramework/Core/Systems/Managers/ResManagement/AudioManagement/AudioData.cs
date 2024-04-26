using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {

    [System.Serializable]
    public class AudioGroupData {

        public string groupName;
        public float groupVolume;
        public float groupPitch;
        public bool groupLoop;
        public float groupSpatialBlend;
        public int groupPlayMode;
        public List<AudioData> groupData;
        // Editor编辑界面所需数据
        public bool isExpanded = false;

        public int dataCount {
            get {
                return groupData.Count;
            }
        }

        // -- 非储存字段
        public int lastPlayIndex = -1;

        public AudioGroupData(string name) {
            groupName = name;
            groupVolume = 1f;
            groupPitch = 1f;
            groupLoop = false;
            groupSpatialBlend = 0f;
            groupPlayMode = AudioConfigs.ENUM_GROUP_PLAY_MODE_RANDOM;
            groupData = new List<AudioData>();
        }

        public void ResetPlayIndex() {
            lastPlayIndex = -1;
        }

        public int GetRandomIndex() {
            return Random.Range(0, groupData.Count);
        }

        public int MoveNextIndex() {
            if (groupPlayMode == AudioConfigs.ENUM_GROUP_PLAY_MODE_RANDOM) {
                lastPlayIndex = Random.Range(0, groupData.Count);
            } else {
                lastPlayIndex++;
                if (lastPlayIndex >= dataCount) {
                    lastPlayIndex -= dataCount;
                }
            }
            return lastPlayIndex;
        }

        public void PutAudioData(string dataName, string clipName, AudioClip clip) {
            var data = new AudioData(dataName, clipName, clip);
            groupData.Add(data);
        }

        public AudioData GetNextAudioData() {
            if (dataCount > 0) {
                var index = MoveNextIndex();
                return groupData[index];
            }
            return null;
        }
    }

    [System.Serializable]
    public class AudioData {

        public string dataName;
        public string clipName;
        public AudioClip audioClip;

        public AudioData(string dn, string cn, AudioClip clip) {
            dataName = dn;
            clipName = cn;
            audioClip = clip;
        }
    }

    public static class AudioConfigs {
        public const int ENUM_GROUP_PLAY_MODE_RANDOM = 0;
        public const int ENUM_GROUP_PLAY_MODE_SEQUENCE = 1;
        public const int ENUM_GROUP_PLAY_MODE_COMBINED = 2;

        public const string PATH_ASSET_RESOURCE_FOLDER = "Assets/Resources";
        public const string FOLDER_AUDIO_DATA = "AudioData";
        public const string FOLDER_AUDIO_DATABASE = "AudioDatabase";
        public const string URI_AUDIO_DATA_ROOT = "AudioData/AudioDataRoot";
        public const string FILE_AUDIO_DATA_ROOT = "AudioDataRoot";
        public const string PREFIX_AUDIO_DATABASE = "ADB_";
    }
}
