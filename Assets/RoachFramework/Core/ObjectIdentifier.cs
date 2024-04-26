using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 游戏对象标识符
    /// </summary>
    public class ObjectIdentifier : MonoBehaviour {
        public string oid;
        private void Awake() {
            if (string.IsNullOrEmpty(oid)) oid = gameObject.name;
            ObjectManager.Instance.RegisterObject(oid, gameObject);
        }

        private void OnDestroy() {
            ObjectManager.Instance.UnregisterObject(oid);
        }
    }
}
