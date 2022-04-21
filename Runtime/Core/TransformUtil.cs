using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Edger.Unity {
    public static class TransformUtil {
        public const string SegmentSeparator = "/";
        public static string GetPath(Transform t) {
            string path = "";
            while (t != null) {
                if (string.IsNullOrEmpty(path)) {
                    path = t.name;
                } else {
                    path = string.Format("{0}{1}{2}", t.name, SegmentSeparator, path);
                }
                t = t.parent;
            }
            return path;
        }

        public static void FilterComponents<T>(Transform t, Func<Transform, bool> checker, Action<T> callback) where T : Component {
            if (t == null) return;
            if (!checker(t)) return;

            T[] all = t.GetComponents<T>();
            foreach (T one in all) {
                callback(one);
            }

            if (t == null) return;
            foreach (Transform child in t) {
                FilterComponents<T>(child, checker, callback);
            }
        }

        /*
         * It's safe to remove sub transform by using this filter.
         */
        public static void FilterComponentsSafe<T>(Transform t, Func<Transform, bool> checker, Action<T> callback) where T : Component {
            if (t == null) return;
            if (!checker(t)) return;

            T[] all = t.GetComponents<T>();
            foreach (T one in all) {
                callback(one);
            }
            List<Transform> children = null;

            if (t == null) return;
            foreach (Transform child in t) {
                if (children == null) children = new List<Transform>();
                children.Add(child);
            }
            if (children != null) {
                foreach (Transform child in children) {
                    FilterComponentsSafe<T>(child, checker, callback);
                }
            }
        }

        public static void SelfComponentsSafe<T>(Transform t, Action<T> callback) where T : Component {
            FilterComponentsSafe<T>(t, (t1) => t1 == t, callback);
        }

        public static T GetParentComponent<T>(Transform transform) where T : Component {
            Transform t = transform;

            while (t != null) {
                T result = t.GetComponent<T>();
                if (result != null) {
                    return result;
                }
                t = t.parent;
            }
            return null;
        }

        public static Transform GetChild(Transform parent, string name) {
            if (parent == null) return null;
            return parent.Find(name);
        }

    }
}
