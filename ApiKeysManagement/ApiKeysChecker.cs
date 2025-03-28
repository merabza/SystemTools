//using System.Threading.Tasks;
//using ApiContracts;
//using ApiContracts.Errors;
//using ApiKeysManagement.Domain;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//namespace ApiKeysManagement;

//// ReSharper disable once UnusedType.Global
//public sealed class ApiKeysChecker : IEndpointFilter
//{
//    private readonly IConfiguration _configuration;
//    private readonly ILogger _logger;

//    // ReSharper disable once ConvertToPrimaryConstructor
//    public ApiKeysChecker(ILoggerFactory loggerFactory, IConfiguration configuration)
//    {
//        _configuration = configuration;
//        _logger = loggerFactory.CreateLogger<ApiKeysChecker>();
//    }

//    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
//    {
//        var apiKey = context.HttpContext.Request.Query[ApiKeysConstants.ApiKeyParameterName].ToString();
//        var remoteAddress = context.HttpContext.Connection.RemoteIpAddress;

//        if (remoteAddress is null)
//            return Results.BadRequest(new[] { ApiErrors.InvalidRemoteAddress });

//        if (!Check(apiKey, remoteAddress.MapToIPv4().ToString()))
//            return Results.BadRequest(new[] { ApiErrors.ApiKeyIsInvalid });

//        return await next(context);
//    }

//    private bool Check(string? apiKey, string remoteIpAddress)
//    {
//        if (string.IsNullOrWhiteSpace(apiKey))
//            return false;

//        var apiKeys = ApiKeysDomain.Create(_configuration, _logger);

//        //var apiKeysCount = apiKeys.ApiKeys.Count;
//        //_logger.LogInformation("View Api Keys. count is - {apiKeysCount}", apiKeysCount);
//        //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
//        //{
//        //    var keyRemoteIpAddress = key.RemoteIpAddress;
//        //    _logger.LogInformation("RemoteIpAddress is - {keyRemoteIpAddress}", keyRemoteIpAddress);
//        //    var keyApiKey = key.ApiKey;
//        //    _logger.LogInformation("ApiKey is - {keyApiKey}", keyApiKey);
//        //}

//        //_logger.LogInformation("View Api Keys Finished");

//        var ak = apiKeys.AppSettingsByApiKey(apiKey, remoteIpAddress);

//        if (ak != null)
//            return true;
//        _logger.LogError("RemoteIpAddress is - {remoteIpAddress}", remoteIpAddress);
//        _logger.LogError("API Key is invalid - {apiKey}", apiKey);
//        return false;
//    }
//}