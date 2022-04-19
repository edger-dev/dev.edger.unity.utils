using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Edger.Unity {
    public class FolderActionResult {
        public readonly string[] Pathes;
        public readonly Exception Error;
        public FolderActionResult(string[] pathes, Exception error) {
            Pathes = pathes;
            Error = error;
        }
    }

    public static class CacheUtil {
        public static string GetCacheRoot() {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android) {
                return Application.temporaryCachePath;
            } else {
                return Path.Combine(Application.dataPath, "..", "_cache_");
            }
        }

        public static byte[] LoadBytes(ILogger logger, string path, bool isDebug = false) {
            var filePath = Path.Combine(GetCacheRoot(), path);
            if (logger.LogDebug) {
                logger.Debug("CacheUtil.LoadBytes() Trying: {0} -> {1}", path, filePath);
            }
            if (File.Exists(filePath)) {
                try {
                    return FileUtil.LoadBytesFromFile(filePath, isDebug);
                } catch (Exception e) {
                    logger.Error("CacheUtil.LoadBytes() Failed: {0} -> {1} -> {2}", path, filePath, e);
                }
            } else {
                logger.ErrorOrDebug(isDebug, "CacheUtil.LoadBytes() Failed: {0} -> {1} -> Not Found", path, filePath);
            }
            return null;
        }

        public static string LoadText(ILogger logger, string path, bool isDebug = false) {
            var bytes = LoadBytes(logger, path, isDebug);
            if (bytes == null) return null;
            return StringUtil.DecodeUtf8FromBytes(bytes);
        }

        public static T LoadJson<T>(ILogger logger, string path, bool isDebug = false) {
            var text = LoadText(logger, path, isDebug);
            return JsonUtil.Decode<T>(logger, text);
        }

        public static bool Save(ILogger logger, string path, Action<FileStream> write) {
            var cachePath = System.IO.Path.Combine(GetCacheRoot(), path);
            try {
                FileUtil.CheckFileDirectory(cachePath);
                FileStream stream = new FileStream(cachePath, FileMode.Create, FileAccess.Write);
                write(stream);
                stream.Close();
                logger.Info("Cache Saved: {0} -> {1}", path, cachePath);
                return true;
            } catch (System.Exception e) {
                logger.Error("Save Cache Failed: {0} -> {1} -> {2}", path, cachePath, e);
                return false;
            }
        }

        public static bool SaveBytes(ILogger logger, string path, List<byte[]> chunks) {
            return Save(logger, path, (FileStream stream) => {
                foreach (var chunk in chunks) {
                    stream.Write(chunk, 0, chunk.Length);
                }
            });
        }

        public static bool SaveBytes(ILogger logger, string path, byte[] bytes) {
            return Save(logger, path, (FileStream stream) => {
                stream.Write(bytes, 0, bytes.Length);
            });
        }

        public static bool SaveText(ILogger logger, string path, string text) {
            byte[] bytes = StringUtil.EncodeUtf8ToBytes(text);
            return SaveBytes(logger, path, bytes);
        }

        public static bool SaveJson(ILogger logger, string path, IJson obj, bool pretty = true) {
            string text = obj.ToJson(pretty);
            return SaveText(logger, path, text);
        }

        private static void DoCopyFolder(ILogger logger, List<string> pathes, string ToRoot, string prefix, DirectoryInfo dir) {
            var toFolder = Path.Combine(ToRoot, prefix);
            FileUtil.CheckDirectory(toFolder);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                var toPath = Path.Combine(toFolder, file.Name);
                File.Copy(file.FullName, toPath, true);
                var path = Path.Combine(prefix, file.Name);
                pathes.Add(path);
                logger.Info("File Copied: {0} -> {1} -> {2}", path, file.FullName, toPath);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs) {
                string subPrefix = prefix == null ? subdir.Name
                            : Path.Combine(prefix, subdir.Name);
                DoCopyFolder(logger, pathes, ToRoot, subPrefix, subdir);
            }
        }

        public static FolderActionResult CopyFolder(ILogger logger, string fromPath, string toPath) {
            var cacheFromPath = System.IO.Path.Combine(GetCacheRoot(), fromPath);
            var cacheToPath = System.IO.Path.Combine(GetCacheRoot(), toPath);
            List<string> pathes = new List<string>();
            System.Exception error = null;
            try {
                DirectoryInfo dir = new DirectoryInfo(cacheFromPath);
                if (!dir.Exists) {
                    throw new CriticalException("CopyFolder Failed: Dir Not Exist: {0}", dir.FullName);
                }
                DoCopyFolder(logger, pathes, cacheToPath, "", dir);
            } catch (System.Exception e) {
                logger.Error("Copy Folder Failed: {0} -> {1} -> {2}", cacheFromPath, cacheToPath, e);
                error = e;
            }
            return new FolderActionResult(pathes.ToArray(), error);
        }

        private static void DoDeleteFolder(ILogger logger, List<string> pathes, string prefix, DirectoryInfo dir) {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                File.Delete(file.FullName);
                var path = Path.Combine(prefix, file.Name);
                pathes.Add(path);
                logger.Info("File Deleted: {0} -> {1}", path, file.FullName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs) {
                string subPrefix = prefix == null ? subdir.Name
                            : Path.Combine(prefix, subdir.Name);
                DoDeleteFolder(logger, pathes, subPrefix, subdir);
            }
            logger.Info("Folder Deleted: {0} -> {1}", prefix, dir.FullName);
            Directory.Delete(dir.FullName);
        }

        public static FolderActionResult DeleteFolder(ILogger logger, string path) {
            var cachePath = System.IO.Path.Combine(GetCacheRoot(), path);
            List<string> pathes = new List<string>();
            System.Exception error = null;
            try {
                DirectoryInfo dir = new DirectoryInfo(cachePath);
                if (dir.Exists) {
                    DoDeleteFolder(logger, pathes, "", dir);
                }
            } catch (System.Exception e) {
                logger.Error("Delete Folder Failed: {0} -> {1} -> {2}", path, cachePath, e);
                error = e;
            }
            return new FolderActionResult(pathes.ToArray(), error);
        }
    }
}