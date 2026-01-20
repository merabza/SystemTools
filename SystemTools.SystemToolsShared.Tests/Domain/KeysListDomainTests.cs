using System;
using System.IO;
using Newtonsoft.Json;
using SystemTools.SystemToolsShared.Domain;
using SystemTools.SystemToolsShared.Models;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests.Domain;

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
        {
            File.Delete(_testFilePath);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void LoadFromFile_WithInvalidFileName_ReturnsNull(string filename)
    {
        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(filename);

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
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

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
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

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
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

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
        string nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent.json");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => KeysListDomain.LoadFromFile(nonExistentFile));
    }

    // Constructor tests (testing through LoadFromFile since constructor is private)

    [Fact]
    public void Constructor_InitializesKeysProperty_WithProvidedList()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", "key2"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Keys);
        Assert.Equal(2, result.Keys.Count);
    }

    [Fact]
    public void Constructor_InitializesKeysProperty_AsNonNull()
    {
        // Arrange
        var keysList = new KeysList { Keys = [] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Keys);
    }

    [Fact]
    public void Constructor_PreservesKeyOrder_FromInputList()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["first", "second", "third", "fourth"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("first", result.Keys[0]);
        Assert.Equal("second", result.Keys[1]);
        Assert.Equal("third", result.Keys[2]);
        Assert.Equal("fourth", result.Keys[3]);
    }

    [Fact]
    public void Constructor_AllowsDuplicateKeys()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", "key2", "key1", "key3"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Keys.Count);
        Assert.Equal(2, result.Keys.FindAll(k => k == "key1").Count);
    }

    [Fact]
    public void Constructor_FiltersOutNullValues_FromInputList()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", null, "key2", null, "key3"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Keys.Count);
        Assert.DoesNotContain(null, result.Keys);
        Assert.Contains("key1", result.Keys);
        Assert.Contains("key2", result.Keys);
        Assert.Contains("key3", result.Keys);
    }

    [Fact]
    public void KeysProperty_IsMutable_AllowsModification()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", "key2"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Act
        result!.Keys.Add("key3");

        // Assert
        Assert.Equal(3, result.Keys.Count);
        Assert.Contains("key3", result.Keys);
    }

    [Fact]
    public void KeysProperty_CanBeReassigned()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", "key2"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Act
        result!.Keys = ["newKey1", "newKey2", "newKey3"];

        // Assert
        Assert.Equal(3, result.Keys.Count);
        Assert.Contains("newKey1", result.Keys);
        Assert.DoesNotContain("key1", result.Keys);
    }

    [Fact]
    public void Constructor_HandlesEmptyStringKeys()
    {
        // Arrange
        var keysList = new KeysList { Keys = ["key1", "", "key2", "   ", "key3"] };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(keysList));

        // Act
        KeysListDomain? result = KeysListDomain.LoadFromFile(_testFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Keys.Count);
        Assert.Contains("", result.Keys);
        Assert.Contains("   ", result.Keys);
    }
}
