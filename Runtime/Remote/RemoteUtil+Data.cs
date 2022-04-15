using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Udp;

namespace Edger.Unity.Remote {
    public partial class RemoteUtil {
        [Serializable]
        public struct Request {
            public int port;
            public string command;
            public string key;
            public string value;
        }

        [Serializable]
        public struct Setting {
            public string key;
            public string value;
        }

        [Serializable]
        public struct Settings {
            public List<Setting> settings;
        }
    }
}