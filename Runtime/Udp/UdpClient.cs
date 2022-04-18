using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Edger.Unity;

namespace Edger.Unity.Udp {
    public class UdpClient {
        public static bool IsIPv4Multicast(string address) {
            IPAddress ipAddress;
            if (IPAddress.TryParse(address, out ipAddress)) {
                return address.StartsWith("224.0.0.");
            }
            return false;
        }
        private IPEndPoint _ClientEndPoint = null;

        private System.Net.Sockets.UdpClient _Client = null;
        public System.Net.Sockets.UdpClient Client {
            get { return _Client; }
        }

        public bool Connected {
            get { return _Client != null; }
        }

        public UdpClient(string address, int port) {
            try {
                var ip = IPAddress.Parse(address);
                _ClientEndPoint = new IPEndPoint(ip, port);
                _Client = new System.Net.Sockets.UdpClient();
                if (IsIPv4Multicast(address)) {
                    _Client.JoinMulticastGroup(ip);
                }
            } catch (Exception err) {
                Log.Error("Failed to create client: {0}:{1} -> {2}", address, port, err);
            }
        }

        public void Close() {
            if (_Client != null) {
                try {
                    _Client.Close();
                } catch (Exception err) {
                    Log.Error("Failed to close UDP client: {0} -> {1}", _Client, err);
                }
            }
            _Client = null;
        }

        public bool SendData(string data) {
            byte[] bytes = StringHelper.EncodeUtf8ToBytes(data);
            if (_Client != null) {
                try {
                    _Client.Send(bytes, bytes.Length, _ClientEndPoint);
                    return true;
                } catch (Exception err) {
                    Close();
                    Log.Error("SendBytes failed: {0} -> {1} -> {2}", _ClientEndPoint, data, err);
                }
            } else {
                Log.Error("SendBytes failed: {0} -> {1}", _ClientEndPoint, data);
            }
            return false;
        }
    }
}
