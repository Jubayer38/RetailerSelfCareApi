using System.Security.Cryptography;

namespace Domain.StaticClass
{
    public class Cryptography
    {
        private static string SaltKey => "ZyFhKWhyNnsleipqdWNvW2d7JTEoI3poeX1bIWFqXXk=";
        private static string SaltIv => "VcPTDQyfynqvjdGB6IrTKA==";


        public static string Decryptor(string encryptedText)
        {
            string decrypted = null;
            byte[] cipher = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(SaltKey);
                aes.IV = Convert.FromBase64String(SaltIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

                using MemoryStream ms = new(cipher);
                using CryptoStream cs = new(ms, dec, CryptoStreamMode.Read);
                using (StreamReader sr = new(cs))
                {
                    decrypted = sr.ReadToEnd();
                }
            }

            return decrypted;
        }
    }
}