using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity {
    public static class ObjectExtension {
        public static T TryAs<T>(this object obj) {
            if (obj == null) return default(T);

            if (!(obj is T)) {
                return default(T);
            }
            return (T)obj;
        }

        public static T As<T>(this object obj, bool isDebug = false) {
            if (obj == null) return default(T);

            if (!(obj is T)) {
                Log.ErrorOrDebug(isDebug, "Type Mismatched: <{0}> -> {1}: {2}",
                            typeof(T).FullName, obj.GetType().FullName, obj);
                return default(T);
            }
            return (T)obj;
        }

        public static bool Is<T>(this object obj) {
            return TryAs<T>(obj) != null;
        }

        public static string GetTypeAndName(this object obj) {
            if (obj == null) return "null";
            UnityEngine.Object unityObj = As<UnityEngine.Object>(obj);
            if (unityObj != null) {
                return $"<{obj.GetType().Name}>{unityObj.name}";
            }
            return $"<{obj.GetType().Name}>";
        }

        public static void SelectInEditor(this object obj) {
            if (!Application.isEditor) {
                return;
            }
            var selection = System.Type.GetType("UnityEditor.Selection,UnityEditor.dll");
            var activeObject = selection.GetProperty("activeObject", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            activeObject.SetValue(null, obj, null);
        }
    }
}
