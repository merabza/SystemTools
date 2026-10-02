using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SystemTools.ApiKeysManagement.Domain;
using SystemTools.ApiKeysManagement.Tests.TestDoubles;

namespace SystemTools.ApiKeysManagement.Tests.Domain;

//Reading the keys from the configuration and finding the entry of a key and a client address
public sealed class ApiKeysDomainTests
{
    private const string KeyA = "key-a";
    private const string KeyB = "key-b";
    private const string AddressA = "10.1.2.3";
    private const string AddressB = "10.1.2.4";

    private static ApiKeysDomain Create(params (string? ApiKey, string? RemoteIpAddress)[] entries)
    {
        return ApiKeysDomain.Create(ApiKeysConfiguration.Create(entries), NullLogger.Instance);
    }

    [Fact]
    public void Create_ReturnsNoKeys_WhenTheSectionIsMissing()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();

        ApiKeysDomain apiKeys = ApiKeysDomain.Create(configuration, NullLogger.Instance);

        Assert.Empty(apiKeys.ApiKeys);
    }

    [Fact]
    public void Create_ReturnsNoKeys_WhenTheSectionHasNoEntries()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ApiKeys:Other"] = "value" }).Build();
        var logger = new CollectingLogger();

        ApiKeysDomain apiKeys = ApiKeysDomain.Create(configuration, logger);

        Assert.Empty(apiKeys.ApiKeys);
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void Create_ReadsEveryEntry()
    {
        ApiKeysDomain apiKeys = Create((KeyA, AddressA), (KeyB, ApiKeysDomain.AnyRemoteIpAddress));

        Assert.Equal(2, apiKeys.ApiKeys.Count);
        Assert.Contains(apiKeys.ApiKeys, k => k is { ApiKey: KeyA, RemoteIpAddress: AddressA });
        Assert.Contains(apiKeys.ApiKeys, k => k is { ApiKey: KeyB, RemoteIpAddress: "*" });
    }

    [Theory]
    [InlineData(null, AddressA)]
    [InlineData(KeyA, null)]
    public void Create_SkipsAnIncompleteEntry(string? apiKey, string? remoteIpAddress)
    {
        ApiKeysDomain apiKeys = Create((apiKey, remoteIpAddress), (KeyB, AddressB));

        ApiKeyAndRemoteIpAddressDomain entry = Assert.Single(apiKeys.ApiKeys);
        Assert.Equal(KeyB, entry.ApiKey);
    }

    [Fact]
    public void Create_LogsAnEntryWithoutAddress_WithTheLengthOfTheKeyInsteadOfTheKey()
    {
        var logger = new CollectingLogger();

        ApiKeysDomain.Create(ApiKeysConfiguration.Create((KeyA, null)), logger);

        (LogLevel level, string message) = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, level);
        Assert.Equal("Invalid ApiKey or RemoteIpAddress: ApiKey length is 5 for (null)", message);
    }

    [Fact]
    public void Create_LogsAnEntryWithoutKey_WithItsAddress()
    {
        var logger = new CollectingLogger();

        ApiKeysDomain.Create(ApiKeysConfiguration.Create((null, AddressA)), logger);

        (LogLevel level, string message) = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, level);
        Assert.Equal("Invalid ApiKey or RemoteIpAddress: ApiKey length is (null) for 10.1.2.3", message);
    }

    [Fact]
    public void Create_LogsNothing_WhenTheErrorLevelIsDisabled()
    {
        var logger = new CollectingLogger(false);

        ApiKeysDomain apiKeys = ApiKeysDomain.Create(ApiKeysConfiguration.Create((KeyA, null)), logger);

        Assert.Empty(apiKeys.ApiKeys);
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void AppSettingsByApiKey_ReturnsTheEntryOfTheKeyAndTheAddress()
    {
        ApiKeysDomain apiKeys = Create((KeyA, AddressA), (KeyB, AddressB));

        ApiKeyAndRemoteIpAddressDomain? entry = apiKeys.AppSettingsByApiKey(KeyB, AddressB);

        Assert.NotNull(entry);
        Assert.Equal(KeyB, entry.ApiKey);
        Assert.Equal(AddressB, entry.RemoteIpAddress);
    }

    [Fact]
    public void AppSettingsByApiKey_ReturnsNull_ForAnotherAddress()
    {
        ApiKeysDomain apiKeys = Create((KeyA, AddressA));

        Assert.Null(apiKeys.AppSettingsByApiKey(KeyA, AddressB));
    }

    [Fact]
    public void AppSettingsByApiKey_ReturnsNull_ForAnUnknownKey()
    {
        ApiKeysDomain apiKeys = Create((KeyA, AddressA));

        Assert.Null(apiKeys.AppSettingsByApiKey(KeyB, AddressA));
    }

    [Fact]
    public void AppSettingsByApiKey_ComparesTheKeyCaseSensitively()
    {
        ApiKeysDomain apiKeys = Create((KeyA, ApiKeysDomain.AnyRemoteIpAddress));

        Assert.Null(apiKeys.AppSettingsByApiKey("KEY-A", AddressA));
    }

    [Theory]
    [InlineData(AddressA)]
    [InlineData(AddressB)]
    [InlineData("192.168.0.10")]
    public void AppSettingsByApiKey_AcceptsAnyAddress_ForAnAsteriskEntry(string remoteIpAddress)
    {
        ApiKeysDomain apiKeys = Create((KeyA, ApiKeysDomain.AnyRemoteIpAddress));

        ApiKeyAndRemoteIpAddressDomain? entry = apiKeys.AppSettingsByApiKey(KeyA, remoteIpAddress);

        Assert.NotNull(entry);
        Assert.Equal(KeyA, entry.ApiKey);
        Assert.Equal("*", entry.RemoteIpAddress);
    }

    [Fact]
    public void AppSettingsByApiKey_DoesNotAcceptAnotherKey_ForAnAsteriskEntry()
    {
        ApiKeysDomain apiKeys = Create((KeyA, ApiKeysDomain.AnyRemoteIpAddress));

        Assert.Null(apiKeys.AppSettingsByApiKey(KeyB, AddressA));
    }

    [Fact]
    public void AppSettingsByApiKey_PrefersTheEntryOfTheAddress_ToTheAsteriskEntry()
    {
        ApiKeysDomain apiKeys = Create((KeyA, ApiKeysDomain.AnyRemoteIpAddress), (KeyA, AddressA));

        ApiKeyAndRemoteIpAddressDomain? entry = apiKeys.AppSettingsByApiKey(KeyA, AddressA);

        Assert.NotNull(entry);
        Assert.Equal(AddressA, entry.RemoteIpAddress);
    }

    [Fact]
    public void AppSettingsByApiKey_UsesTheAsteriskEntry_WhenTheEntryOfTheKeyIsForAnotherAddress()
    {
        ApiKeysDomain apiKeys = Create((KeyA, AddressA), (KeyA, ApiKeysDomain.AnyRemoteIpAddress));

        ApiKeyAndRemoteIpAddressDomain? entry = apiKeys.AppSettingsByApiKey(KeyA, AddressB);

        Assert.NotNull(entry);
        Assert.Equal("*", entry.RemoteIpAddress);
    }

    [Theory]
    [InlineData("10.1.2.*")]
    [InlineData("**")]
    [InlineData(" * ")]
    [InlineData("")]
    public void AppSettingsByApiKey_TreatsOnlyAnAsteriskAsAnyAddress(string remoteIpAddress)
    {
        ApiKeysDomain apiKeys = Create((KeyA, remoteIpAddress));

        Assert.Null(apiKeys.AppSettingsByApiKey(KeyA, AddressA));
    }
}
