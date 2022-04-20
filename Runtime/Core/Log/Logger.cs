using System;

namespace Edger.Unity {
    public interface ILogger {
        bool LogDebug { get; }

        void Critical(string format, params object[] values);
        void Error(string format, params object[] values);
        void Info(string format, params object[] values);
        void Debug(string format, params object[] values);
        void Custom(string kind, string format, params object[] values);

        void ErrorOrDebug(bool isDebug, string format, params object[] values);
    }

    public static class LoggerConsts {
        public const string CRITICAL = "CRITICAL";
        public const string ERROR = "ERROR";
        public const string INFO = "INFO";
        public const string DEBUG = "DEBUG";
        public const string TRACE = "TRACE";
    }

    public abstract class Logger : ILogger {
        public virtual bool DebugMode {
            get { return false; }
        }

        public virtual string LogPrefix {
            get {
                return string.Format("[{0}] ", GetType().Name);
            }
        }

        public bool LogDebug {
            get { return DebugMode || Log.LogDebug; }
        }

        public void Critical(string format, params object[] values) {
            string msg = Log.GetMsg(format, values);
            Log.AddLogWithStackTrace(this, LoggerConsts.CRITICAL, LogPrefix, msg);
            throw new CriticalException(msg);
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

        /* Not adding LogPrefix here on purpose, since this is used to create log lines
         * to be used by other tools, e.g. istatd
         */
        public void Custom(string kind, string format, params object[] values) {
            Log.AddLog(this, kind, format, values);
            if (DebugMode) {
                Log.AddLogWithStackTrace(this, kind, LogPrefix, format, values);
            }
        }
    }
}