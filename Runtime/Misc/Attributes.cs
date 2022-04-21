using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Edger.Unity {
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class Priority: System.Attribute {
        public static int GetPriority(Type type) {
            object[] attribs = type.GetCustomAttributes(false);
            foreach (var attr in attribs) {
                if (attr is Priority) {
                    return ((Priority)attr).Value;
                }
            }
            return 0;
        }

        //Higher Priority First
        public static void SortTypes(List<Type> types) {
            types.Sort((Type a, Type b) => {
                int orderA = GetPriority(a);
                int orderB = GetPriority(b);
                if (orderA == orderB) {
                    return -1 * a.FullName.CompareTo(b.FullName);
                } else {
                    return -1 * orderA.CompareTo(orderB);
                }
            });
        }

        //Higher Priority First
        public static void SortInstances<T>(List<T> objs) {
            Dictionary<T, int> orders = new Dictionary<T, int>();

            foreach (T obj in objs) {
                orders[obj] = GetPriority(obj.GetType());
            }

            objs.Sort((T a, T b) => {
                int orderA = orders[a];
                int orderB = orders[b];
                if (orderA == orderB) {
                    return -1 * a.GetType().FullName.CompareTo(b.GetType().FullName);
                } else {
                    return -1 * orderA.CompareTo(orderB);
                }
            });
        }

        public readonly int Value;
        public Priority(int priority) {
            Value = priority;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class DapParam : System.Attribute {
        public const string DefaultSuffix = "_Default";
        public static string GetDefaultFieldName(string fieldName) {
            return string.Format("{0}{1}", fieldName, DefaultSuffix);
        }

        public static DapParam GetDapParam(FieldInfo field) {
            object[] attribs = field.GetCustomAttributes(false);
            foreach (var attr in attribs) {
                if (attr is DapParam) {
                    return (DapParam)attr;
                }
            }
            return null;
        }

        public readonly System.Type ParamType;
        public readonly bool Optional;

        public DapParam(System.Type t, bool optional) {
            ParamType = t;
            Optional = optional;
        }

        public DapParam(System.Type t) : this(t, false) {}
    }
}
