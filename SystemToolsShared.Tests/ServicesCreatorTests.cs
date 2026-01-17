using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class ServicesCreatorTests : IDisposable
{
    private readonly string _testAppName;
    private readonly string _testLogFolder;

    public ServicesCreatorTests()
    {
        _testLogFolder = Path.Combine(Path.GetTempPath(), "ServicesCreatorTests_" + Guid.NewGuid());
        _testAppName = "TestApp";
        Directory.CreateDirectory(_testLogFolder);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testLogFolder))
        {
            Directory.Delete(_testLogFolder, true);
        }
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        var creator = new ServicesCreator(_testLogFolder, null, _testAppName);
        Assert.NotNull(creator);
    }

    [Fact]
    public void CreateServiceProvider_WithLogFileName_CreatesProviderAndLogger()
    {
        var logFileName = Path.Combine(_testLogFolder, "test.log");
        var creator = new ServicesCreator(null, logFileName, _testAppName);
        // ReSharper disable once using
        using var provider = creator.CreateServiceProvider(LogEventLevel.Information);
        Assert.NotNull(provider);
        var logger = provider.GetService<ILogger<ServicesCreatorTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void CreateServiceProvider_WithLogFolder_CreatesProviderAndLogger()
    {
        var creator = new ServicesCreator(_testLogFolder, null, _testAppName);
        // ReSharper disable once using
        using var provider = creator.CreateServiceProvider(LogEventLevel.Warning);
        Assert.NotNull(provider);
        var logger = provider.GetService<ILogger<ServicesCreatorTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void CreateServiceProvider_WithNoLogConfig_CreatesProvider()
    {
        var creator = new ServicesCreator(null, null, _testAppName);
        // ReSharper disable once using
        using var provider = creator.CreateServiceProvider(LogEventLevel.Error);
        Assert.NotNull(provider);
    }

    //[Fact]
    //public void CreateServiceProvider_InvalidLogPath_ReturnsNull()
    //{
    //    var invalidFolder = Path.Combine(_testLogFolder, "?:\\invalid|path");
    //    var creator = new ServicesCreator(invalidFolder, null, _testAppName);
    //    var provider = creator.CreateServiceProvider(LogEventLevel.Information);
    //    Assert.Null(provider);
    //}

    [Fact]
    public void ConfigureServices_AddsLogging()
    {
        var creator = new TestServicesCreator(_testLogFolder, null, _testAppName);
        var services = new ServiceCollection();
        creator.TestConfigureServices(services);
        // ReSharper disable once using
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetService<ILogger<ServicesCreatorTests>>();
        Assert.NotNull(logger);
    }

    // Helper to expose protected ConfigureServices
    private sealed class TestServicesCreator : ServicesCreator
    {
        public TestServicesCreator(string? logFolder, string? logFileName, string appName) : base(logFolder,
            logFileName, appName)
        {
        }

        public void TestConfigureServices(IServiceCollection services)
        {
            ConfigureServices(services);
        }
    }
}
