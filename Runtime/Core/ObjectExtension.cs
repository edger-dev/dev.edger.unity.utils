using System;
using System.Collections.Generic;

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
    }
}
