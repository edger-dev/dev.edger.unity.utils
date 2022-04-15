using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Diagnostics;

namespace Edger.Unity {
    public class LogFileWriter {
        public static long FLUSH_DURATION = 5 * 60 * 1000; // flush every 5 minute

        private string _LogDir = "";
        private string _LogName = "";
        private int _RunID = -1;
        private int _LogDayOfYear = -1;
        private string _LogFilePath = "";
        public string LogFilePath {
            get { return _LogFilePath; }
        }

        private StreamWriter _LogWriter = null;
        private DateTime _LastFlushTime;

        protected bool _AutoFlush = true;

        public virtual bool AutoFlush {
            get { return _AutoFlush; }
        }

        public LogFileWriter(bool autoFlush, string logDir, string logName, int runID) {
            _AutoFlush = autoFlush;
            _LogDir = logDir;
            _LogName = logName;

            if (runID < 0) {
                _RunID = GetNextRunID(logDir, logName, 1);
            } else {
                _RunID = runID;
            }
            SetupLogWriter();
        }

        public override string ToString() {
            return string.Format("[<{0}>{1}]", GetType().FullName);
        }

        public int GetNextRunID(string logDir, string logName, int startRunID) {
            int runID = startRunID;

            while (true) {
                if (!IsLogFileExist(logDir, logName, runID)) {
                    break;
                }
                runID++;
            }
            Log.Info("GetNextRunID: {0} {1} {2} -> {3}", logDir, logName, startRunID, runID);
            return runID;
        }

        public void SetRunID(int runID) {
            Log.Info("SetRunID: {0}", runID);
            _RunID = runID;
            SetupLogWriter();
        }

        public void SetAutoFlush(bool autoFlush) {
            _AutoFlush = autoFlush;
            SetupLogWriter();
        }

        public void Flush() {
            if (_LogWriter != null) {
                _LogWriter.Flush();
            }
        }

        public void Finish() {
            if (_LogWriter != null) {
                _LogWriter.Flush();
                _LogWriter.Dispose();
                _LogWriter = null;
            }
        }

        private bool IsLogFileExist(string logDir, string logName, int runID) {
            var now = System.DateTime.UtcNow;
            string month = now.ToString("yyyy-MM");
            string dir = string.Format("{0}/{1}", logDir, month);

            string date = now.ToString("yyyy-MM-dd");
            string logFilePath = string.Format("{0}/{1}_{2}_{3}.log", dir, date, logName, runID);
 
            try {
                if (!Directory.Exists(dir)) {
                    return false;
                }
                return File.Exists(logFilePath);
            } catch (Exception e) {
                Log.Error("IsLogFileExist: Get Exception: {0}", e);
                return false;
            }
        }

        private void SetupLogWriter() {
            Finish();
            var now = System.DateTime.UtcNow;
            _LastFlushTime = now;

            string month = now.ToString("yyyy-MM");
            string dir = string.Format("{0}/{1}", _LogDir, month);

            _LogDayOfYear = now.DayOfYear;
            string date = now.ToString("yyyy-MM-dd");
            _LogFilePath = string.Format("{0}/{1}_{2}_{3}.log", dir, date, _LogName, _RunID);
 
            try {
                if (!Directory.Exists(dir)) {
                    Log.Info("CreateDirectory: {0}", dir);
                    Directory.CreateDirectory(dir);
                }
                FileStream stream = new FileStream(_LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                _LogWriter = new StreamWriter(stream);
                Log.Info("Start Logging: {0}", _LogFilePath);

                //Probably can't use AutoFlush here due to performance issue
                //need to check whether the system level cache make this workable
                //or need to flush periodically.
                _LogWriter.AutoFlush = AutoFlush;
            } catch (Exception e) {
                _LogWriter = null;
                Log.Error("Failed to create log writer: {0} : {1}", _LogFilePath, e);
            }
        }

        public void AddLog(System.DateTime time, string kind, string log) {
            if (_LogWriter != null) {
                if (time.DayOfYear != _LogDayOfYear) {
                    SetupLogWriter();
                    if (_LogWriter == null) {
                        return;
                    }
                }
                try {
                    _LogWriter.WriteLine(log);
                    if (kind == LoggerConsts.CRITICAL || time.Ticks - _LastFlushTime.Ticks > FLUSH_DURATION) {
                        _LogWriter.Flush();
                        _LastFlushTime = time;
                    }
                } catch (Exception e) {
                    _LogWriter = null;
                    Log.Error("Failed to write log: {0} : {1}", _LogFilePath, e);
                    //TODO: maybe try to resume logging to file later.
                }
            }
        }
    }
}
