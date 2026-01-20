using System;
using System.Security.Cryptography;
using System.Text;

namespace SystemTools.SystemToolsShared;

public static class EncryptDecrypt
{
    public static string? EncryptString(string? str, string key)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        string? result = null;
        try
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            var aesKey = new byte[32];
            Array.Copy(hash, aesKey, 32);

            // ReSharper disable once using
            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC; // Changed from ECB to CBC
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // Generate a random IV

            var iv = aes.IV;
            var buff = Encoding.UTF8.GetBytes(str);
            var encrypted = aes.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length);

            // Prepend IV to the encrypted data
            var resultBytes = new byte[iv.Length + encrypted.Length];
            Array.Copy(iv, 0, resultBytes, 0, iv.Length);
            Array.Copy(encrypted, 0, resultBytes, iv.Length, encrypted.Length);

            result = Convert.ToBase64String(resultBytes);
        }
        catch
        {
            // ignored
        }

        return result;
    }

    public static string? DecryptString(string? str, string key)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        string? result = null;
        try
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            var aesKey = new byte[32];
            Array.Copy(hash, aesKey, 32);

            var fullCipher = Convert.FromBase64String(str);

            // ReSharper disable once using
            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC; // Changed from ECB to CBC
            aes.Padding = PaddingMode.PKCS7;

            // Extract IV from the beginning of the cipher text
            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            // ReSharper disable once using
            using var transform = aes.CreateDecryptor();
            result = Encoding.UTF8.GetString(transform.TransformFinalBlock(cipher, 0, cipher.Length));
        }
        catch
        {
            // ignored
        }

        return result;
    }
}
