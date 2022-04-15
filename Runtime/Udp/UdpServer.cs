using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Edger.Unity;

namespace Edger.Unity.Udp {
    public class UdpServer {
        private object _ReceivedPacketsLock = new object();
        private Queue<UdpPacket> _ReceivedPackets = new Queue<UdpPacket>();

        private bool _Started = false;
        public bool Started {
            get { return _Started; }
        }

        private int _Port = -1;
        public int Port {
            get { return _Port; }
        }

        private System.Net.Sockets.UdpClient _Server = null;
        public System.Net.Sockets.UdpClient Server {
            get { return _Server; }
        }

        /*
        public delegate void OnData(string clientIP, byte[] msg);
        public event OnData DataListeners = delegate { };
         */

        public UdpServer(int port, string group = null, bool isDebug = false) {
            _Port = port;
            try {
                _Server = new System.Net.Sockets.UdpClient(_Port);
                //Without this setting, same Udp port can be opened multiple by default, not sure whether it's
                //like this before, what I need is NOT sharing ports anyway
                _Server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                if (group != null) {
                    _Server.JoinMulticastGroup(IPAddress.Parse(group));
                }
                _Started = StartServer();
            } catch (Exception err) {
                Log.ErrorOrDebug(isDebug, "Failed to listen on port: {0} -> {1}", _Port, err);
            }
        }

        ~UdpServer() {
            Close();
        }

        public void Close() {
            if (_Server != null) {
                try {
                    _Server.Close();
                } catch (Exception err) {
                    Log.Error("Failed to close UDP port: {0} -> {1}", _Port, err);
                }
            }
            _Server = null;
            _Started = false;
        }

        public static UdpServer Factory(int port, string group = null) {
            UdpServer server = new UdpServer(port, group);
            if (server.Started) {
                return server;
            }
            return null;
        }

        public static UdpServer FactoryWithFreePort(int basePort, int range = 100, string group = null) {
            for (int i = 0; i < range; i++) {
                int port = basePort + i;
                UdpServer server = new UdpServer(port, group, true);
                if (server.Started) {
                    return server;
                } else {
                    Log.Info("UdpServer.FactoryWithFreePort: port already taken: {0}, {1}, {2}", basePort, range, port);
                }
            }
            Log.Error("Failed to create UdpServer with free port: {0}, {1}, {2}",
                        basePort, range, group);
            return null;
        }

        private bool StartServer() {
            bool result = BeginReceive();
            if (result) {
                Log.Info("UdpServer started: {0}", _Port);
            } else {
                Log.Error("Failed to start UdpServer: {0}", _Port);
            }
            return result;
        }


        private bool BeginReceive() {
            try {
                _Server.BeginReceive(new AsyncCallback(OnReceived), null);
                return true;
            } catch (Exception err) {
                Log.Error("Failed to BeginReceive: {0} -> {1}", _Port, err);
            }
            return false;
        }

        private void OnReceived(IAsyncResult asyncResult) {
            // receivers package and identifies IP
            IPEndPoint endPoint = null;
            byte[] receiveBytes = _Server.EndReceive(asyncResult, ref endPoint);
            try {
                string fromAddress = endPoint.Address.ToString();
                UdpPacket pkt = UdpPacket.ReadPacket(fromAddress, receiveBytes);
                if (pkt != null) {
                    lock (_ReceivedPacketsLock) {
                        _ReceivedPackets.Enqueue(pkt);
                    }
                }
            } catch (Exception e) {
                Log.Error("Handle msg failed: {0} -> {1} -> {2} -> {3}", _Port, endPoint, receiveBytes, e);
            }
            _Started = BeginReceive();
        }

        public void HandleReceivedPackets(Action<UdpPacket> callback, int maxItemCount = 0) {
            lock (_ReceivedPacketsLock) {
                int count = 0;

                while (_ReceivedPackets.Count > 0) {
                    UdpPacket pkt = _ReceivedPackets.Dequeue();
                    callback(pkt);
                    count++;
                    if (maxItemCount > 0 && count >= maxItemCount) {
                        break;
                    }
                }
            }
        }
    }
}
