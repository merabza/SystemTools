using Microsoft.Extensions.DependencyInjection;

namespace SystemToolsShared.Tests;

public sealed class TestServicesCreator : ServicesCreator
{
    public TestServicesCreator(string? logFolder, string? logFileName, string appName) 
        : base(logFolder, logFileName, appName)
    {
    }

    public IServiceCollection? LastConfiguredServices { get; private set; }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        LastConfiguredServices = services;
    }
}