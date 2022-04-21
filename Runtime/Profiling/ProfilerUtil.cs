using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity.Remote;

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

        private GUIStyle _TextStyle;

        public string Stats = "";

        public bool ShowGUI = false;
        public Color Color = Color.yellow;

        public void Update() {
            if (ShowGUI) {
                Stats = CalcStats(true);
            } else {
                _TextStyle = null;
            }
            if (_UdpClient != null && _UdpClient.Connected) {
                _UdpClient.SendData(CalcStats(false));
            }
        }

        public void OnGUI() {
            if (ShowGUI) {
                if (_TextStyle == null) {
                    _TextStyle = ToolGuiHelper.NewTextStyle(Color);
                }
                GUI.Label(ToolGuiHelper.ScreenRect, Stats, _TextStyle);
            }
        }

        private IRemoteSetting _Visible;
        private IRemoteSetting _ToUdp;

        public void OnEnable() {
            _Visible = RemoteUtil.Instance.Register("Profiler.Visible", () => {
                return ShowGUI.ToRemote();
            }, (value) => {
                ShowGUI = value.RemoteToBool();
                _TextStyle = null;
            });
            _ToUdp = RemoteUtil.Instance.Register("Profiler.ToUdp", GetToUdp, SetToUdp);
        }
        public void OnDisable() {
            RemoteUtil.Instance.Unregister(ref _Visible);
            RemoteUtil.Instance.Unregister(ref _ToUdp);
        }
    }
}