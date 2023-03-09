using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Udp;

namespace Edger.Unity.Dev.Remote {
    public partial class RemoteTool : BaseMono {
        public const string Command_List = "list";
        public const string Command_Get = "get";
        public const string Command_Set = "set";
        public const string Command_Toggle = "toggle";


        private void SendUdp(string address, int port, string data) {
            var client = new UdpClient(address, port);
            client.SendData(data);
        }

        private void OnReceivedPacket(UdpPacket packet) {
            Info("Packet received: {0} {1} {2}", packet.FromAddress, packet.Time, packet.Data);
            var request = JsonUtility.FromJson<Request>(packet.Data);
            if (request.port <= 0) {
                Error("Invalid request port: {0} <- {1}", request.port, packet.Data);
            } else if (request.command == Command_List) {
                HandleList(packet, request.port);
            } else if (request.command == Command_Get) {
                HandleGet(packet, request.port, request.key);
            } else if (request.command == Command_Set) {
                HandleSet(packet, request.port, request.key, request.value);
            } else if (request.command == Command_Toggle) {
                HandleToggle(packet, request.port, request.key);
            } else {
                Error("Invalid request command: {0} <- {1}", request.command, packet.Data);
            }
        }

        private void HandleList(UdpPacket packet, int port) {
            var settings = new List<Setting>();
            foreach(var setting in _Settings.Values) {
                settings.Add(new Setting {
                    key = setting.Key,
                    value = setting.GetValue(),
                });
            }
            var response = new Settings { settings = settings };
            SendUdp(packet.FromAddress, port, JsonUtility.ToJson(response));
        }

        private void SendValue(string command, UdpPacket packet, int port, string key, IRemoteSetting setting) {
            string value = "";
            if (setting != null) {
                value = setting.GetValue();
            } else {
                Error("Handle command failed: [{0}] {1} {2} {3}", command, packet.FromAddress, port, key);
            }
            var response = new Setting {
                key = key,
                value = value,
            };
            SendUdp(packet.FromAddress, port, JsonUtility.ToJson(response));
        }

        private void HandleGet(UdpPacket packet, int port, string key) {
            var setting = GetSetting(key);
            SendValue(Command_Get, packet, port, key, setting);
        }

        private void HandleSet(UdpPacket packet, int port, string key, string value) {
            var setting = GetSetting(key);
            if (setting != null) {
                setting.SetValue(value);
            }
            SendValue(Command_Set, packet, port, key, setting);
        }

        private void HandleToggle(UdpPacket packet, int port, string key) {
            var setting = GetSetting(key);
            if (setting != null) {
                bool value = setting.GetValue().RemoteToBool();
                setting.SetValue(!value);
            }
            SendValue(Command_Toggle, packet, port, key, setting);
        }
    }
}