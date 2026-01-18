using System;
using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class FileLoaderTests : IDisposable
{
    private readonly string _testFilePath;

    public FileLoaderTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public void LoadDeserializeResolve_ValidJson_ReturnsDeserializedObject()
    {
        var model = new TestModel { Name = "Test", Value = 42 };
        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(model));

        var result = FileLoader.LoadDeserializeResolve<TestModel>(_testFilePath, false);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void LoadDeserializeResolve_FileNotFound_ReturnsDefault()
    {
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        var result = FileLoader.LoadDeserializeResolve<TestModel>(nonExistentFile, false);
        Assert.Null(result);
    }

    [Fact]
    public void LoadDeserializeResolve_InvalidJson_ReturnsDefault()
    {
        File.WriteAllText(_testFilePath, "{ invalid json }");
        var result = FileLoader.LoadDeserializeResolve<TestModel>(_testFilePath, false);
        Assert.Null(result);
    }

    [Fact]
    public void LoadDeserializeResolve_EmptyFile_ReturnsDefault()
    {
        File.WriteAllText(_testFilePath, "");
        var result = FileLoader.LoadDeserializeResolve<TestModel>(_testFilePath, false);
        Assert.Null(result);
    }

    private sealed class TestModel
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }
}
