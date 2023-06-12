using System;
using System.Security.Cryptography;
using System.Text;

namespace SystemToolsShared;

public static class EncryptDecrypt
{
    public static string? EncryptString(string? str, string key)
    {
        if (string.IsNullOrWhiteSpace(str))
            return str;
        string? result = null;
        try
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
            var tripleDes = TripleDES.Create();
            tripleDes.Key = hash;
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;
            var buff = Encoding.ASCII.GetBytes(str);
            result = Convert.ToBase64String(tripleDes.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));
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
            return str;
        string? result = null;
        try
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
            var tripleDes = TripleDES.Create();
            tripleDes.Key = hash;
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;
            var buff = Convert.FromBase64String(str);
            var transform = tripleDes.CreateDecryptor();
            result = Encoding.ASCII.GetString(transform.TransformFinalBlock(buff, 0, buff.Length));
        }
        catch
        {
            // ignored
        }

        return result;
    }
}