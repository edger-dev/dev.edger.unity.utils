using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity.Utils.Profiling {
    public partial class ProfilerUtil : MonoBehaviour {
        private static ProfilerUtil _Instance;
        public static ProfilerUtil Instance {
            get {
                if (_Instance == null) {
                    _Instance = GameObjectUtil.Instance.gameObject.AddComponent<ProfilerUtil>();
                }
                return _Instance;
            }
        }

        private GUIStyle TextStyle;

        public string Stats = "";

        public bool ShowGUI = false;
        public Color Color = Color.yellow;

        public void Update() {
            Stats = CalcStats();
        }

        public void OnGUI() {
            if (ShowGUI) {
                if (TextStyle == null) {
                    TextStyle = ToolGuiHelper.NewTextStyle(Color);
                }
                GUI.Label(ToolGuiHelper.ScreenRect, Stats, TextStyle);
            }
        }
    }
}