using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using SystemTools.ApiKeysManagement.Domain;
using SystemTools.ApiKeysManagement.Tests.TestDoubles;

namespace SystemTools.ApiKeysManagement.Tests;

//The finder that the API key authentication handler uses: the keys come from the configuration of the host
public sealed class ApiKeyByConfigFinderTests
{
    private const string Key = "key-a";
    private const string Address = "10.1.2.3";

    private static ApiKeyByConfigFinder CreateFinder(IConfiguration configuration)
    {
        return new ApiKeyByConfigFinder(NullLogger<ApiKeyByConfigFinder>.Instance, configuration);
    }

    [Fact]
    public async Task GetApiKeyAndRemAddress_ReturnsTheEntryOfTheKeyAndTheAddress()
    {
        ApiKeyByConfigFinder finder = CreateFinder(ApiKeysConfiguration.Create((Key, Address)));

        ApiKeyAndRemoteIpAddressDomain? entry = await finder.GetApiKeyAndRemAddress(Key, Address);

        Assert.NotNull(entry);
        Assert.Equal(Key, entry.ApiKey);
        Assert.Equal(Address, entry.RemoteIpAddress);
    }

    [Fact]
    public async Task GetApiKeyAndRemAddress_AcceptsAnyAddress_ForAnAsteriskEntry()
    {
        ApiKeyByConfigFinder finder =
            CreateFinder(ApiKeysConfiguration.Create((Key, ApiKeysDomain.AnyRemoteIpAddress)));

        ApiKeyAndRemoteIpAddressDomain? entry = await finder.GetApiKeyAndRemAddress(Key, Address);

        Assert.NotNull(entry);
        Assert.Equal("*", entry.RemoteIpAddress);
    }

    [Fact]
    public async Task GetApiKeyAndRemAddress_ReturnsNull_ForAnInvalidKey()
    {
        ApiKeyByConfigFinder finder = CreateFinder(ApiKeysConfiguration.Create((Key, Address)));

        Assert.Null(await finder.GetApiKeyAndRemAddress("key-b", Address));
    }

    [Fact]
    public async Task GetApiKeyAndRemAddress_ReadsTheConfigurationOnEveryCall()
    {
        IConfigurationRoot configuration = ApiKeysConfiguration.Create((Key, Address));
        ApiKeyByConfigFinder finder = CreateFinder(configuration);
        Assert.NotNull(await finder.GetApiKeyAndRemAddress(Key, Address));

        configuration["ApiKeys:AppSettingsByApiKey:0:ApiKey"] = "key-b";

        Assert.Null(await finder.GetApiKeyAndRemAddress(Key, Address));
        Assert.NotNull(await finder.GetApiKeyAndRemAddress("key-b", Address));
    }
}
