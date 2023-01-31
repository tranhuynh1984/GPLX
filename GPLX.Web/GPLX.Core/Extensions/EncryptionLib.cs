using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GPLX.Core.Extensions
{
    public static class EncryptionLib
    {
        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="ArgumentNullException">Text is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Password is null or empty.</exception>
        public static string EncryptStringAES(string plainText, string sharedSecret, byte[] salt)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText", "Text is null or empty.");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret", "Password is null or empty.");

            string outStr = null;
            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, salt);
                using (var aesAlg = new RijndaelManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(plainText);
                        outStr = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch { }
            return outStr;
        }
        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="ArgumentNullException">Text is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Password is null or empty.</exception>
        public static string DecryptStringAES(string cipherText, string sharedSecret, byte[] salt)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText", "Text is null or empty.");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret", "Password is null or empty.");

            string plaintext = null;
            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, salt);
                using (var aesAlg = new RijndaelManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    byte[] bytes = Convert.FromBase64String(cipherText);
                    using (MemoryStream msDecrypt = new MemoryStream(bytes))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        plaintext = srDecrypt.ReadToEnd();
                }
            }
            catch { }
            return plaintext;
        }

        public static string HashSHA1(string plainText)
        {
            using (var sha = SHA1Managed.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(plainText)));
            }
        }

        public static string HashSHA256(string plainText)
        {
            using (var sha = SHA256Managed.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(plainText)));
            }
        }

        public static string HashHMACMD5(string plainText, byte[] key)
        {
            using (HMACMD5 hmac = new HMACMD5(key))
            {
                var bytes = Encoding.ASCII.GetBytes(plainText);

                return Convert.ToBase64String(hmac.ComputeHash(bytes));
            }
        }
    }
}
