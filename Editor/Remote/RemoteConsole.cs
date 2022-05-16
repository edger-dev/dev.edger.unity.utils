#if UNITY_2021_1_OR_NEWER

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Edger.Unity.Udp;
using Edger.Unity.Remote;


namespace Edger.Unity.Editor.Remote {
    public class RemoteConsole : EditorWindow {
        [MenuItem("Edger/Open Remote Console")]
        public static void OpenRemoteConsole() {
            RemoteConsole wnd = GetWindow<RemoteConsole>();
            wnd.titleContent = new GUIContent("Edger Remote Console");
        }
        public const int DEFAULT_UDP_PORT = 12295;

        private UdpServer _Server;
        public int ServerPort {
            get {
                if (_Server == null) {
                    _Server = UdpServer.FactoryWithFreePort(DEFAULT_UDP_PORT);
                }
                if (_Server != null && _Server.Started) {
                    return _Server.Port;
                } else {
                    return -1;
                }
            }
        }

        public string RemoteAddress { get; private set; }
        public int RemotePort { get; private set; }

        private List<RemoteUtil.Setting> _Settings = new List<RemoteUtil.Setting>();
        private ListView _SettingsView;

        public void Update() {
            if (_Server != null) {
                _Server.HandleReceivedPackets(OnReceivedPacket);
            }
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/dev.edger.unity.utils/Editor/Remote/RemoteConsole.uxml");
            VisualElement view = visualTree.Instantiate();
            root.Add(view);

            var settingView = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/dev.edger.unity.utils/Editor/Remote/RemoteSetting.uxml");

            _SettingsView = view.Q<ListView>("settings");
            _SettingsView.itemsSource = _Settings;
            _SettingsView.makeItem = () => MakeSettingView(settingView);
            _SettingsView.bindItem = BindSettingView;
            var refresh = view.Q<Button>("refresh");
            refresh.clicked += OnRefreshClicked;
        }

        private VisualElement MakeSettingView(VisualTreeAsset asset) {
            var view = asset.CloneTree();
            var settingView = view.Q<TextField>("setting");
            var set = view.Q<Button>("set");
            set.clicked += () => {
                OnSetClicked(settingView);
            };
            var toggle = view.Q<Button>("toggle");
            toggle.clicked += () => {
                OnToggleClicked(settingView);
            };
            return view;
        }

        private void BindSettingView(VisualElement view, int index) {
            if (index < 0 || index >= _Settings.Count) {
                Log.Error("Invalid Index: {0}/{1}", index, _Settings.Count);
                return;
            }
            var setting = _Settings[index];
            Log.Info("BindSettingView(): {0}/{1} {2} = {3}", index, _Settings.Count, setting.key, setting.value);
            var settingView = view.Q<TextField>("setting");
            settingView.label = setting.key;
            settingView.value = setting.value;
            var toggle = view.Q<Button>("toggle");
            toggle.SetEnabled(settingView.value.RemoteIsBool());
        }

        private void OnSetClicked(TextField settingView) {
            SendRequest(RemoteUtil.Command_Set, settingView.label, settingView.text);
        }

        private void OnToggleClicked(TextField settingView) {
            SendRequest(RemoteUtil.Command_Toggle, settingView.label);
        }

        private void OnRefreshClicked() {
            var ip = rootVisualElement.Q<TextField>("remote_ip");
            var port = rootVisualElement.Q<TextField>("remote_port");
            RemoteAddress = ip.text;
            RemotePort = Convert.ToInt32(port.text);
            _Settings.Clear();
            _SettingsView.Rebuild();
            SendRequest(RemoteUtil.Command_List);
        }

        private void SendRequest(RemoteUtil.Request request) {
            var client = new UdpClient(RemoteAddress, RemotePort);
            var data = JsonUtility.ToJson(request);
            Log.Info("<RemoteConsole> Sending Request to {0}:{1} -> {2}", RemoteAddress, RemotePort, data);
            client.SendData(data);
        }

        private void SendRequest(string command, string key = "", string value = "") {
            var request = new RemoteUtil.Request{
                port = ServerPort,
                command = command,
                key = key,
                value = value,
            };
            SendRequest(request);
        }

        private void OnReceivedPacket(UdpPacket packet) {
            Log.Info("<RemoteConsole> Packet received: {0} {1} {2}", packet.FromAddress, packet.Time, packet.Data);
            var setting = JsonUtility.FromJson<RemoteUtil.Setting>(packet.Data);
            if (!string.IsNullOrEmpty(setting.key)) {
                OnReceivedSetting(setting);
            } else {
                OnReceivedSettings(JsonUtility.FromJson<RemoteUtil.Settings>(packet.Data));
            }
        }

        private void OnReceivedSettings(RemoteUtil.Settings settings) {
            _Settings.Clear();
            foreach (var setting in settings.settings) {
                _Settings.Add(setting);
            }
            _SettingsView.Rebuild();
        }

        private void OnReceivedSetting(RemoteUtil.Setting setting) {
            for (int i = 0; i < _Settings.Count; i++) {
                if (_Settings[i].key == setting.key) {
                    _Settings[i] = setting;
                    _SettingsView.RefreshItem(i);
                    break;
                }
            }
        }
    }
}

#endif