using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Adliance.AspNetCore.Buddy
{
    public static class Crypto
    {
        public static string HashWithoutSalt(string value)
        {
            return Hash(value, null);
        }

        public static string Hash(string value, string? salt)
        {
            var saltBytes = new byte[0];
            if (!string.IsNullOrEmpty(salt))
            {
                saltBytes = Convert.FromBase64String(salt);
            }

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(value, saltBytes, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
        }

        public static string Hash(string value, Guid salt)
        {
            var saltBytes = Encoding.Unicode.GetBytes(salt.ToString());
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(value, saltBytes, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
        }

        public static string Hash(string value, out string salt)
        {
            var saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            salt = Convert.ToBase64String(saltBytes);
            return Hash(value, salt);
        }
        
        public static string RandomString(int length)
        {
            var random = new Random();
            var sb = new StringBuilder();

            var availableCharacters = new List<char>();

            // get all characters and numbers from ASCII. Look it up: http://www.asciitable.com/.
            for (var i = 48; i <= 122; i++)
            {
                if ((i >= 58 && i <= 64) || (i >= 91 && i <= 96))
                {
                    continue;
                }

                availableCharacters.Add((char) i);
            }

            for (var i = 1; i <= length; i++)
            {
                sb.Append(availableCharacters[random.Next(0, availableCharacters.Count - 1)]);
            }

            return sb.ToString();
        }
    }
}