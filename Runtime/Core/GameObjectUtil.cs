using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity {
    [DisallowMultipleComponent()]
    public partial class GameObjectUtil: MonoBehaviour {
        private static GameObjectUtil _Instance;
        public static GameObjectUtil Instance {
            get {
                if (_Instance == null) {
                    GameObject go = GameObjectUtil.GetOrSpawnRoot("_EdgerUtils_");
                    UnityEngine.Object.DontDestroyOnLoad(go);

                    _Instance = go.GetOrAddComponent<GameObjectUtil>();
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
        public static void Destroy(GameObject go) {
            if (go == null) return;
            go.transform.SetParent(null, false);
            if (Application.isPlaying) {
                GameObject.Destroy(go);
            } else {
                GameObject.DestroyImmediate(go);
            }
        }

        public static bool IsEmpty(GameObject go) {
            if (go == null) return false;
            if (go.transform.childCount > 0) return false;
            Component[] allComponents = go.GetComponents<Component>();
            return allComponents.Length <= 1;
        }}
}
