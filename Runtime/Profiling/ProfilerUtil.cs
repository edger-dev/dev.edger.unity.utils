using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity.Profiling {
    public partial class ProfilerUtil : BaseMono {
        private static ProfilerUtil _Instance;
        public static ProfilerUtil Instance {
            get {
                if (_Instance == null) {
                    _Instance = GameObjectUtil.Instance.gameObject.GetOrAddComponent<ProfilerUtil>();
                }
                return _Instance;
            }
        }

        private GUIStyle TextStyle;

        public string Stats = "";

        public bool ShowGUI = false;
        public Color Color = Color.yellow;

        public void Update() {
            if (ShowGUI) {
                Stats = CalcStats(true);
            }
            if (_UdpClient != null) {
                _UdpClient.SendData(CalcStats(false));
            }
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