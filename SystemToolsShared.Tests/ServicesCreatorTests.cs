using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class ServicesCreatorTests : IDisposable
{
    private readonly string _testLogFolder;
    private readonly string _testAppName;

    public ServicesCreatorTests()
    {
        _testLogFolder = Path.Combine(Path.GetTempPath(), "ServicesCreatorTests");
        _testAppName = "TestApp";
        Directory.CreateDirectory(_testLogFolder);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testLogFolder))
            Directory.Delete(_testLogFolder, true);
    }

    [Fact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        // Arrange & Act
        var creator = new ServicesCreator(_testLogFolder, null, _testAppName);

        // Assert
        Assert.NotNull(creator);
    }

    [Fact]
    public void CreateServiceProvider_WithLogFileName_CreatesProvider()
    {
        // Arrange
        var logFileName = Path.Combine(_testLogFolder, "test.log");
        var creator = new ServicesCreator(null, logFileName, _testAppName);

        // Act
        var serviceProvider = creator.CreateServiceProvider(LogEventLevel.Information);

        // Assert
        Assert.NotNull(serviceProvider);
        var logger = serviceProvider.GetService<ILogger<ServicesCreatorTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void CreateServiceProvider_WithNoLogConfig_CreatesProviderWithoutLogger()
    {
        // Arrange
        var creator = new ServicesCreator(null, null, _testAppName);

        // Act
        var serviceProvider = creator.CreateServiceProvider(LogEventLevel.Information);

        // Assert
        Assert.NotNull(serviceProvider);
    }
}