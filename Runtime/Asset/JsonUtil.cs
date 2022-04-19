using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Edger.Unity {
    public interface IJson {
    }

    public static class JsonExtension {
        public static string ToJson(this IJson v, bool pretty = true) {
            if (v == null) return "null";
            return JsonUtility.ToJson(v, pretty);
        }
    }


    public static class JsonUtil {
        public static T Decode<T>(ILogger logger, string text) {
            if (text == null) return default(T);
            try {
                return JsonUtility.FromJson<T>(text);
            } catch (Exception e) {
                logger.Error("JsonUtil.Decode<{0}>() Failed: {1} -> {2}", typeof(T).FullName, text, e);
            }
            return default(T);
        }
    }
}