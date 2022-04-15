using System;
using System.Collections;

using UnityEngine;

namespace Edger.Unity {
    public class ReadOnlyFieldAttribute : PropertyAttribute {
    }

    public class MonoTypeAttribute : ReadOnlyFieldAttribute {
    }

    public abstract class BaseMono : MonoBehaviour {
        [MonoTypeAttribute]
        [SerializeField]
        private string _MonoType = null;
        public string MonoType {
            get {
                return _MonoType;
            }
        }

        public void Reset() {
            _MonoType = GetType().FullName;
            OnReset();
        }

        /*
         * In case the type been renamed, do a check here
         *
         * This will be called before scene saved.
         */
        public bool _CheckMonoType() {
            string monoType = GetType().FullName;
            if (_MonoType != monoType) {
                _MonoType = monoType;
                return true;
            }
            return false;
        }

        public void Awake() {
            _CheckMonoType();
            OnAwake();
        }

        protected virtual void OnReset() {}
        protected virtual void OnAwake() {}

        public virtual bool LogDebug { get { return false; } }
        public virtual string LogPrefix { get { return string.Format("<{0}>[{1}]", GetType().Name, gameObject.name); } }
        public virtual bool DebugMode { get { return false; } }

        public bool _Debugging = false;
        public virtual bool Debugging { get { return false; } }

        public void Critical(string format, params object[] values) {
            Log.AddLogWithStackTrace(this, LoggerConsts.CRITICAL, LogPrefix, format, values);
        }

        public void Error(string format, params object[] values) {
            Log.AddLogWithStackTrace(this, LoggerConsts.ERROR, LogPrefix, format, values);
        }

        public void Info(string format, params object[] values) {
            if (DebugMode) {
                Log.AddLogWithStackTrace(this, LoggerConsts.INFO, LogPrefix, format, values);
            } else {
                Log.AddLog(this, LoggerConsts.INFO, LogPrefix, format, values);
            }
        }

        public void Debug(string format, params object[] values) {
            if (DebugMode) {
                Log.AddLogWithStackTrace(this, LoggerConsts.DEBUG, LogPrefix, format, values);
            } else if (LogDebug) {
                Log.AddLog(this, LoggerConsts.DEBUG, LogPrefix, format, values);
            }
        }

        public void ErrorOrDebug(bool isDebug, string format, params object[] values) {
            if (!isDebug) {
                Log.AddLogWithStackTrace(this, LoggerConsts.ERROR, LogPrefix, format, values);
            } else if (DebugMode) {
                Log.AddLogWithStackTrace(this, LoggerConsts.DEBUG, LogPrefix, format, values);
            } else if (LogDebug) {
                Log.AddLog(this, LoggerConsts.DEBUG, LogPrefix, format, values);
            }
        }

        public void Custom(string kind, string format, params object[] values) {
            if (DebugMode) {
                Log.AddLogWithStackTrace(this, kind, LogPrefix, format, values);
            } else {
                Log.AddLog(this, kind, LogPrefix, format, values);
            }
        }

        public void ClearCoroutine(ref IEnumerator coroutine) {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public void RunCoroutine<T>(ref IEnumerator coroutine, Func<T, IEnumerator> doAsync, T param) {
            if (coroutine != null) {
                Error("Coroutine Not Finished Yet: {0}", coroutine);
            }
            coroutine = doAsync(param);
            StartCoroutine(coroutine);
        }

        public void RunCoroutine<T1, T2>(ref IEnumerator coroutine, Func<T1, T2, IEnumerator> doAsync,
                                            T1 param1, T2 param2) {
            if (coroutine != null) {
                Error("Coroutine Not Finished Yet: {0}", coroutine);
            }
            coroutine = doAsync(param1, param2);
            StartCoroutine(coroutine);
        }

        public void RunCoroutine<T1, T2, T3>(ref IEnumerator coroutine, Func<T1, T2, T3, IEnumerator> doAsync,
                                            T1 param1, T2 param2, T3 param3) {
            if (coroutine != null) {
                Error("Coroutine Not Finished Yet: {0}", coroutine);
            }
            coroutine = doAsync(param1, param2, param3);
            StartCoroutine(coroutine);
        }
    }
}

