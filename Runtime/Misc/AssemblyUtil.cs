using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Edger.Unity {
    public static class AssemblyUtil {
        public enum CheckMode {SubClass, Interface, Assignable};

        public static bool IsValidType(CheckMode mode, Type baseType, Type type) {
            if (type == null || baseType == null) return false;
            switch (mode) {
                case CheckMode.SubClass:
                    return type.IsSubclassOf(baseType);
                case CheckMode.Interface:
                    return type.GetInterfaces().Contains(baseType);
                case CheckMode.Assignable:
                    return baseType.IsAssignableFrom(type);
            }
            return false;
        }

        public static void ForEachAssembly(Action<Assembly> callback) {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asms) {
                //Log.Error("ForEachAssembly: {0}", asm.CodeBase);
                callback(asm);
            }
        }

        public static string _Debugging = null;

        public static void ForEachType(Action<Type> callback) {
            ForEachAssembly((Assembly asm) => {
                try {
                    Type[] types = asm.GetTypes();

                    foreach (Type type in types) {
                        callback(type);
                    }
                } catch (Exception e) {
                    if (_Debugging != null && Log.Provider != null) {
                        Log.Info("ForEachType Failed: [{0}] {1} -> {2}: {3}",
                                _Debugging, asm.GetName().Name, e.GetType().Name, e.Message);
                    }
                }
            });
        }

        public static void ForEachType(CheckMode mode, Type baseType, Action<Type> callback) {
            ForEachType((Type type) => {
                /*
                if (_Debugging != null && Log.Provider != null) {
                    Log.Info("ForEachType: {0} <{1}> {2} {3} -> {4} -> {5}",
                             _Debugging, baseType.FullName,
                             type.Assembly.GetName().Name, type.FullName,
                             type.IsAbstract(), IsValidType(mode, baseType, type));
                }
                */
                if (type.IsAbstract) return;
                if (!IsValidType(mode, baseType, type)) return;

                callback(type);
            });
        }

        public static void ForEachSubClass<T>(Action<Type> callback) where T : class {
            ForEachType(CheckMode.SubClass, typeof(T), callback);
        }

        /*
         * There is no generic constraint for interface.
         *
         * https://msdn.microsoft.com/en-us/library/d5x73970.aspx
         */
        public static void ForEachInterface<T>(Action<Type> callback) where T : class {
            ForEachType(CheckMode.Interface, typeof(T), callback);
        }

        public static void ForEachAssignable<T>(Action<Type> callback) where T : class {
            ForEachType(CheckMode.Assignable, typeof(T), callback);
        }

        public static List<T> CreateInstances<T>(Type baseType, bool sortByPriority = false) {
            var mode = baseType.IsInterface ? CheckMode.Interface : CheckMode.SubClass;
            var result = new List<T>();
            ForEachType(mode, baseType, (Type type) => {
                T instance = Factory.Create<T>(type);
                if (instance != null) {
                    result.Add(instance);
                }
            });
            if (sortByPriority) {
                Priority.SortInstances(result);
            }
            return result;
        }

        public static List<T> CreateInstances<T>(bool sortByPriority = false) {
            return CreateInstances<T>(typeof(T), sortByPriority);
        }

        public static T CreateTopPriorityInstance<T>(Type baseType) {
            var mode = baseType.IsInterface ? CheckMode.Interface : CheckMode.SubClass;
            int maxPriority = -1;
            Type resultType = null;
            ForEachType(mode, baseType, (Type type) => {
                int priority = Priority.GetPriority(type);
                if (priority > maxPriority) {
                    maxPriority = priority;
                    resultType = type;
                }
            });
            if (resultType != null) {
                return Factory.Create<T>(resultType);
            } else {
                return default(T);
            }
        }

        public static T CreateTopPriorityInstance<T>() {
            return CreateTopPriorityInstance<T>(typeof(T));
        }

        public static Type GetType(string fullName) {
            Type result = null;
            ForEachAssembly((Assembly asm) => {
                Type t = asm.GetType(fullName);
                if (t != null){
                    if (result == null) {
                        result = t;
                    } else {
                        Log.Error("Type Conflicted: {0}: {1} -> {2}", fullName, result, t);
                    }
                }
            });
            return result;
        }

        public static Type GetTargetType(Type genericInterface, Type type, bool isDebug = false) {
            foreach (Type interfaceType in type.GetInterfaces()) {
                if (interfaceType.IsGenericType) {
                    if (interfaceType.GetGenericTypeDefinition() == genericInterface) {
                        return interfaceType.GetGenericArguments()[0];
                    }
                }
            }
            Log.ErrorOrDebug(isDebug, "GetTargetType<{0}>() failed: Not Found: {1}", genericInterface.Name, type.FullName);
            return null;
        }

        public static Assembly LoadAssembly(byte[] assemblyData) {
            // Assembly assembly = Assembly.Load(assemblyData);
            Assembly assembly = AppDomain.CurrentDomain.Load(assemblyData);
            return assembly;
        }

        public static List<Assembly> LoadAssemblies(List<byte[]> assembliesData) {
            var result = new List<Assembly>();
            foreach (var data in assembliesData) {
                var assembly = LoadAssembly(data);
                result.Append(assembly);
            }
            return result;
        }
    }
}
