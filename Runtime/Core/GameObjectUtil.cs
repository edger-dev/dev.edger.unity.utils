using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity.Utils {
    public partial class GameObjectUtil: MonoBehaviour {
        private static GameObjectUtil _Instance;
        public static GameObjectUtil Instance {
            get {
                if (_Instance == null) {
                    GameObject go = GameObjectUtil.GetOrSpawnRoot("_EdgerUtils_");
                    UnityEngine.Object.DontDestroyOnLoad(go);

                    _Instance = go.AddComponent<GameObjectUtil>();
                }
                return _Instance;
            }
        }
        public static GameObject GetOrSpawnRoot(string name) {
            GameObject result = GameObject.Find("/" + name);
            if (result == null) {
                result = new GameObject(name);
            }
            return result;
        }
    }
}
