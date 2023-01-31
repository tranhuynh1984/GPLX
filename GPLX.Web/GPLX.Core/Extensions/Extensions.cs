using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GPLX.Core.Enum;
using GPLX.Core.Model;

namespace GPLX.Core.Extensions
{
    public static class Extensions
    {
        public static string StringAesEncryption(this string text, string salt)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(salt))
                return text;
            try
            {
                string encrypted = Encrypt(text, GlobalEnums.PrivateKey, salt, initialVector: GlobalEnums.Vector);
                return encrypted;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        public static string StringAesDecryption(this string text, string salt, bool onParam = true)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                text = onParam ? text.Replace("-", "+") : text;
                return Decrypt(text, GlobalEnums.PrivateKey, salt, initialVector: GlobalEnums.Vector);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string StringUnSign(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            string[] unicodeChars = { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] normalizeChars = { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y",};
            for (int i = 0; i < unicodeChars.Length; i++)
            {
                text = text.Replace(unicodeChars[i], normalizeChars[i]);
                text = text.Replace(unicodeChars[i].ToUpper(), normalizeChars[i].ToUpper());
            }
            return text;
        }

        #region Static Functions

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="password">Password to encrypt with</param>
        /// <param name="salt">Salt to encrypt with</param>
        /// <param name="hashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="passwordIterations">Number of iterations to do</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 128, 192, or 256</param>
        /// <returns>An encrypted string</returns>
        static string Encrypt(string plainText, string password,
            string salt, string hashAlgorithm = "SHA1",
            int passwordIterations = 2, string initialVector = "",
            int keySize = 256)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";
            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            byte[] cipherTextBytes;
            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
            {
                using MemoryStream memStream = new MemoryStream();
                using CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memStream.ToArray();
                memStream.Close();
                cryptoStream.Close();
            }
            symmetricKey.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="cipherText">Text to be decrypted</param>
        /// <param name="password">Password to decrypt with</param>
        /// <param name="salt">Salt to decrypt with</param>
        /// <param name="hashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="passwordIterations">Number of iterations to do</param>
        /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="keySize">Can be 128, 192, or 256</param>
        /// <returns>A decrypted string</returns>
         static string Decrypt(string cipherText, string password,
            string salt = "", string hashAlgorithm = "SHA1",
            int passwordIterations = 2, string initialVector = "",
            int keySize = 256)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";
            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;
            using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
            {
                using MemoryStream memStream = new MemoryStream(cipherTextBytes);
                using CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memStream.Close();
                cryptoStream.Close();
            }
            symmetricKey.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }

        #endregion

        public static IList<WeekInYear> DbWeekInYear(this int year)
        {
            var jan1 = new DateTime(year, 1, 1);
            //beware different cultures, see other answers
            var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            var weeks =
                Enumerable
                    .Range(0, 54)
                    .Select(i => new {
                        weekStart = startOfFirstWeek.AddDays(i * 7)
                    })
                    .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                    .Select(x => new {
                        x.weekStart,
                        weekFinish = x.weekStart.AddDays(6)
                    })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new WeekInYear
                    {
                        weekStart = x.weekStart,
                        weekFinish = x.weekFinish,
                        weekNum = i + 1
                    }).ToList();

            return weeks;
        }


        public static bool EqualsCheckNull(this string str,string input)
        {
            return !string.IsNullOrEmpty(str) ? str.Equals(input) : false;
        }

        public static DateTime FirstDateOfWeek(this int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        public static List<T> CreateList<T>(params T[] elements)
        {
            return new List<T>(elements);
        }
    }

}
