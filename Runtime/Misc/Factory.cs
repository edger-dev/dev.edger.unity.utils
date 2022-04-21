using System;
using System.Collections.Generic;

namespace Edger.Unity {
    public static class Factory {
        private static T Create<T>(string caller, Type type, params object[] values) {
            if (type != null) {
                try {
                    object obj = Activator.CreateInstance(type, values);
                    if (obj is T) {
                        return (T)obj;
                    } else {
                        Log.Error("Factory.{0}: <{1}> Type Mismatched: {2} -> {3}",
                                caller, typeof(T).FullName, type, obj.GetType().FullName);
                    }
                } catch (Exception e) {
                    Log.Error("Factory.{0}: <{1}> {2} -> {3}",
                                caller, typeof(T).FullName, type, e);
                }
            } else {
                Log.Error("Factory.{0}: <{1}> {2} -> Unknown Type",
                                caller, typeof(T).FullName, type);
            }
            return default(T);
        }

        public static T Create<T>(Type type, params object[] values) {
            return Create<T>("Create", type, values);
        }

        public static T Create<T>(params object[] values) {
            return Create<T>(typeof(T), values);
        }

        public static object[] InsertParams(object[] originalValues, params object[] toInsertValues) {
            if (originalValues == null || originalValues.Length == 0) return toInsertValues;
            if (toInsertValues == null || toInsertValues.Length == 0) return originalValues;

            object[] values = new object[toInsertValues.Length + originalValues.Length];
            for (int i = 0; i < toInsertValues.Length; i++) {
                values[i] = toInsertValues[i];
            }
            for (int i = 0; i < originalValues.Length; i++) {
                values[i + toInsertValues.Length] = originalValues[i];
            }
            return values;
        }

        public static object[] AppendParams(object[] originalValues, params object[] toAppendValues) {
            if (originalValues == null || originalValues.Length == 0) return toAppendValues;
            if (toAppendValues == null || toAppendValues.Length == 0) return originalValues;

            object[] values = new object[originalValues.Length + toAppendValues.Length];
            for (int i = 0; i < originalValues.Length; i++) {
                values[i] = originalValues[i];
            }
            for (int i = 0; i < toAppendValues.Length; i++) {
                values[i + originalValues.Length] = toAppendValues[i];
            }
            return values;
        }
    }
}

