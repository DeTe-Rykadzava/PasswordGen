using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MyPasswordGenLibrary.Interfaces;

namespace MyPasswordGenLibrary
{
    public class Encryption : IEncryption
    {
        public string Encrypt(string originalText, byte[] key, byte[] iv)
        {
            var encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key.Take(32).ToArray();
            encryptor.IV = iv;

            var ms = new MemoryStream();
            var aesEncryptor = encryptor.CreateEncryptor();
            var cryptoStream = new CryptoStream(ms, aesEncryptor, CryptoStreamMode.Write);

            var textBytes = Encoding.UTF8.GetBytes(originalText);
            cryptoStream.Write(textBytes, 0, textBytes.Length);
            cryptoStream.FlushFinalBlock();

            var cipherBytes = ms.ToArray();
            ms.Close();
            cryptoStream.Close();

            var cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }

        public string Decrypt(string encryptedText, byte[] key, byte[] iv)
        {
            var encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key.Take(32).ToArray();
            encryptor.IV = iv;

            var ms = new MemoryStream();
            var aesDecryptor = encryptor.CreateDecryptor();
            var cryptoStream = new CryptoStream(ms, aesDecryptor, CryptoStreamMode.Write);

            var originalText = String.Empty;

            try
            {
                var cipherBytes = Convert.FromBase64String(encryptedText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();

                var originalBytes = ms.ToArray();
                originalText = Encoding.UTF8.GetString(originalBytes, 0, originalBytes.Length);
            }
            finally
            {
                ms.Close();
                cryptoStream.Close();
            }

            return originalText;
        }
    }
}