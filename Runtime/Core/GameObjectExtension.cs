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
    }
}
