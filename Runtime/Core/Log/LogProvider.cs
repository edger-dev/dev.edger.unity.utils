using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Edger.Unity {
    public abstract class LogProvider : ILogger {
        public static int MAX_STACK_TRACK_NUM = 32;
        public static int DEBUG_MAX_STACK_TRACK_NUM = 128;
        public static string TIMESTAMP_FORMAT = "HH:mm:ss.fff";    //http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx

        protected abstract void OnAddLog(System.DateTime now, object source, string kind, string log, StackTrace stackTrace);

        protected bool _AutoFlush = true;

        public virtual bool AutoFlush {
            get { return _AutoFlush; }
        }
        public virtual void Flush() {}
        public virtual void Finish() {}

        private bool _LogDebug;
        public bool LogDebug {
            get { return _LogDebug; }
        }
        public void SetLogDebug(bool logDebug) {
            _LogDebug = logDebug;
        }

        private StringBuilder _StackBuilder = new StringBuilder(1024);

        protected LogProvider(bool logDebug) {
            _LogDebug = logDebug;
        }

        public void Critical(string format, params object[] values) {
            StackTrace stackTrace = new StackTrace(1, true);
            string msg = Log.GetMsg(format, values);
            AddLog(this, LoggerConsts.CRITICAL, msg, stackTrace);
            throw new CriticalException(msg);
        }

        public void Error(string format, params object[] values) {
            StackTrace stackTrace = new StackTrace(1, true);
            AddLog(this, LoggerConsts.ERROR, Log.GetMsg(format, values), stackTrace);
        }

        public void Info(string format, params object[] values) {
            AddLog(this, LoggerConsts.INFO, Log.GetMsg(format, values), null);
        }

        public void Debug(string format, params object[] values) {
            if (_LogDebug) {
                AddLog(this, LoggerConsts.DEBUG, Log.GetMsg(format, values), null);
            }
        }

        public void ErrorOrDebug(bool isDebug, string format, params object[] values) {
            if (!isDebug) {
                AddLog(this, LoggerConsts.ERROR, Log.GetMsg(format, values), null);
            } else if (_LogDebug) {
                AddLog(this, LoggerConsts.DEBUG, Log.GetMsg(format, values), null);
            }
        }

        public void Custom(string kind, string format, params object[] values) {
            AddLog(this, kind, Log.GetMsg(format, values), null);
        }

        public string FormatStackTrace(StackTrace stackTrace, string prefix, int max) {
            _StackBuilder.Length = 0;
            for (int i = 0; i< stackTrace.FrameCount; i++) {
                if (i >= max) break;
                StackFrame stackFrame = stackTrace.GetFrame(i);
                var method = stackFrame.GetMethod();
                _StackBuilder.Append(prefix);
                _StackBuilder.Append(Path.GetFileName(stackFrame.GetFileName()));
                _StackBuilder.Append("<");
                _StackBuilder.Append(stackFrame.GetFileLineNumber());
                _StackBuilder.Append(">\t");
                if (method != null) {
                    var t = method.ReflectedType;
                    if (t != null) {
                        _StackBuilder.Append(method.ReflectedType.Namespace);
                        _StackBuilder.Append(".");
                        _StackBuilder.Append(method.ReflectedType.Name);
                        _StackBuilder.Append("\t");
                    }
                    _StackBuilder.Append(method.Name);
                    _StackBuilder.Append("()\n");
                }
            }
            return _StackBuilder.ToString();
        }

        public string FormatBytes(byte[] data, int startIndex, int size, string prefix = null) {
            _StackBuilder.Length = 0;
            if (!string.IsNullOrEmpty(prefix)) {
                _StackBuilder.Append(prefix);
                _StackBuilder.Append("\n");
            }
            for (int i = startIndex; i < size; i++) {
                int index = i - startIndex;
                if (index % 32 == 0) {
                    _StackBuilder.Append("\t");
                }
                _StackBuilder.Append(data[i].ToString("X2"));
                if (index % 32 == 31) {
                    _StackBuilder.Append("\n");
                } else {
                    _StackBuilder.Append(" ");
                }
            }
            return _StackBuilder.ToString();
        }

        public void AddLog(object source, string kind, string msg, StackTrace stackTrace) {
            var now = System.DateTime.UtcNow;

            string log = null;
            if (stackTrace != null) {
                log = string.Format("{0} {1}[{2}] {3}\n{4}",
                            now.ToString(TIMESTAMP_FORMAT), GetTickMsg(), kind, msg,
                            FormatStackTrace(stackTrace, "\t",
                                LogDebug ? DEBUG_MAX_STACK_TRACK_NUM: MAX_STACK_TRACK_NUM));
            } else {
                log = string.Format("{0} {1}[{2}] {3}",
                            now.ToString(TIMESTAMP_FORMAT), GetTickMsg(), kind, msg);
            }

            OnAddLog(now, source, kind, log, stackTrace);
        }

        protected virtual string GetTickMsg() {
            return "";
        }
    }
}
