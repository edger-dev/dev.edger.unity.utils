using System;
using System.IO;
using System.IO.Compression;

using Edger.Unity;

namespace Edger.Unity.Zip {
    /*
     * Not using the Zip's encryption here, for 2 reasons:
     *
     * - The ionic from Pathfinding only support EncryptionAlgorithm.PkzipWeak
     * - Each time create a new zip with password, the data is changed, guess it's due
     *   to some salt-like algorithem.
     */
    public static class ZipUtil {
        public const bool USE_ZIP_ENCRIPTION = false;

        public static string FILENAME_DATA = "data";
        //The time here is to keep the zip file identical for same data
        public static DateTimeOffset ENTRY_TIME = new DateTimeOffset(new DateTime(2000, 1, 1));

        public static byte[] GetDataFromZip(ZipArchive zip, string fileName, string password) {
            byte[] data = null;
            try {
                ZipArchiveEntry entry = zip.GetEntry(fileName);
                if (entry != null) {
                    MemoryStream buffer = new MemoryStream();
                    using (var entryStream = entry.Open()) {
                        entryStream.CopyTo(buffer);
                    }
                    buffer.Close();
                    data = buffer.ToArray();
                    if (!string.IsNullOrEmpty(password)) {
                        data = AesUtil.Decrypt(data, password);
                    }
                } else {
                    Log.Error("GetDataFromZip Failed: No entry in zip: {0}", fileName);
                }
            } catch (Exception e) {
                //Catches exceptions when an invalid zip file is found
                Log.Error("GetDataFromZip Failed: {0}, {1} -> {2}", zip, fileName, e);
            }
            return data;
        }

        public static ZipArchive GetZipFromData(byte[] data, string password) {
            try {
                MemoryStream stream = new MemoryStream();
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
                return zip;
            } catch (Exception e) {
                //Catches exceptions when an invalid zip file is found
                Log.Error("GetZipFromData Failed: [{0}] -> {1}", data.Length, e);
            }
            return null;
        }

        public static ZipArchive GetZipFromFile(string path, string password) {
            if (!File.Exists(path)) {
                return null;
            }

            try {
                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read);
                    return zip;
                }
            } catch (Exception e) {
                //Catches exceptions when an invalid zip file is found
                Log.Error("GetZipFromFile Failed: [{0}] -> {1}", path, e);
            }
            return null;
        }

        public static byte[] Encode(byte[] data, string password) {
            using (MemoryStream output = new MemoryStream()) {
                using (ZipArchive zip = new ZipArchive(output, ZipArchiveMode.Create)) {
                    AddEntryToZip(zip, FILENAME_DATA, data, password);

                    return output.ToArray();
                }
            }
        }

        public static byte[] Decode(byte[] data, string password) {
            if (data == null) return null;

            byte[] result = null;
            using (ZipArchive zip = GetZipFromData(data, password)) {
                if (zip != null) {
                    result = GetDataFromZip(zip, FILENAME_DATA, password);
                }
            }
            if (result == null) {
                Log.Error("ZipHelper.Decode() Failed: [{0}], {1}", data.Length, password);
            }
            return result;
        }

        public static byte[] DecodeFromFile(string path, string password) {
            return Decode(FileUtil.LoadBytesFromFile(path), password);
        }

        public static bool SaveToZip(string path, string password, Action<ZipArchive> addEntries) {

            try {
                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create)) {
                        addEntries(zip);
                        return true;
                    }
                }
            } catch (Exception e) {
                Log.Error("SaveToZip Failed: [{0}] -> {1}", path, e);
            }
            return false;
        }

        public static void AddEntryToZip(ZipArchive zip, string path, byte[] data, string password) {
            if (!string.IsNullOrEmpty(password)) {
                data = AesUtil.Encrypt(data, password);
            }
            try {
                using (MemoryStream stream = new MemoryStream()) {
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    var entry = zip.CreateEntry(path);
                    entry.LastWriteTime = ENTRY_TIME;
                    using (var entryStream = entry.Open()) {
                        stream.CopyTo(entryStream);
                    }
                }
            } catch (Exception e) {
                Log.Error("AddEntryToZip Failed: {0} {1} [{2}] -> {3}",
                        zip, path, data.Length, e);
            }
        }

        public static string LoadStringFromFile(string path, string password) {
            byte[] data = DecodeFromFile(path, password);
            if (data != null) {
                try {
                    return StringUtil.DecodeUtf8FromBytes(data);
                } catch (Exception e) {
                    Log.Error("Failed to get string from data: {0} -> {1}", path, e);
                }
            }
            return null;
        }

        public static bool SaveDataToFile(string path, byte[] data, string password) {
            return SaveToZip(path, password, (ZipArchive zip) => {
                AddEntryToZip(zip, FILENAME_DATA, data, password);
            });
        }

        public static bool SaveStringToFile(string path, string content, string password) {
            return SaveDataToFile(path, StringUtil.EncodeUtf8ToBytes(content), password);
        }

        /*
        public static byte[] Compress(byte[] data) {
            var result = Ionic.Zlib.ZlibStream.CompressBuffer(data);
            return result;
        }

        public static byte[] Uncompress(byte[] data) {
            var result = Ionic.Zlib.ZlibStream.UncompressBuffer(data);
            return result;
        }
        */
    }
}

