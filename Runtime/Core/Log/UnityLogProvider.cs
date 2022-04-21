using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using UnityEngine;

namespace Edger.Unity {
    public class UnityLogProvider : LogProvider {
        public static string GetLogDir() {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android) {
                return Application.temporaryCachePath + "/Logs";
            } else {
                return Application.dataPath + "/../Logs";
            }
        }

        public static bool TraceAsErrorInEditor = true;
        public static bool LogDebugInEditor = false;

        private static bool _GettingContext = false;

        public static UnityEngine.Object GetContext(System.Object obj) {
            if (obj is UnityEngine.Object) {
                return obj as UnityEngine.Object;
            }
            return null;
        }

        private LogFileWriter _LogFileWriter = null;

        public UnityLogProvider(bool logDebug, string logFileName) : base(logDebug) {
            if (!string.IsNullOrEmpty(logFileName)) {
                _LogFileWriter = new LogFileWriter(false, GetLogDir(), logFileName, -1);
            }
        }

        public override void Flush() {
            if (_LogFileWriter != null) {
                _LogFileWriter.Flush();
            }
        }

        public override void Finish() {
            if (_LogFileWriter != null) {
                _LogFileWriter.Finish();
            }
        }

        protected override void OnAddLog(System.DateTime now, object source, string kind, string log, StackTrace stackTrace) {
            if (Application.isEditor) {
                OnLogInEditor(source, kind, log, stackTrace);
            } else {
                OnLogInPlayer(source, kind, log, stackTrace);
            }
            if (_LogFileWriter != null) {
                _LogFileWriter.AddLog(now, kind, log);
            }
        }

        private void OnLogInPlayer(object source, string kind, string log, StackTrace stackTrace) {
            if (stackTrace != null) {
                UnityEngine.Debug.LogError(GetFramePrefex() + log);
            } else if (this.LogDebug || kind != LoggerConsts.DEBUG) {
                UnityEngine.Debug.Log(GetFramePrefex() + log);
            }
        }

        private void OnLogInEditor(object source, string kind, string log, StackTrace stackTrace) {
            if (_GettingContext) return;
            /*
             * In GetContext(), the logic might trigger extra log messages,
             * this logic here is to NOT trying to get context of those messages,
             * since they can trigger loops that leads to stack overflow.
             */
            UnityEngine.Object context = null;
            if (source != null) {
                _GettingContext = true;
                context = GetContext(source);
                _GettingContext = false;
            }
            if (stackTrace != null || (TraceAsErrorInEditor && kind == LoggerConsts.TRACE)) {
                UnityEngine.Debug.LogError(GetFramePrefex() + log, context);
            } else if (LogDebugInEditor || kind != LoggerConsts.DEBUG) {
                UnityEngine.Debug.Log(GetFramePrefex() + log, context);
            }
        }

        private string GetFramePrefex() {
            return $"[{Time.frameCount}] ";
        }
    }
}
