using System;
using System.Collections.Generic;

using UnityEngine;

namespace Edger.Unity {
    public interface ISingleton : ILogger {
    }

    public static class Singleton {
        public static T GetInstance<T>(ref T instance) where T : Component, ISingleton {
            if (instance == null) {
                instance = Singleton.GetInstance<T>();
                if (instance == null) {
                    GameObject go = GameObjectUtil.GetOrSpawnRoot(CalcName<T>());
                    instance = go.GetOrAddComponent<T>();
                }
            }
            return instance;
        }

        public static void SetupInstance<T>(ref T instance, T self) where T : Component, ISingleton {
            bool failed = false;
            if (instance == null) {
                if (Singleton.AddInstance<T>(self)) {
                    instance = self;
                    self.gameObject.name = CalcName<T>();
                    UnityEngine.Object.DontDestroyOnLoad(self.gameObject);
                } else {
                    failed = true;
                }
            } else if (instance != self) {
                failed = true;
            }
            if (failed) {
                GameObjectUtil.Destroy(self.gameObject);
                self.Critical("SetupInstance<{0}> failed: instance = {1}, self = {2}", typeof(T).FullName, instance, self);
            }
        }

        private static string CalcName<T>() {
            return string.Format("_{0}_", typeof(T).FullName);
        }

        private static Dictionary<Type, ISingleton> _Instances = new Dictionary<Type, ISingleton>();

        private static T GetInstance<T>() where T : Component, ISingleton {
            ISingleton instance = null;
            if (_Instances.TryGetValue(typeof(T), out instance)) {
                return instance.As<T>();
            } else {
                return null;
            }
        }

        private static bool AddInstance<T>(T instance) where T : Component, ISingleton {
            var key = instance.GetType();
            if (instance.transform.parent != null) {
                instance.Error("AddInstance() failed: is not root");
            } else if (_Instances.ContainsKey(key)) {
                instance.Error("AddInstance() failed: already exist: <{0}>", instance.GetType().FullName);
            } else {
                _Instances[key] = instance;
                return true;
            }
            return false;
        }
    }
}