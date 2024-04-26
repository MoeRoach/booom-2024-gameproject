﻿// File create date:2021/2/15
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
namespace RoachFramework {
    /// <summary>
    /// 本地资源相关辅助方法
    /// </summary>
    public static class AssetUtils {
        /// <summary>
        /// Returns a reference to a scriptable object of type T with the given fileName at the relative resourcesPath.
        /// <para/> If the asset is not found, one will get created automatically (in the Editor only) 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="resourcesPath"></param>
        /// <param name="saveAssetDatabase"></param>
        /// <param name="refreshAssetDatabase"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetScriptableObject<T>(string fileName, string resourcesPath, bool saveAssetDatabase,
            bool refreshAssetDatabase) where T : ScriptableObject {
            if (string.IsNullOrEmpty(resourcesPath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            resourcesPath = CleanPath(resourcesPath);
            var obj = (T) Resources.Load(fileName, typeof(T));
            if (obj == null) {
                var simpleResourcesPath =
                    resourcesPath.Replace(
                        resourcesPath.Substring(0, resourcesPath.LastIndexOf("Resources", StringComparison.Ordinal)), "");
                simpleResourcesPath = simpleResourcesPath.Replace("Resources", "").Remove(0, 1);
                obj = (T) Resources.Load(Path.Combine(simpleResourcesPath, fileName), typeof(T));
            }

#if UNITY_EDITOR
            if (obj != null) return obj;
            obj = CreateAsset<T>(resourcesPath, fileName, ".asset", saveAssetDatabase, refreshAssetDatabase);
#endif
            return obj;
        }

        public static T GetResource<T>(string resourcesPath, string fileName) where T : ScriptableObject {
            if (string.IsNullOrEmpty(resourcesPath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            resourcesPath = CleanPath(resourcesPath);
            return (T)Resources.Load(resourcesPath + fileName, typeof(T));
        }

        public static string CleanPath(string path) {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!path[^1].Equals(@"\")) path += @"\";
            path = path.Replace(@"\\", @"\");
            path = path.Replace(@"\", "/");
            return path;
        }

#if UNITY_EDITOR
        public static T CreateAsset<T>(string relativePath, string fileName, string extension = ".asset",
            bool saveAssetDatabase = true, bool refreshAssetDatabase = true) where T : ScriptableObject {
            if (string.IsNullOrEmpty(relativePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            relativePath = CleanPath(relativePath);
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, relativePath + fileName + extension);
            EditorUtility.SetDirty(asset);
            if (saveAssetDatabase) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return asset;
        }

        public static List<T> GetAssets<T>() where T : ScriptableObject {
            var list = new List<T>();
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            foreach (var guid in guids) {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset == null) continue;
                list.Add(asset);
            }

            return list;
        }

        public static void MoveAssetToTrash(string relativePath, string fileName, bool saveAssetDatabase = true,
            bool refreshAssetDatabase = true, bool printDebugMessage = true) {
            if (string.IsNullOrEmpty(relativePath)) return;
            if (string.IsNullOrEmpty(fileName)) return;
            relativePath = CleanPath(relativePath);
            if (!AssetDatabase.MoveAssetToTrash(relativePath + fileName + ".asset")) return;
            if (printDebugMessage) Debug.Log("The " + fileName + ".asset file has been moved to trash.");
            if (saveAssetDatabase) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
        }

        public static Texture GetTexture(string filePath, string fileName, string fileExtension = ".png") {
            if (string.IsNullOrEmpty(filePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            filePath = CleanPath(filePath);
            return AssetDatabase.LoadAssetAtPath<Texture>(filePath + fileName + fileExtension);
        }

        public static Texture2D GetTexture2D(string filePath, string fileName, string fileExtension = ".png") {
            if (string.IsNullOrEmpty(filePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            filePath = CleanPath(filePath);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(filePath + fileName + fileExtension);
        }
#endif
    }
}
