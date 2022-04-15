using System;

using UnityEngine;

namespace Edger.Unity {
    [DisallowMultipleComponent()]
    public class DontDestroyOnLoad : BaseMono {
        protected override void OnAwake() {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }
    }
}

