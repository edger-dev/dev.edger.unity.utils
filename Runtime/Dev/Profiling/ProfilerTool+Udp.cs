using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Profiling;

using Edger.Unity.Udp;
using Edger.Unity.Dev.Remote;

namespace Edger.Unity.Dev.Profiling {
    public partial class ProfilerTool {
        public const string MULTICAST_ADDRESS = "224.0.0.94";
        public const int DEFAULT_UDP_PORT = 2294;

        public string UdpAddress;
        public int UdpPort = DEFAULT_UDP_PORT;
        private UdpClient _UdpClient;

        public void ResetUdpClient() {
            UdpAddress = "";
            _UdpClient = null;
        }

        public void SetupUdpClient(string address, int? port) {
            UdpPort = port ?? DEFAULT_UDP_PORT;
            var client = new UdpClient(address, UdpPort);
            if (client.Connected) {
                UdpAddress = address;
                _UdpClient = client;
            } else {
                UdpAddress = "";
                _UdpClient = null;
            }
        }

        public void SetupMulticast(int? port) {
            SetupUdpClient(MULTICAST_ADDRESS, port);
        }

        private string GetToUdp() {
            return $"{UdpAddress}:{UdpPort}";
        }

        private void SetToUdp(string value) {
            string address, _port;
            StringUtil.Split(value, ':', out address, out _port);
            if (string.IsNullOrEmpty(address)) {
                ResetUdpClient();
            } else {
                int port = _port.RemoteToInt(DEFAULT_UDP_PORT);
                SetupUdpClient(address, port);
            }
        }
    }
}