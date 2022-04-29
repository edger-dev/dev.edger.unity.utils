using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity {
    public static class GameObjectExtension {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
            if (go == null) return null;

            T result = go.GetComponent<T>();
            if (result == null) {
                result = go.AddComponent<T>();
            }
            return result;
        }

        public static bool InTree(this GameObject go, GameObject root) {
            if (go == null || root == null) return false;
            Transform rootTrans = root.transform;
            Transform trans = go.transform;
            while (trans != null) {
                if (trans == rootTrans) return true;
                trans = trans.parent;
            }
            return false;
        }
    }
}
