using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using SystemTools.ApiContracts.Tests.TestDoubles;
using SystemTools.SharedKernel;

namespace SystemTools.ApiContracts.Tests;

//Building the request: overloads, the message hub, the authorization header and the address
public sealed class ApiClientRequestTests
{
    private const string ApiKey = "test-key";
    private const string ListAddress = "/tasks/list";

    private static readonly string Server =
        new UriBuilder(Uri.UriSchemeHttp, "localhost", 5028, "api/v1").Uri.AbsoluteUri;

    private static TestableApiClient CreateClient(HttpMessageHandler handler, string? apiKey = ApiKey,
        IMessageHubClient? messageHubClient = null, string? accessToken = null, string? server = null)
    {
        return new TestableApiClient(new FakeHttpClientFactory(handler), server ?? Server, apiKey, messageHubClient,
            accessToken: accessToken);
    }

    [Fact]
    public async Task RunMessages_ReturnsFalse_WhenThereIsNoMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        TestableApiClient client = CreateClient(handler);

        Assert.False(await client.RunMessages());
    }

    [Fact]
    public async Task RunMessages_StartsTheMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Assert.True(await client.RunMessages());
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(0, messageHubClient.StopCount);
    }

    [Fact]
    public async Task StopMessages_ReturnsFalse_WhenThereIsNoMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        TestableApiClient client = CreateClient(handler);

        Assert.False(await client.StopMessages());
    }

    [Fact]
    public async Task StopMessages_StopsTheMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Assert.True(await client.StopMessages());
        Assert.Equal(0, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task GetAsync_SendsGetAndRunsTheMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task GetAsyncReturn_RunsAndStopsTheMessageHub_WhenAsked()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<bool> result = await client.GetReturn<bool>("/test/testconnection", true);

        Assert.True(result.Value);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task GetAsyncAsString_RunsAndStopsTheMessageHub()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, "1.0.0", MediaTypeNames.Text.Plain);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<string> result = await client.GetString("/test/getversion");

        Assert.Equal("1.0.0", result.Value);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task GetAsyncAsString_ReturnsTheServerError_OnFailure()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.NotFound,
            """[{"code":"VersionNotFound","description":"no version","type":3}]""");
        TestableApiClient client = CreateClient(handler);

        Result<string> result = await client.GetString("/test/getversion");

        Assert.True(result.IsFailure);
        Assert.Equal("VersionNotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeleteAsync_SendsDeleteAndRunsTheMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result result = await client.Delete("/tasks/delete/x");

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsync_WithDefaults_PostsWithoutBodyAndRunsTheMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result result = await client.PostWithDefaults("/tasks/run");

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsync_WithoutBody_DoesNotRunTheMessageHub_WhenNotAsked()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result result = await client.PostWithoutBody("/tasks/run", false);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(0, messageHubClient.RunCount);
        Assert.Equal(0, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsync_ReturnsTheServerError_OnFailure()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"TaskNameRequired","description":"task name is required","type":2}]""");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Post("/tasks/create", false, "{}");

        Assert.True(result.IsFailure);
        Assert.Equal("TaskNameRequired", result.Error.Code);
    }

    [Fact]
    public async Task PutAsync_SendsPutWithTheBodyAndRunsTheMessageHub()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result result = await client.Put("/tasks/update", """{"TaskName":"x"}""");

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Put, handler.LastRequestMethod);
        Assert.Equal("""{"TaskName":"x"}""", handler.LastRequestBody);
        Assert.Equal(MediaTypeNames.Application.Json, handler.LastRequestContentType);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PutAsync_WithoutBody_SendsNoContent()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.PutWithoutBody("/tasks/update");

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Put, handler.LastRequestMethod);
        Assert.Null(handler.LastRequestBody);
    }

    [Fact]
    public async Task PutAsync_ReturnsTheServerError_OnFailure()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.Conflict,
            """[{"code":"TaskIsRunning","description":"task is running","type":4}]""");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Put("/tasks/update", "{}");

        Assert.True(result.IsFailure);
        Assert.Equal("TaskIsRunning", result.Error.Code);
    }

    [Fact]
    public async Task PostAsyncReturnString_ReturnsTheBodyWithoutTheMessageHub_WhenNotAsked()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, "created", MediaTypeNames.Text.Plain);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<string> result = await client.PostReturnString("/tasks/create", false, """{"TaskName":"x"}""");

        Assert.Equal("created", result.Value);
        Assert.Equal("""{"TaskName":"x"}""", handler.LastRequestBody);
        Assert.Equal(0, messageHubClient.RunCount);
        Assert.Equal(0, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsyncReturnString_WithDefaults_PostsWithoutBodyAndRunsTheMessageHub()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, "done", MediaTypeNames.Text.Plain);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<string> result = await client.PostReturnStringWithDefaults("/tasks/run");

        Assert.Equal("done", result.Value);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsyncReturnString_WithoutBody_RunsTheMessageHub_WhenAsked()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, "done", MediaTypeNames.Text.Plain);
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<string> result = await client.PostReturnStringWithoutBody("/tasks/run", true);

        Assert.Equal("done", result.Value);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsyncReturnString_ReturnsTheServerError_OnFailure()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"TaskNameRequired","description":"task name is required","type":2}]""");
        TestableApiClient client = CreateClient(handler);

        Result<string> result = await client.PostReturnString("/tasks/create", false, "{}");

        Assert.True(result.IsFailure);
        Assert.Equal("TaskNameRequired", result.Error.Code);
    }

    [Fact]
    public async Task PostAsyncReturn_WithDefaults_DeserializesAndRunsTheMessageHub()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.OK, """{"name":"abc","count":3}""");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<SampleDto> result = await client.PostReturnWithDefaults<SampleDto>("/tasks/start");

        Assert.Equal("abc", result.Value.Name);
        Assert.Equal(3, result.Value.Count);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsyncReturn_WithoutBody_DeserializesWithoutTheMessageHub_WhenNotAsked()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<bool> result = await client.PostReturnWithoutBody<bool>("/tasks/cancel", false);

        Assert.True(result.Value);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Null(handler.LastRequestBody);
        Assert.Equal(0, messageHubClient.RunCount);
        Assert.Equal(0, messageHubClient.StopCount);
    }

    [Fact]
    public async Task PostAsyncReturn_RunsTheMessageHub_WhenAsked()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        var messageHubClient = new FakeMessageHubClient();
        TestableApiClient client = CreateClient(handler, messageHubClient: messageHubClient);

        Result<bool> result = await client.PostReturn<bool>("/tasks/cancel", true, """{"TaskName":"x"}""");

        Assert.True(result.Value);
        Assert.Equal("""{"TaskName":"x"}""", handler.LastRequestBody);
        Assert.Equal(1, messageHubClient.RunCount);
        Assert.Equal(1, messageHubClient.StopCount);
    }

    [Fact]
    public async Task GetWithTokenAsync_SendsTheTokenAsBearer()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.GetWithToken("token-1", ListAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.NotNull(handler.LastRequestAuthorization);
        Assert.Equal("Bearer", handler.LastRequestAuthorization.Scheme);
        Assert.Equal("token-1", handler.LastRequestAuthorization.Parameter);
    }

    [Fact]
    public async Task GetWithTokenAsync_ReturnsTheServerError_OnFailure()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.Unauthorized, null);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.GetWithToken("token-1", ListAddress);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task PostAsync_SendsTheAccessTokenAsBearer()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, accessToken: "access-1");

        await client.Post("/tasks/create", false, "{}");

        Assert.NotNull(handler.LastRequestAuthorization);
        Assert.Equal("Bearer", handler.LastRequestAuthorization.Scheme);
        Assert.Equal("access-1", handler.LastRequestAuthorization.Parameter);
    }

    [Fact]
    public async Task GetAsyncReturn_ReplacesAnotherTokenWithTheAccessToken()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "true");
        TestableApiClient client = CreateClient(handler, accessToken: "access-1");

        await client.GetWithToken("token-1", ListAddress);
        await client.GetReturn<bool>("/test/testconnection", false);

        Assert.NotNull(handler.LastRequestAuthorization);
        Assert.Equal("access-1", handler.LastRequestAuthorization.Parameter);
    }

    [Fact]
    public async Task GetAsync_SendsNoAuthorization_WhenThereIsNoAccessToken()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler);

        await client.Get(ListAddress);

        Assert.Null(handler.LastRequestAuthorization);
    }

    [Fact]
    public async Task PostAsync_SendsNoAuthorization_WhenThereIsNoAccessToken()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler);

        await client.Post("/tasks/create", false, "{}");

        Assert.Null(handler.LastRequestAuthorization);
    }

    [Fact]
    public async Task GetAsync_KeepsTheTokenOfGetWithTokenAsync_WhenThereIsNoAccessToken()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler);

        await client.GetWithToken("token-1", ListAddress);
        await client.Get(ListAddress);

        Assert.NotNull(handler.LastRequestAuthorization);
        Assert.Equal("Bearer", handler.LastRequestAuthorization.Scheme);
        Assert.Equal("token-1", handler.LastRequestAuthorization.Parameter);
    }

    [Fact]
    public async Task GetAsync_SendsTheAccessToken_WhenTheHeaderAlreadyHasIt()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, accessToken: "access-1");

        await client.Get(ListAddress);
        await client.Get(ListAddress);

        Assert.NotNull(handler.LastRequestAuthorization);
        Assert.Equal("Bearer", handler.LastRequestAuthorization.Scheme);
        Assert.Equal("access-1", handler.LastRequestAuthorization.Parameter);
    }

    [Fact]
    public async Task CreateUri_AppendsTheApiKeyAfterAnExistingQuery()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler);

        await client.Get("/items/getbyname?name=abc");

        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("?name=abc&apikey=test-key", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task CreateUri_EscapesTheApiKey()
    {
        const string apiKey = "a+b&c=d/e f#g%h";
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, apiKey);

        await client.Get(ListAddress);

        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("?apikey=a%2Bb%26c%3Dd%2Fe%20f%23g%25h", handler.LastRequestUri.Query);
        Assert.Equal(apiKey, Uri.UnescapeDataString(handler.LastRequestUri.Query["?apikey=".Length..]));
    }

    [Fact]
    public async Task CreateUri_EscapesTheApiKeyAfterAnExistingQuery()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, "k&name=x");

        await client.Get("/items/getbyname?name=abc");

        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("?name=abc&apikey=k%26name%3Dx", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task CreateUri_AddsNoQuery_WithoutApiKey()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, null);

        await client.Get(ListAddress);

        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
        Assert.Equal("/api/v1/tasks/list", handler.LastRequestUri.AbsolutePath);
    }

    [Fact]
    public async Task CreateUri_IgnoresAWhitespaceApiKey()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, "  ");

        await client.Get("/items/getbyname?name=abc");

        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("?name=abc", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task Constructor_RemovesTheTrailingSlashOfTheServer()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, null);
        TestableApiClient client = CreateClient(handler, server: Server + "/");

        await client.Get(ListAddress);

        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal("/api/v1/tasks/list", handler.LastRequestUri.AbsolutePath);
    }
}
