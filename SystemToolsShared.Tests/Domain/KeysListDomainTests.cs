using System;
using System.IO;
using Xunit;
using SystemToolsShared.Domain;
using SystemToolsShared.Models;
using Newtonsoft.Json;

namespace SystemToolsShared.Tests.Domain;

public sealed class KeysListDomainTests : IDisposable
{
    private readonly string _testFilePath;

    public KeysListDomainTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), "test_keys.json");
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LoadFromFile_WithInvalidFileName_ReturnsNull(string filename)
    {
        // Act
        var result = KeysListDomain.LoadFromFile(filename);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void LoadFromFile_WithValidKeysFile_LoadsKeys()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", "key2", "key3"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        var result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Keys.Count);
        Assert.Contains("key1", result.Keys);
        Assert.Contains("key2", result.Keys);
        Assert.Contains("key3", result.Keys);
    }

    [Fact]
    public void LoadFromFile_WithEmptyKeysList_ReturnsNull()
    {
        // Arrange
        var keysList = new KeysList { Keys = [] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        var result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Keys);
    }

    [Fact]
    public void LoadFromFile_WithNullKeys_ReturnsNull()
    {
        // Arrange
        var keysList = new KeysList { Keys = null };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        var result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void LoadFromFile_WithMalformedJson_ThrowsJsonException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "{ invalid json }");

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => KeysListDomain.LoadFromFile(_testFilePath));
    }

    [Fact]
    public void LoadFromFile_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent.json");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => KeysListDomain.LoadFromFile(nonExistentFile));
    }
}