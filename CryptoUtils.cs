using System;
using System.Security.Cryptography;
using System.Text;

namespace ZIMAeTicket
{
    static class CryptoUtils
    {
        public static string Hash(string value)
        {
            StringBuilder stringBuilder = new();

            using (var hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                foreach (byte b in bytes)
                    stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
