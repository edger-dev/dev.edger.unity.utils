using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Edger.Unity {
    public class CacheResourceItem<T> {
        public readonly bool InCache;
        public readonly T Value;
        public CacheResourceItem(bool inCache, T v) {
            InCache = inCache;
            Value = v;
        }
    }

    public static class CacheResourceUtil {
        public static CacheResourceItem<byte[]> LoadBytes(ILogger logger, string path, bool isDebug = false) {
            byte[] fromCache = CacheUtil.LoadBytes(logger, path, true);
            if (fromCache != null) {
                return new CacheResourceItem<byte[]>(true, fromCache);
            }
            return new CacheResourceItem<byte[]>(false, ResourceUtil.LoadBytes(logger, path, isDebug));
        }

        public static CacheResourceItem<string> LoadText(ILogger logger, string path, bool isDebug = false) {
            var bytes = LoadBytes(logger, path, isDebug);
            if (bytes.Value == null) {
                return new CacheResourceItem<string>(bytes.InCache, null);
            }
            return new CacheResourceItem<string>(bytes.InCache, StringUtil.DecodeUtf8FromBytes(bytes.Value));
        }

        public static CacheResourceItem<T> LoadJson<T>(ILogger logger, string path, bool isDebug = false) {
            var text = LoadText(logger, path, isDebug);
            return new CacheResourceItem<T>(text.InCache, JsonUtil.Decode<T>(logger, text.Value));
        }
    }
}