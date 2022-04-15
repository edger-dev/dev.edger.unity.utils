using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Profiling;

using Edger.Unity.Udp;

namespace Edger.Unity.Profiling {
    public partial class ProfilerUtil {
        public const string MULTICAST_ADDRESS = "224.0.0.94";
        public const int DEFAULT_UDP_PORT = 2294;

        public string UdpAddress;
        public int UdpPort;
        private UdpClient _UdpClient;
        public void SetupUdpClient(string address, int port = DEFAULT_UDP_PORT) {
            var client = new UdpClient(address, port);
            if (client.Connected) {
                UdpAddress = address;
                UdpPort = port;
                _UdpClient = client;
            }
        }

        public void SetupMulticast(int port = DEFAULT_UDP_PORT) {
            SetupUdpClient(MULTICAST_ADDRESS, port);
        }
    }
}