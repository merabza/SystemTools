using System.Threading.Tasks;
using ApiToolsShared.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable ReplaceWithPrimaryConstructorParameter

namespace ApiToolsShared;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ApiKeysChecker(ILoggerFactory loggerFactory, IConfiguration configuration) : IEndpointFilter
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = loggerFactory.CreateLogger<ApiKeysChecker>();
    public const string ApiKeyParameterName = "ApiKey";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var apiKey = context.HttpContext.Request.Query[ApiKeyParameterName].ToString();
        var remoteAddress = context.HttpContext.Connection.RemoteIpAddress;

        if (remoteAddress is null)
            return Results.BadRequest(new[] { ApiErrors.InvalidRemoteAddress });

        if (!Check(apiKey, remoteAddress.MapToIPv4().ToString()))
            return Results.BadRequest(new[] { ApiErrors.ApiKeyIsInvalid });

        return await next(context);
    }

    private bool Check(string? apiKey, string remoteIpAddress)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        var apiKeys = ApiKeysDomain.Create(_configuration, _logger);

        //_logger.LogInformation($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
        //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
        //{
        //    _logger.LogInformation($"RemoteIpAddress is - {key.RemoteIpAddress}");
        //    _logger.LogInformation($"ApiKey is - {key.ApiKey}");
        //}
        //_logger.LogInformation("View Api Keys Finished");

        var ak = apiKeys.AppSettingsByApiKey(apiKey, remoteIpAddress);


        if (ak != null)
            return true;
        _logger.LogError("RemoteIpAddress is - {remoteIpAddress}", remoteIpAddress);
        _logger.LogError("API Key is invalid - {apiKey}", apiKey);
        return false;
    }
}