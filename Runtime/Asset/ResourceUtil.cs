using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Edger.Unity {
    public static class ResourceUtil {
        // https://docs.unity3d.com/Manual/class-TextAsset.html
        public readonly static string[] Supported_Extensions =
            new string[] {".txt", ".html", ".htm", ".xml", ".bytes", ".json", ".csv", ".yaml", ".fnt" };

        public static bool IsValidPath(string path) {
            foreach (var ext in Supported_Extensions) {
                if (path.EndsWith(ext)) {
                    return true;
                }
            }
            return false;
        }

        public static string CalcResourcePath(string path) {
            foreach (var ext in Supported_Extensions) {
                if (path.EndsWith(ext)) {
                    return StringUtil.ReplaceLast(path, ext, "");
                }
            }
            return path;
        }

        public static byte[] LoadBytes(ILogger logger, string path, bool isDebug = false) {
            var resourcePath = CalcResourcePath(path);
            if (logger.LogDebug) {
                logger.Debug("ResourceUtil.LoadBytes() Trying: {0} -> {1}", path, resourcePath);
            }
            try {
                TextAsset asset = Resources.Load<TextAsset>(resourcePath);
                if (asset != null) {
                    return asset.bytes;
                } else {
                    logger.ErrorOrDebug(isDebug, "ResourceUtil.LoadBytes() Failed: {0} -> {1} -> Not Found", path,resourcePath);
                }
            } catch (Exception e) {
                logger.Error("ResourceUtil.LoadBytes() Failed: {0} -> {1} -> {2}", path, resourcePath, e);
            }
            return null;
        }

        public static string LoadText(ILogger logger, string path, bool isDebug = false) {
            var bytes = LoadBytes(logger, path, isDebug);
            if (bytes == null) return null;
            return StringUtil.DecodeUtf8FromBytes(bytes);
        }

        public static T LoadJson<T>(ILogger logger, string path, bool isDebug = false) {
            var text = LoadText(logger, path, isDebug);
            return JsonUtil.Decode<T>(logger, text);
        }
    }
}