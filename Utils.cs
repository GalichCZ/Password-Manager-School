using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace password_manager
{
    public class Utils
    {
        public static string CheckPasswordDifficult(string password)
        {
            int count = 1;
            string weakness = string.Empty;
            bool isLong = password.Length == 13 ? true : false;
            if(!isLong)
            {
                weakness += $"{count}. Your password is too short, use at least 13 symbols\n";
                count++;
            }

            bool hasDigits = Regex.IsMatch(password, @"\d");
            if(!hasDigits)
            {
                weakness += $"{count}. Your password doesn't contain digits\n";
                count++;
            }

            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            if(!hasUpperCase)
            {
                weakness += $"{count}. Your password doesn't contain upper case letters\n";
                count++;
            }

            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            if(!hasLowerCase)
            {
                weakness += $"{count}. Your password doesn't contain lower case letters\n";
                count++;
            }

            bool hasSpecialChars = Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            if(!hasSpecialChars)
            {
                weakness += $"{count}. Your password doesn't contain special characters\n";
            }

            if(isLong && hasDigits && hasUpperCase && hasLowerCase && hasSpecialChars)
            {
                weakness = "Your password is strong enough";
            }

            return weakness;
        }

        public static string EncryptPassword(string key, string password)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(password);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptPassword(string key, string password)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(password);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
