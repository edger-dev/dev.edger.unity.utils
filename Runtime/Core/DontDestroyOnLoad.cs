using System;
using System.Collections.Generic;

using UnityEngine;

namespace Edger.Unity {
    [DisallowMultipleComponent()]
    public class DontDestroyOnLoad : BaseMono {
        private static Dictionary<string, DontDestroyOnLoad> _Instances = new Dictionary<string, DontDestroyOnLoad>();

        private static bool AddInstance(DontDestroyOnLoad instance) {
            if (instance.transform.parent != null) {
                instance.Error("AddInstance() failed: is not root");
            } else if (string.IsNullOrEmpty(instance.name)) {
                instance.Error("AddInstance() failed: invalid name");
            } else if (_Instances.ContainsKey(instance.name)) {
                instance.Error("AddInstance() failed: already exist");
            } else {
                _Instances[instance.name] = instance;
                return true;
            }
            return false;
        }

        protected override void OnAwake() {
            if (AddInstance(this)) {
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            } else {
                gameObject.SetActive(false);
                GameObjectUtil.Destroy(gameObject);
            }
        }
    }
}

