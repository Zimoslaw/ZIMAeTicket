using System.Security.Cryptography;

namespace ZIMAeTicket
{
    static class CryptoUtils
    {
        public static string Hash(string value)
        {
            StringBuilder stringBuilder = new();

            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));

            foreach (byte b in bytes)
                stringBuilder.Append(b.ToString("x2"));

            return stringBuilder.ToString();
        }

        public static string RandomLowerCaseString(int length) 
        {
            StringBuilder stringBuilder = new();
            Random random = new();
            
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append((char)random.Next(97, 123));
            }

            return stringBuilder.ToString();
        }
    }
}
