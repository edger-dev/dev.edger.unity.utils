using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Udp;
using Edger.Unity.Dev;

namespace Edger.Unity.Dev.Remote {
    [DisallowMultipleComponent()]
    public partial class RemoteTool : BaseMono {
        public const int DEFAULT_UDP_PORT = 2295;

        private static RemoteTool _Instance;
        public static RemoteTool Instance {
            get {
                if (_Instance == null) {
                    _Instance = DevTool.Instance.gameObject.GetOrAddComponent<RemoteTool>();
                }
                return _Instance;
            }
        }

        private readonly Dictionary<string, IRemoteSetting> _Settings = new Dictionary<string, IRemoteSetting>();

        public int Port = DEFAULT_UDP_PORT;

        private UdpServer _Server;

        public void Start() {
    #if UNITY_EDITOR
            Type t = AssemblyUtil.GetType("UnityEditor.EditorApplication");
            if (t != null) {
                if (t.GetProperty("isPlaying").GetValue(null).As<bool>()) {
                    _Server = UdpServer.FactoryWithFreePort(Port);
                }
            }
    #else
            _Server = UdpServer.FactoryWithFreePort(Port);
    #endif
            if (_Server != null) {
                Port = _Server.Port;
                Info("Started UDP server on port: {0}", Port);
            }
        }

        public void Update() {
            if (_Server != null) {
                _Server.HandleReceivedPackets(OnReceivedPacket);
            }
        }

        public bool Register(IRemoteSetting setting) {
            if (setting == null) {
                return false;
            }
            if (_Settings.ContainsKey(setting.Key)) {
                Error("Setting already exist: {0}", setting.Key);
                return false;
            }
            Info("Setting registered: {0}", setting.Key);
            _Settings.Add(setting.Key, setting);
            return true;
        }

        public IRemoteSetting Register(string key, Func<string> getter, Action<string> setter) {
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            if (_Settings.ContainsKey(key)) {
                Error("Setting already exist: {0}", key);
                return null;
            }
            var setting = new RemoteSetting(key, getter, setter);
            _Settings.Add(key, setting);
            Info("Setting registered: {0}", setting.Key);
            return setting;
        }

        public bool Unregister(IRemoteSetting setting) {
            if (setting == null) {
                return false;
            }
            if (!_Settings.ContainsKey(setting.Key)) {
                Error("Setting not found: {0}", setting.Key);
                return false;
            }
            if (_Settings[setting.Key] != setting) {
                Error("Setting not matched: {0}", setting.Key);
                return false;
            }
            _Settings.Remove(setting.Key);
            Info("Setting Unregistered: {0}", setting.Key);
            return true;
        }

        public bool Unregister(ref IRemoteSetting setting) {
            if (Unregister(setting)) {
                setting = null;
                return true;
            }
            return false;
        }

        public IRemoteSetting GetSetting(string key) {
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            if (!_Settings.ContainsKey(key)) {
                Error("Setting not found: {0}", key);
                return null;
            }
            return _Settings[key];
        }
    }
}