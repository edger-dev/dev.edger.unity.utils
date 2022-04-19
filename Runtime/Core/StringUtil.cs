using System;
using System.Collections;
using System.Collections.Generic;

namespace Edger.Unity {
    public static class StringUtil {
        public static void Split(string val, char separator, out string left, out string right) {
            if (!string.IsNullOrEmpty(val)) {
                int pos = val.IndexOf(separator);
                if (pos < 0) {
                    left = val;
                    right = null;
                } else {
                    left = val.Remove(pos);
                    right = val.Substring(pos + 1);
                }
            } else {
                left = null;
                right = null;
            }
            //Log.Debug("Split: [{0}] '{1}' -> [{2}] [{3}]", val, separator, left, right);
        }

        public static void SplitWithEscape(string val, char separator, char escape, out string left, out string right) {
            if (!string.IsNullOrEmpty(val)) {
                bool escaping = false;
                char[] chars = val.ToCharArray();
                for (int i = 0; i < chars.Length; i++) {
                    char ch = chars[i];
                    if (!escaping && (ch == separator)) {
                        left = val.Substring(0, i);
                        right = val.Substring(i + 1);
                        return;
                    } else if (ch == escape) {
                        escaping = !escaping;
                    }
                }
                left = val;
                right = null;
            } else {
                left = null;
                right = null;
            }
            //Log.Debug("Split: [{0}] '{1}' -> [{2}] [{3}]", value, separator, left, right);
        }

        public static string ReplaceFirst(this string str, string oldValue, string newValue) {
            if (string.IsNullOrEmpty(str)) return str;

            int index = str.IndexOf(oldValue);
            if (index >= 0) {
                return str.Remove(index, oldValue.Length).Insert(index, newValue);
            }
            return str;
        }

        public static string ReplaceLast(this string str, string oldValue, string newValue) {
            if (string.IsNullOrEmpty(str)) return str;

            int index = str.LastIndexOf(oldValue);
            if (index >= 0) {
                return str.Remove(index, oldValue.Length).Insert(index, newValue);
            }
            return str;
        }

        public static string DecodeUtf8FromBytes(byte[] bytes) {
            if (bytes == null) return null;
            try {
                return System.Text.Encoding.UTF8.GetString(bytes);
            } catch (Exception e) {
                Log.Error("DecodeUtf8FromBytes Failed: {0} -> {1}", bytes.Length, e);
            }
            return null;
        }

        public static byte[] EncodeUtf8ToBytes(string content) {
            return System.Text.Encoding.UTF8.GetBytes(content);
        }
    }
}
