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
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            byte[] aesKey = new byte[32];
            Array.Copy(hash, aesKey, 32);

            // ReSharper disable once using
            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC; // Changed from ECB to CBC
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // Generate a random IV

            byte[] iv = aes.IV;
            byte[] buff = Encoding.UTF8.GetBytes(str);
            byte[] encrypted = aes.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length);

            // Prepend IV to the encrypted data
            byte[] resultBytes = new byte[iv.Length + encrypted.Length];
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
            //#pragma warning disable CA5351
            //#pragma warning disable CA2000
            //            MD5 md5 = MD5.Create();
            //#pragma warning restore CA2000
            //#pragma warning restore CA5351
            //#pragma warning disable CA1850
            //            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
            //#pragma warning restore CA1850
            //#pragma warning disable S5547
            //#pragma warning disable CA5350
            //#pragma warning disable CA5350
            //#pragma warning disable CA2000
            //            TripleDES tripleDes = TripleDES.Create();
            //#pragma warning restore CA2000
            //#pragma warning restore CA5350
            //#pragma warning restore CA5350
            //#pragma warning restore S5547
            //            tripleDes.Key = hash;
            //#pragma warning disable CA5358
            //            tripleDes.Mode = CipherMode.ECB;
            //#pragma warning restore CA5358
            //            tripleDes.Padding = PaddingMode.PKCS7;
            //            byte[] buff = Encoding.ASCII.GetBytes(str);
            //            result = Convert.ToBase64String(tripleDes.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));


            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            byte[] aesKey = new byte[32];
            Array.Copy(hash, aesKey, 32);

            byte[] fullCipher = Convert.FromBase64String(str);

            // ReSharper disable once using
            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC; // Changed from ECB to CBC
            aes.Padding = PaddingMode.PKCS7;

            // Extract IV from the beginning of the cipher text
            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            // ReSharper disable once using
            using ICryptoTransform transform = aes.CreateDecryptor();
            result = Encoding.UTF8.GetString(transform.TransformFinalBlock(cipher, 0, cipher.Length));
        }
        catch
        {
            // ignored
        }

        return result;
    }
}
