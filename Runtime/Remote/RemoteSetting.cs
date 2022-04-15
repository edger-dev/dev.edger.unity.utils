using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Udp;

namespace Edger.Unity.Remote {
    public interface IRemoteSetting {
        public string Key { get; }
        public string GetValue();
        public void SetValue(string value);
    }

    public class RemoteSetting : IRemoteSetting {
        private readonly string _Key;
        public string Key {
            get { return _Key; }
        }
        private readonly Func<string> _Getter;
        private readonly Action<string> _Setter;

        public RemoteSetting(string key, Func<string> getter, Action<string> setter) {
            _Key = key;
            _Getter = getter;
            _Setter = setter;
        }

        public string GetValue() {
            return _Getter();
        }

        public void SetValue(string value) {
            _Setter(value);
        }
    }

    public static class RemoteSettingExtension {
        public static string ToRemote(this bool value) {
            return value ? "true" : "false";
        }

        public static string ToRemote(this int value) {
            return value.ToString();
        }

        public static string ToRemote(this long value) {
            return value.ToString();
        }

        public static string ToRemote(this float value) {
            return value.ToString();
        }

        public static string ToRemote(this double value) {
            return value.ToString();
        }

        public static void SetValue(this IRemoteSetting setting, bool value) {
            if (setting != null) {
                setting.SetValue(value.ToRemote());
            }
        }

        public static void SetValue(this IRemoteSetting setting, int value) {
            if (setting != null) {
                setting.SetValue(value.ToRemote());
            }
        }

        public static void SetValue(this IRemoteSetting setting, long value) {
            if (setting != null) {
                setting.SetValue(value.ToRemote());
            }
        }

        public static void SetValue(this IRemoteSetting setting, float value) {
            if (setting != null) {
                setting.SetValue(value.ToRemote());
            }
        }

        public static void SetValue(this IRemoteSetting setting, double value) {
            if (setting != null) {
                setting.SetValue(value.ToRemote());
            }
        }

        public static bool RemoteToBool(this string value, bool defaultValue=default(bool)) {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return value.ToLower() == "true";
        }

        public static int RemoteToInt(this string value, int defaultValue=default(int)) {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            try {
                return Convert.ToInt32(value);
            } catch (Exception e) {
                Log.Error("RemoteToInt failed: {0} -> {1}", value, e.Message);
                return defaultValue;
            }
        }

        public static long RemoteToLong(this string value, long defaultValue=default(long)) {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            try {
                return Convert.ToInt64(value);
            } catch (Exception e) {
                Log.Error("RemoteToLong failed: {0} -> {1}", value, e.Message);
                return defaultValue;
            }
        }

        public static float RemoteToFloat(this string value, float defaultValue=default(float)) {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            try {
                return Convert.ToSingle(value);
            } catch (Exception e) {
                Log.Error("RemoteToFloat failed: {0} -> {1}", value, e.Message);
                return defaultValue;
            }
        }

        public static double RemoteToDouble(this string value, double defaultValue=default(double)) {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            try {
                return Convert.ToDouble(value);
            } catch (Exception e) {
                Log.Error("RemoteToDouble failed: {0} -> {1}", value, e.Message);
                return defaultValue;
            }
        }
    }
}