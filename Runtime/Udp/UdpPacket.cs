using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Edger.Unity;

namespace Edger.Unity.Udp {
    public class UdpPacket {
        public static UdpPacket ReadPacket(string fromAddress, byte[] buffer) {
            UdpPacket pkt = null;
            string data = StringHelper.DecodeUtf8FromBytes(buffer);
            if (data != null) {
                pkt = new UdpPacket(fromAddress, data);
            }
            return pkt;
        }
        public readonly DateTime Time;
        public readonly string FromAddress;
        public readonly string Data;

        public UdpPacket(string fromAddress, string data) {
            Time = System.DateTime.UtcNow;
            FromAddress = fromAddress;
            Data = data;
        }
    }
}
