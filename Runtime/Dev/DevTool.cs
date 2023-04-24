using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity.Dev {
    [DisallowMultipleComponent()]
    public partial class DevTool: BaseMono, ISingleton {
        private static DevTool _Instance;
        public static DevTool Instance { get => Singleton.GetInstance(ref _Instance); }

        protected override void OnAwake() {
            if (Singleton.SetupInstance(ref _Instance, this)) {
            }
        }
    }
}
