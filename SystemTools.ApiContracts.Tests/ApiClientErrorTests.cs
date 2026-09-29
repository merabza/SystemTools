using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using SystemTools.ApiContracts.Errors;
using SystemTools.ApiContracts.Tests.TestDoubles;
using SystemTools.SharedKernel;

namespace SystemTools.ApiContracts.Tests;

//Reading the error response: the fallback text, the ProblemDetails edge cases and network errors
public sealed class ApiClientErrorTests
{
    private const string ListAddress = "/tasks/list";

    private static readonly string Server =
        new UriBuilder(Uri.UriSchemeHttp, "localhost", 5028, "api/v1").Uri.AbsoluteUri;

    private static TestableApiClient CreateClient(HttpMessageHandler handler)
    {
        return new TestableApiClient(new FakeHttpClientFactory(handler), Server, "test-key");
    }

    [Fact]
    public async Task GetAsync_ReturnsApiRequestFailed_WhenHandlerThrowsIOException()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Throw(new IOException("connection reset"));
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.True(result.IsFailure);
        Assert.Equal(nameof(ApiClientErrors.ApiRequestFailed), result.Error.Code);
        Assert.Equal("Api request failed: http://localhost:5028/api/v1/tasks/list: connection reset",
            result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_PutsTheStatusAndTheBodyIntoTheFallbackError()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadGateway,
            " <html>down</html> ", MediaTypeNames.Text.Html);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal("Api Returned an Error: 502 Bad Gateway: <html>down</html>", result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_TruncatesALongBodyInTheFallbackError()
    {
        string body = new('a', 600);
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.InternalServerError, body, MediaTypeNames.Text.Plain);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal($"Api Returned an Error: 500 Internal Server Error: {new string('a', 500)}...",
            result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_KeepsABodyOfExactlyTheMaximumLength()
    {
        string body = new('b', 500);
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.InternalServerError, body, MediaTypeNames.Text.Plain);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal($"Api Returned an Error: 500 Internal Server Error: {body}", result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_PutsOnlyTheStatusIntoTheFallbackError_WhenTheBodyIsWhitespace()
    {
        using StubHttpMessageHandler handler =
            StubHttpMessageHandler.Respond(HttpStatusCode.ServiceUnavailable, "   ", MediaTypeNames.Text.Plain);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal("Api Returned an Error: 503 Service Unavailable", result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_TrimsTheStatusLine_WhenThereIsNoReasonPhrase()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond((HttpStatusCode)499, null);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal("Api Returned an Error: 499", result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_ReturnsTheFallbackError_WhenTheBodyIsAJsonString()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest, "\"oops\"");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_PrefersTheCodeToTheTitle()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """{"code":"TaskFailed","description":"task failed","type":2,"title":"Other"}""");
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal("TaskFailed", result.Error.Code);
        Assert.Equal("task failed", result.Error.Description);
    }

    [Fact]
    public async Task GetAsync_ReturnsTheFallbackError_WhenTheProblemDetailsTitleIsEmpty()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """{"title":"","status":400}""", MediaTypeNames.Application.ProblemJson);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_ReturnsTheFallbackError_WhenTheProblemDetailsErrorsIsAnObject()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """{"title":"One or more validation errors occurred.","status":400,"errors":{"Name":["The Name field is required."]}}""",
            MediaTypeNames.Application.ProblemJson);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.Equal(nameof(ApiClientErrors.ApiReturnedAnError), result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_ReturnsTheOnlyErrorOfTheProblemDetailsList()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """{"title":"Validation.General","status":400,"detail":"One or more validation errors occurred","errors":[{"code":"NameRequired","description":"name is required","type":2}]}""",
            MediaTypeNames.Application.ProblemJson);
        TestableApiClient client = CreateClient(handler);

        Result result = await client.Get(ListAddress);

        Assert.IsNotType<ValidationError>(result.Error);
        Assert.Equal("NameRequired", result.Error.Code);
        Assert.Equal(ErrorType.Problem, result.Error.Type);
    }
}
