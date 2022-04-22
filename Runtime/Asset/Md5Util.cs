using System;
using System.Collections;
using System.Collections.Generic;

namespace Edger.Unity {
    public static class Md5Util {
        // c/o http://wiki.unity3d.com/index.php?title=MD5
        public static string Md5Sum(byte[] bytes) {
            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++) {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public static string Md5Sum(string strToEncrypt) {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);
            return Md5Sum(bytes);
        }
    }
}

