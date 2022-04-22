using System;
using System.IO;
using System.Security.Cryptography;

using Edger.Unity;

/*
 * http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp
 */
namespace Edger.Unity.Zip {
    public static class AesUtil {
        private static RijndaelManaged _Rijndael = new RijndaelManaged();

        public static byte[] Encrypt(byte[] buffer, string password) {
            var encryptor = _Rijndael.CreateEncryptor(
                                StringUtil.EncodeUtf8ToBytes(password),
                                GetVector(password));
            return Transform(buffer, encryptor);
        }

        public static byte[] Decrypt(byte[] buffer, string password) {
            var decryptor = _Rijndael.CreateDecryptor(
                                StringUtil.EncodeUtf8ToBytes(password),
                                GetVector(password));
            return Transform(buffer, decryptor);
        }

        /*
        * Use a simple algorithm to calculate vector from password, so no need to maintain another secret.
        */
        private static byte[] GetVector(string password) {
            byte[] md5 = StringUtil.EncodeUtf8ToBytes(Md5Util.Md5Sum(password));
            byte[] vector = new byte[16];
            Array.Copy(md5, vector, 16);
            return vector;
        }

        private static byte[] Transform(byte[] buffer, ICryptoTransform transform) {
            var stream = new MemoryStream();
            using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write)) {
                cs.Write(buffer, 0, buffer.Length);
            }

            return stream.ToArray();
        }
    }
}
