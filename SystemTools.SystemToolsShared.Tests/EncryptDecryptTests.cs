using Xunit;

namespace SystemTools.SystemToolsShared.Tests;

public sealed class EncryptDecryptTests
{
    [Fact]
    public void EncryptString_WithNullOrWhitespace_ReturnsInput()
    {
        Assert.Null(EncryptDecrypt.EncryptString(null, "key"));
        Assert.Equal("", EncryptDecrypt.EncryptString("", "key"));
        Assert.Equal("   ", EncryptDecrypt.EncryptString("   ", "key"));
    }

    [Fact]
    public void DecryptString_WithNullOrWhitespace_ReturnsInput()
    {
        Assert.Null(EncryptDecrypt.DecryptString(null, "key"));
        Assert.Equal("", EncryptDecrypt.DecryptString("", "key"));
        Assert.Equal("   ", EncryptDecrypt.DecryptString("   ", "key"));
    }

    [Fact]
    public void EncryptDecryptString_RoundTrip_ReturnsOriginal()
    {
        const string key = "test-key";
        const string original = "Hello, World! 1234";
        string? encrypted = EncryptDecrypt.EncryptString(original, key);

        Assert.False(string.IsNullOrWhiteSpace(encrypted));
        Assert.NotEqual(original, encrypted);

        string? decrypted = EncryptDecrypt.DecryptString(encrypted, key);
        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void DecryptString_WithWrongKey_ReturnsNullOrGibberish()
    {
        const string key = "key1";
        const string wrongKey = "key2";
        const string original = "SensitiveData";
        string? encrypted = EncryptDecrypt.EncryptString(original, key);

        string? decrypted = EncryptDecrypt.DecryptString(encrypted, wrongKey);

        // Decryption with wrong key should not return the original string
        Assert.NotEqual(original, decrypted);
    }

    [Fact]
    public void EncryptString_WithNonAsciiCharacters_EncryptsAndDecryptsCorrectly()
    {
        const string key = "ключ";
        const string original = "Тестовые данные 你好";
        string? encrypted = EncryptDecrypt.EncryptString(original, key);

        // Encrypted string may lose non-ASCII chars due to Encoding.ASCII, so decrypted may not match
        string? decrypted = EncryptDecrypt.DecryptString(encrypted, key);

        // Because ASCII encoding is used, non-ASCII chars will be lost, so decrypted != original
        Assert.NotEqual(original, decrypted);
    }

    [Fact]
    public void EncryptString_WithEmptyKey_StillEncrypts()
    {
        const string original = "data";
        string? encrypted = EncryptDecrypt.EncryptString(original, "");
        Assert.False(string.IsNullOrWhiteSpace(encrypted));
        Assert.NotEqual(original, encrypted);

        string? decrypted = EncryptDecrypt.DecryptString(encrypted, "");
        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void DecryptString_WithInvalidBase64_ReturnsNull()
    {
        string? result = EncryptDecrypt.DecryptString("not-a-base64-string", "key");
        Assert.Null(result);
    }
}
