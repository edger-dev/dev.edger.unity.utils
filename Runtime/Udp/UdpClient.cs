using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Edger.Unity.Utils;

namespace Edger.Unity.Utils.Udp {
    public class UdpClient {
        private IPEndPoint _ClientEndPoint = null;

        private System.Net.Sockets.UdpClient _Client = null;
        public System.Net.Sockets.UdpClient Client {
            get { return _Client; }
        }

        public bool Connected {
            get { return _Client != null; }
        }

        public UdpClient(string address, int port, string group = null) {
            try {
                _ClientEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
                _Client = new System.Net.Sockets.UdpClient();
                if (group != null) {
                    _Client.JoinMulticastGroup(IPAddress.Parse(group));
                }
            } catch (Exception err) {
                Log.Error("Failed to create client: {0}:{1} -> {2}", address, port, err);
            }
        }

        /*
        ~UdpClient() {
            Close();
        }
        */

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
                    Log.Error("SendBytes failed: {0} -> {1} -> {2}", _ClientEndPoint, data, err);
                }
            } else {
                Log.Error("SendBytes failed: {0} -> {1}", _ClientEndPoint, data);
            }
            return false;
        }
    }
}
