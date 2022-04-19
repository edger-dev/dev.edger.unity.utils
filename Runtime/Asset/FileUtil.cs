using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

namespace Edger.Unity {
    public static class FileUtil {
        public static void AllLines(string path, Action<string> callback) {
            if (!File.Exists(path)) return;

            using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read))) {
                while (reader.Peek() >= 0) {
                    string line = reader.ReadLine();
                    callback(line);
                }
                reader.Close();
            }
        }

        public static List<string> ReadFile(string path) {
            List<string> lines = new List<string>();
            AllLines(path, (string line) => {
                lines.Add(line);
            });
            return lines;
        }

        public static void CheckDirectory(string path) {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists) {
                Log.Info("Create Directory: {0}", dirInfo);
                dirInfo.Create();
            }
        }

        public static void CheckFileDirectory(string path) {
            DirectoryInfo dirInfo = new FileInfo(path).Directory;
            if (!dirInfo.Exists) {
                Log.Info("Create Directory: {0}", dirInfo);
                dirInfo.Create();
            }
        }

        public static void WriteFile(string path, string content) {
            CheckFileDirectory(path);
            StreamWriter writer = new StreamWriter(path);
            writer.Write(content);
            writer.Close();
        }

        public static string LoadStringFromFile(string path, bool isDebug = false) {
            if (File.Exists(path)) {
                StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
                string str = reader.ReadToEnd();
                reader.Close();
                return str;
            } else {
                Log.ErrorOrDebug(isDebug, "File Not Exist: {0}", path);
                return null;
            }
        }

        public static bool WriteStringToFile(string path, string content) {
            try {
                CheckFileDirectory(path);
                StreamWriter writer = new StreamWriter(path);
                writer.Write(content);
                writer.Close();
                return true;
            } catch (System.Exception e) {
                Log.Error("Failed to save string to file: {0} -> {1}", path, e);
            }
            return false;
        }

        public static void WriteFile(string path, byte[] data) {
            CheckFileDirectory(path);
            using (FileStream stream = new FileStream(path, FileMode.Create)) {
                stream.Write(data, 0, data.Length);
            }
        }

        public static byte[] LoadBytesFromFile(string path, bool isDebug = false) {
            if (File.Exists(path)) {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    byte[] bytes = new byte[(int)stream.Length];
                    stream.Read(bytes,0,(int)stream.Length);
                    return bytes;
                }
            } else {
                Log.ErrorOrDebug(isDebug, "File Not Exist: {0}", path);
                return null;
            }
        }
    }
}
