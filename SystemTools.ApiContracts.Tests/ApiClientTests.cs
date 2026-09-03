using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using SystemTools.ApiContracts.Errors;
using SystemTools.ApiContracts.Tests.TestDoubles;
using SystemTools.SharedKernel;

namespace SystemTools.ApiContracts.Tests;

public sealed class ApiClientTests
{
    private const string ApiKey = "test-key";
    private const string ListAddress = "/tasks/list";

    private static readonly string Server =
        new UriBuilder(Uri.UriSchemeHttp, "localhost", 5028, "api/v1").Uri.AbsoluteUri;

    private static TestableApiClient CreateClient(HttpMessageHandler handler, string? apiKey = ApiKey,
        IMessageHubClient? messageHubClient = null, TimeSpan? timeout = null)
    {
        return new TestableApiClient(new FakeHttpClientFactory(handler, timeout), Server, apiKey, messageHubClient);
    }

    [Fact]
    public async Task GetAsyncReturn_ReturnsValueAndSendsApiKey_When200Json()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, """{"name":"abc","count":3}""");
        TestableApiClient client = CreateClient(handler);

        Result<SampleDto> result = await client.GetReturn<SampleDto>("/items/getbyname?name=abc", false);

        Assert.True(result.IsSuccess);
        Assert.Equal("abc", result.Value.Name);
        Assert.Equal(3, result.Value.Count);
        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("/api/v1/items/getbyname", handler.LastRequestUri.AbsolutePath);
        Assert.Contains("apikey=test-key", handler.LastRequestUri.Query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsyncReturn_ReturnsNullValue_WhenEnvelopeValueIsNull()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, """{"value":null}""");
        TestableApiClient client = CreateClient(handler);

        Result<NullableEnvelope<SampleDto>> result =
            await client.GetReturn<NullableEnvelope<SampleDto>>("/items/getbyname?name=x", false);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Value);
    }

    [Fact]
    public async Task GetAsyncReturn_ReturnsServerError_When400WithSingleErrorArray()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"TaskWithNameNotFound","description":"Task with name x not found","type":3}]""");
        TestableApiClient client = CreateClient(handler);

        Result<SampleDto> result = await client.GetReturn<SampleDto>("/tasks/getbyname?name=x", false);

        Assert.True(result.IsFailure);
        Assert.Equal("TaskWithNameNotFound", result.Error.Code);
        Assert.Equal("Task with name x not found", result.Error.Description);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task PostAsync_ReturnsValidationError_When400WithTwoErrors()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"First","description":"first error","type":2},{"code":"Second","description":"second error","type":3}]""");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Post("/tasks/create", false, """{"TaskName":"x"}""");

        Assert.True(result.IsFailure);
        ValidationError validationError = Assert.IsType<ValidationError>(result.Error);
        Assert.Equal(2, validationError.Errors.Length);
        Assert.Equal("First", validationError.Errors[0].Code);
        Assert.Equal("second error", validationError.Errors[1].Description);
    }

    [Fact]
    public async Task PostAsyncReturn_ReturnsThatError_When500WithSingleObjectAsTextPlain()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.InternalServerError,
            """{"code":"UnexpectedApiException","description":"unexpected: 0f8fad5b","type":0}""",
            MediaTypeNames.Text.Plain);
        TestableApiClient client = CreateClient(handler);

        Result<bool> result = await client.PostReturn<bool>("/recounter/cancelcurrentprocess", false, null);

        Assert.True(result.IsFailure);
        Assert.Equal("UnexpectedApiException", result.Error.Code);
        Assert.Equal("unexpected: 0f8fad5b", result.Error.Description);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    [Fact]
    public async Task GetAsync_ReturnsErrorWithStatus_When401WithEmptyBody()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.Unauthorized, null);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
        Assert.Contains("401", result.Error.Description, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFallbackError_WhenErrorBodyIsHtml()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadGateway,
            "<html><body>Bad Gateway</body></html>", MediaTypeNames.Text.Html);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Delete("/tasks/delete?name=x");

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
        Assert.Contains("502", result.Error.Description, StringComparison.Ordinal);
        Assert.Contains("Bad Gateway", result.Error.Description, StringComparison.Ordinal);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
    }

    [Fact]
    public async Task GetAsync_ReturnsFallbackError_When400BodyIsNotAnErrorObject()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """{"title":"Bad Request","status":400}""");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
        Assert.Contains("400", result.Error.Description, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_IgnoresEntriesWithEmptyCode_When400()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"","description":"","type":0}]""");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
    }

    [Fact]
    public async Task GetAsyncReturn_ReturnsApiReturnedInvalidData_When200BodyIsNotJson()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, "<html>not json</html>", MediaTypeNames.Text.Html);
        TestableApiClient client = CreateClient(handler);

        Result<SampleDto> result = await client.GetReturn<SampleDto>(ListAddress, false);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiReturnedInvalidData), result.Error.Code);
    }

    [Fact]
    public async Task GetAsyncReturn_ReturnsApiDidNotReturnAnything_When200BodyIsEmptyForReferenceType()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, string.Empty);
        TestableApiClient client = CreateClient(handler);

        Result<SampleDto> result = await client.GetReturn<SampleDto>(ListAddress, false);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiDidNotReturnAnything), result.Error.Code);
    }

    [Fact]
    public async Task GetAsyncReturn_ReturnsApiReturnedInvalidData_When200BodyIsEmptyForBool()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, string.Empty);
        TestableApiClient client = CreateClient(handler);

        Result<bool> result = await client.GetReturn<bool>("/test/testconnection", false);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiReturnedInvalidData), result.Error.Code);
    }

    [Fact]
    public async Task GetAsyncAsString_ReturnsBody_When200TextPlain()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, "1.0.2607.1919", MediaTypeNames.Text.Plain);
        TestableApiClient client = CreateClient(handler);

        Result<string> result = await client.GetString("/test/getversion");

        Assert.True(result.IsSuccess);
        Assert.Equal("1.0.2607.1919", result.Value);
    }

    [Fact]
    public async Task PostAsyncReturn_ReturnsFalse_When200False()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "false");
        TestableApiClient client = CreateClient(handler);

        Result<bool> result = await client.PostReturn<bool>("/recounter/cancelcurrentprocess", false, null);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task PostAsync_SendsJsonBodyAndApiKey()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Post("/tasks/create", false, """{"TaskName":"x"}""");

        Assert.True(result.IsSuccess);
        Assert.Equal("""{"TaskName":"x"}""", handler.LastRequestBody);
        Assert.Equal(MediaTypeNames.Application.Json, handler.LastRequestContentType);
        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("?apikey=test-key", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetAsync_ReturnsFailure_WhenHandlerThrowsHttpRequestException()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Throw(new HttpRequestException("No connection could be made"));
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiRequestFailed), result.Error.Code);
        Assert.Contains("No connection could be made", result.Error.Description, StringComparison.Ordinal);
        Assert.DoesNotContain("apikey", result.Error.Description, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_ReturnsFailure_WhenHttpClientTimesOut()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Delay(TimeSpan.FromSeconds(30));
        TestableApiClient client = CreateClient(handler, timeout: TimeSpan.FromMilliseconds(100));

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiRequestFailed), result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_PropagatesOperationCanceled_WhenTokenIsCancelled()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        TestableApiClient client = CreateClient(handler);
        var cancelledToken = new CancellationToken(true);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.Get(ListAddress, cancelledToken));
    }

    [Fact]
    public async Task PostAsync_StopsMessageHub_WhenSendFails()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Throw(new HttpRequestException("refused"));
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result result = await client.Post("/crawler/runtask", true, """{"TaskName":"x"}""");

        Assert.True(result.IsFailure);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task GetAsyncReturn_DoesNotUseMessageHub_WhenUseMessageHubClientIsFalse()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<bool> result = await client.GetReturn<bool>("/test/testconnection", false);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, messageHubClient.RunCount);
        Assert.Equal(0, messageHubClient.StopCount);
    }
}
