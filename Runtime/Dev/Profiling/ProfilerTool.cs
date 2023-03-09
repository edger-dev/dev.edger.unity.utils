using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Dev;
using Edger.Unity.Dev.Remote;

namespace Edger.Unity.Dev.Profiling {
    // Note: to keep this called after all other scripts, need to put ProfilerTool
    // as the last one in "Project Settings -> Script Execution Order"
    // http://docs.unity3d.com/Documentation/Components/class-ScriptExecution.html
    [DisallowMultipleComponent()]
    public partial class ProfilerTool : BaseMono {
        private static ProfilerTool _Instance;
        public static ProfilerTool Instance {
            get {
                if (_Instance == null) {
                    DevTool.Instance.gameObject.GetOrAddComponent<ProfilerTimer>();
                    _Instance = DevTool.Instance.gameObject.GetOrAddComponent<ProfilerTool>();
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
                _UdpClient.SendData(CalcTimeSeries());
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
        private IRemoteSetting _TargetFrameRate;
        private IRemoteSetting _ToUdp;

        public void OnEnable() {
            _Visible = RemoteTool.Instance.Register("Profiler.Visible", () => {
                return ShowGUI.ToRemote();
            }, (value) => {
                ShowGUI = value.RemoteToBool();
                _TextStyle = null;
            });
            _TargetFrameRate = RemoteTool.Instance.Register("Profiler.TargetFrameRate", () => {
                return Application.targetFrameRate.ToRemote();
            }, (value) => {
                Application.targetFrameRate = value.RemoteToInt();
            });
            _ToUdp = RemoteTool.Instance.Register("Profiler.ToUdp", GetToUdp, SetToUdp);
        }
        public void OnDisable() {
            RemoteTool.Instance.Unregister(ref _Visible);
            RemoteTool.Instance.Unregister(ref _TargetFrameRate);
            RemoteTool.Instance.Unregister(ref _ToUdp);
        }
    }
}