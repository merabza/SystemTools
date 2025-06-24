using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class JsonConfigurationSourceTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var fileProvider = new Mock<IFileProvider>().Object;
        const string path = "settings.json";
        const bool optional = true;
        const bool reloadOnChange = true;
        const string key = "TestKey";
        const string appSetEnKeysFileName = "appsettings.keys.json";

        // Act
        var source =
            new JsonConfigurationSource(fileProvider, path, optional, reloadOnChange, key, appSetEnKeysFileName);

        // Assert
        Assert.Equal(fileProvider, source.FileProvider);
        Assert.Equal(path, source.Path);
        Assert.Equal(optional, source.Optional);
        Assert.Equal(reloadOnChange, source.ReloadOnChange);
        Assert.Equal(key, source.Key);
        Assert.Equal(appSetEnKeysFileName, source.AppSetEnKeysFileName);
    }
}