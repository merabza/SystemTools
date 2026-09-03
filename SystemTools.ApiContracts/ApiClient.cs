using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SystemTools.ApiContracts.Errors;
using SystemTools.SharedKernel;
using SystemTools.SystemToolsShared;

namespace SystemTools.ApiContracts;

public /*open*/ abstract class ApiClient : IApiClient
{
    //შეცდომის შეტყობინებაში პასუხის სხეულის მაქსიმალური სიგრძე
    private const int MaxErrorBodyLengthInMessage = 500;

    private readonly string? _apiKey;
    private readonly HttpClient _client;
    private readonly ILogger? _logger;
    private readonly string _server;

    private readonly bool _useConsole;

    //protected იყენებს SystemTools
    //readonly არ გამოდგება, JwtContractReCounterApiClient სეტავს
    // ReSharper disable once MemberCanBePrivate.Global
#pragma warning disable IDE0044
    protected string? AccessToken;
#pragma warning restore IDE0044

    protected ApiClient(ILogger? logger, IHttpClientFactory httpClientFactory, string server, string? apiKey,
        IMessageHubClient? messageHubClient, bool useConsole, string? accessToken = null)
    {
        _logger = logger;
        _server = server.RemoveNotNeedLastPart('/');
        _apiKey = apiKey;
        MessageHubClient = messageHubClient;
        _useConsole = useConsole;
        AccessToken = accessToken;
        _client = httpClientFactory.CreateClient();
    }

    //protected იყენებს SystemTools
    // ReSharper disable once MemberCanBePrivate.Global
    protected IMessageHubClient? MessageHubClient { get; }

    protected Task<Result> GetAsync(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return GetAsync(afterServerAddress, true, cancellationToken);
    }

    public async ValueTask<bool> RunMessages(CancellationToken cancellationToken = default)
    {
        if (MessageHubClient is null)
        {
            return false;
        }

        return await MessageHubClient.RunMessages(cancellationToken);
    }

    public async ValueTask<bool> StopMessages(CancellationToken cancellationToken = default)
    {
        if (MessageHubClient is null)
        {
            return false;
        }

        return await MessageHubClient.StopMessages(cancellationToken);
    }

    private async Task<Result> GetAsync(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        SetAuthorizationAccessToken();

        Result<string> result = await SendAndReadAsync(HttpMethod.Get, uri, null, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return ToResult(result);
    }

    protected async Task<Result> GetWithTokenAsync(string token, string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return ToResult(await SendAndReadAsync(HttpMethod.Get, uri, null, cancellationToken));
    }

    private void SetAuthorizationAccessToken()
    {
        if (AccessToken is not null && _client.DefaultRequestHeaders.Authorization is null ||
            _client.DefaultRequestHeaders.Authorization?.Parameter is null ||
            _client.DefaultRequestHeaders.Authorization.Parameter != AccessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
    }

    protected async Task<Result<string>> GetAsyncAsString(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        Result<string> result = await SendAndReadAsync(HttpMethod.Get, uri, null, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return result;
    }

    protected async ValueTask<Result> DeleteAsync(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        Result<string> result = await SendAndReadAsync(HttpMethod.Delete, uri, null, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return ToResult(result);
    }

    protected ValueTask<Result> PostAsync(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, true, null, cancellationToken);
    }

    protected ValueTask<Result> PostAsync(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში DatabaseApiClient-ის მიერ
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<Result> PostAsync(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        SetAuthorizationAccessToken();

        Result<string> result = await SendAndReadAsync(HttpMethod.Post, uri, bodyJsonData, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return ToResult(result);
    }

    protected Task<Result> PutAsync(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return PutAsync(afterServerAddress, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<Result> PutAsync(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        Result<string> result = await SendAndReadAsync(HttpMethod.Put, uri, bodyJsonData, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return ToResult(result);
    }

    protected ValueTask<Result<string>> PostAsyncReturnString(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, true, null, cancellationToken);
    }

    protected ValueTask<Result<string>> PostAsyncReturnString(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<Result<string>> PostAsyncReturnString(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        Result<string> result = await SendAndReadAsync(HttpMethod.Post, uri, bodyJsonData, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return result;
    }

    protected Task<Result<T>> PostAsyncReturn<T>(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, true, null, cancellationToken);
    }

    protected Task<Result<T>> PostAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<Result<T>> PostAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        Result<string> result = await SendAndReadAsync(HttpMethod.Post, uri, bodyJsonData, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return Deserialize<T>(result);
    }

    protected async Task<Result<T>> GetAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        SetAuthorizationAccessToken();

        Result<string> result = await SendAndReadAsync(HttpMethod.Get, uri, null, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        return Deserialize<T>(result);
    }

    //ერთადერთი ადგილი, სადაც მოთხოვნა იგზავნება და პასუხი იკითხება.
    //წარმატებისას აბრუნებს პასუხის ტექსტს, შეცდომისას — სერვერის მიერ დაბრუნებულ Error-ს.
    //ქსელური შეცდომა და HttpClient-ის Timeout Result-ის შეცდომად იქცევა, ნამდვილი გაუქმება (OperationCanceledException) კი გადის
    private async Task<Result<string>> SendAndReadAsync(HttpMethod method, Uri uri, string? bodyJsonData,
        CancellationToken cancellationToken)
    {
        // ReSharper disable once using
        using StringContent? content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        using var request = new HttpRequestMessage(method, uri) { Content = content };

        try
        {
            // ReSharper disable once using
            using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            return response.IsSuccessStatusCode
                ? Result.Success(responseBody)
                : Result.Failure<string>(CreateErrorFromResponse(response, responseBody, bodyJsonData));
        }
        catch (Exception e) when (e is HttpRequestException or IOException ||
                                  e is TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            //TaskCanceledException გაუქმების მოთხოვნის გარეშე HttpClient.Timeout-ს ნიშნავს
            return Result.Failure<string>(CreateRequestFailedError(uri, e));
        }
    }

    private static Result ToResult(Result<string> result)
    {
        return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
    }

    //ქსელური შეცდომა ან Timeout — გამონაკლისის ნაცვლად Result-ის შეცდომა. მისამართიდან query (apikey) მოცილებულია
    private Error CreateRequestFailedError(Uri uri, Exception e)
    {
        string address = uri.GetLeftPart(UriPartial.Path);

        if (_useConsole)
        {
            StShared.WriteErrorLine($"request to {address} failed: {e.Message}", true, null, false);
        }

        if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(e, "Request to {Address} failed", address);
        }

        return ApiClientErrors.ApiRequestFailed($"{address}: {e.Message}");
    }

    //არაწარმატებული პასუხიდან Error-ის აწყობა. გამონაკლისს არ ისვრის
    private Error CreateErrorFromResponse(HttpResponseMessage response, string responseBody, string? bodyJsonData)
    {
        string statusLine = $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim();

        if (_useConsole)
        {
            StShared.WriteErrorLine(
                $"answer after uri: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}", true,
                null, false);

            if (!string.IsNullOrWhiteSpace(bodyJsonData))
            {
                StShared.WriteErrorLine($"request body was : {bodyJsonData}", true, null, false);
            }

            StShared.WriteErrorLine($"Error from server: {statusLine}", true, null, false);
        }

        if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError("Api returned error {StatusLine}: {ResponseBody}", statusLine, responseBody);
        }

        Error[] errors = ParseServerErrors(responseBody);

        return errors.Length switch
        {
            0 => ApiClientErrors.ApiReturnedAnError(BuildFallbackMessage(statusLine, responseBody)),
            1 => errors[0],
            //რამდენიმე შეცდომა ერთად — PrintErrorsOnConsole თითოეულს ცალ-ცალკე დაბეჭდავს
            _ => new ValidationError(errors)
        };
    }

    //BadRequest-ის პასუხი მასივია ([{code,description,type}]), გამონაკლისის დამმუშავებელი კი ერთ ობიექტს აბრუნებს — ორივე იკითხება.
    //ცარიელი code-ის ჩანაწერები იგნორირდება, რომ Error.None-ის ტოლი მნიშვნელობა Result-ში არ მოხვდეს (Result-ის კონსტრუქტორი ისვრის)
    private static Error[] ParseServerErrors(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return [];
        }

        try
        {
            JToken token = JToken.Parse(responseBody);
            Error?[] parsed = token switch
            {
                JArray array => array.ToObject<Error?[]>() ?? [],
                JObject obj => [obj.ToObject<Error>()],
                _ => []
            };

            return [.. parsed.OfType<Error>().Where(e => !string.IsNullOrWhiteSpace(e.Code))];
        }
        catch (JsonException)
        {
            //პასუხი JSON არ არის (მაგალითად, HTML პროქსიდან) ან Error-ის ფორმას არ ემთხვევა
            return [];
        }
    }

    private static string BuildFallbackMessage(string statusLine, string responseBody)
    {
        string body = responseBody.Trim();
        if (body.Length == 0)
        {
            return statusLine;
        }

        if (body.Length > MaxErrorBodyLengthInMessage)
        {
            body = body[..MaxErrorBodyLengthInMessage] + "...";
        }

        return $"{statusLine}: {body}";
    }

    //წარმატებული პასუხის JSON-ის T ტიპად გარდაქმნა: არასწორი JSON → ApiReturnedInvalidData, null → ApiDidNotReturnAnything
    private Result<T> Deserialize<T>(Result<string> bodyResult)
    {
        if (bodyResult.IsFailure)
        {
            return Result.Failure<T>(bodyResult.Error);
        }

        try
        {
            var desResult = JsonConvert.DeserializeObject<T>(bodyResult.Value);
            return desResult is null
                ? Result.Failure<T>(ApiClientErrors.ApiDidNotReturnAnything)
                : Result.Success(desResult);
        }
        catch (JsonException e)
        {
            if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(e, "Api response could not be deserialized to {TypeName}", typeof(T).Name);
            }

            return Result.Failure<T>(ApiClientErrors.ApiReturnedInvalidData(e.Message));
        }
    }

    private Uri CreateUri(string afterServerAddress)
    {
        var uri = new Uri($"{_server}{afterServerAddress}");
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            uri = string.IsNullOrEmpty(uri.Query)
                ? new Uri($"{uri}?apikey={_apiKey}")
                : new Uri($"{uri}&apikey={_apiKey}");
        }

        return uri;
    }
}
