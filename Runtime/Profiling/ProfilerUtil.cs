using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Remote;

namespace Edger.Unity.Profiling {
    // Note: to keep this called after all other scripts, need to put ProfilerUtil
    // as the last one in "Project Settings -> Script Execution Order"
    // http://docs.unity3d.com/Documentation/Components/class-ScriptExecution.html
    [DisallowMultipleComponent()]
    public partial class ProfilerUtil : BaseMono {
        private static ProfilerUtil _Instance;
        public static ProfilerUtil Instance {
            get {
                if (_Instance == null) {
                    GameObjectUtil.Instance.gameObject.GetOrAddComponent<ProfilerTimer>();
                    _Instance = GameObjectUtil.Instance.gameObject.GetOrAddComponent<ProfilerUtil>();
                }
                return _Instance;
            }
        }

        private GUIStyle _TextStyle;

        public string Stats = "";

        public bool ShowGUI = false;
        public Color Color = Color.yellow;

        public double SlowUpdateSeconds = 0.01;
        public double SlowFixedUpdateSeconds = 0.01;
        public double SlowLateUpdateSeconds = 0.01;

        public void Update() {
            if (ProfilerTimer.CalcUpdateSeconds(SlowUpdateSeconds)) {
                Custom(LoggerConsts.TRACE, "slow_Update: {0:F2} ms", ProfilerTimer.UpdateSeconds * 1000.0);
            }
        }

        public void FixedUpdate() {
            if (ProfilerTimer.CalcFixedUpdateSeconds(SlowFixedUpdateSeconds)) {
                Custom(LoggerConsts.TRACE, "slow_FixedUpdate: {0:F2} ms", ProfilerTimer.FixedUpdateSeconds * 1000.0);
            }
        }

        public void LateUpdate() {
            if (ProfilerTimer.CalcLateUpdateSeconds(SlowLateUpdateSeconds)) {
                Custom(LoggerConsts.TRACE, "slow_LateUpdate: {0:F2} ms", ProfilerTimer.LateUpdateSeconds * 1000.0);
            }
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