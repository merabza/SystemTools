using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SystemTools.ApiContracts.Tests.TestDoubles;

namespace SystemTools.ApiContracts.Tests;

[CollectionDefinition(nameof(ConsoleOutputCollection), DisableParallelization = true)]
public sealed class ConsoleOutputCollection;

//Printing errors to the console and logging them. Replaces Console.Out, so it never runs in parallel with other tests
[Collection(nameof(ConsoleOutputCollection))]
public sealed class ApiClientDiagnosticsTests
{
    private const string ApiKey = "test-key";

    private static readonly string Server =
        new UriBuilder(Uri.UriSchemeHttp, "localhost", 5028, "api/v1").Uri.AbsoluteUri;

    private static TestableApiClient CreateClient(HttpMessageHandler handler, bool useConsole,
        ILogger? logger = null, string? apiKey = ApiKey)
    {
        return new TestableApiClient(new FakeHttpClientFactory(handler), Server, apiKey, null, logger, useConsole);
    }

    private static async Task<string> CaptureConsole(Func<Task> action)
    {
        TextWriter original = Console.Out;
        await using var writer = new StringWriter(CultureInfo.InvariantCulture);
        Console.SetOut(writer);
        try
        {
            await action();
        }
        finally
        {
            Console.SetOut(original);
        }

        return writer.ToString();
    }

    private static Mock<ILogger> CreateLogger(bool errorEnabled)
    {
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(LogLevel.Error)).Returns(errorEnabled);
        return logger;
    }

    private static void AssertNothingLogged(Mock<ILogger> logger)
    {
        Assert.DoesNotContain(logger.Invocations, invocation => invocation.Method.Name == nameof(ILogger.Log));
    }

    private static void VerifyErrorLogged(Mock<ILogger> logger, string message, Func<Exception?, bool> exception)
    {
        logger.Verify(
            l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, _) => v.ToString() == message),
                It.Is<Exception?>(e => exception(e)), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ErrorResponse_WritesTheRequestTheBodyAndTheStatus_WhenUsingTheConsole()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"TaskNameRequired","description":"task name is required","type":2}]""");
        TestableApiClient client = CreateClient(handler, true);

        string output = await CaptureConsole(() => client.Post("/tasks/create", false, """{"TaskName":""}""").AsTask());

        Assert.Contains("[ERROR] answer after uri: POST http://localhost:5028/api/v1/tasks/create?apikey=***",
            output.Split(Environment.NewLine));
        Assert.Contains("""[ERROR] request body was : {"TaskName":""}""", output, StringComparison.Ordinal);
        Assert.Contains("[ERROR] Error from server: 400 Bad Request", output, StringComparison.Ordinal);
        Assert.DoesNotContain(ApiKey, output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ErrorResponse_HidesOnlyTheApiKeyValueOfTheAddress()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.NotFound, null);
        TestableApiClient client = CreateClient(handler, true, apiKey: "secret&key");

        string output = await CaptureConsole(() => client.Get("/items/getbyname?name=abc&ApiKey=other-secret"));

        Assert.Contains(
            "[ERROR] answer after uri: GET http://localhost:5028/api/v1/items/getbyname?name=abc&apikey=***&apikey=***",
            output.Split(Environment.NewLine));
        Assert.DoesNotContain("secret", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ErrorResponse_WritesTheAddressUnchanged_WhenThereIsNoQuery()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.NotFound, null);
        TestableApiClient client = CreateClient(handler, true, apiKey: null);

        string output = await CaptureConsole(() => client.Get("/tasks/list"));

        Assert.Contains("[ERROR] answer after uri: GET http://localhost:5028/api/v1/tasks/list",
            output.Split(Environment.NewLine));
    }

    [Fact]
    public async Task ErrorResponse_DoesNotWriteABody_WhenTheRequestHadNone()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.NotFound, null);
        TestableApiClient client = CreateClient(handler, true);

        string output = await CaptureConsole(() => client.Get("/tasks/list"));

        Assert.Contains("[ERROR] answer after uri: GET", output, StringComparison.Ordinal);
        Assert.DoesNotContain("request body was", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ErrorResponse_WritesNothing_WhenNotUsingTheConsole()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.BadRequest,
            """[{"code":"TaskNameRequired","description":"task name is required","type":2}]""");
        TestableApiClient client = CreateClient(handler, false);

        string output = await CaptureConsole(() => client.Post("/tasks/create", false, "{}").AsTask());

        Assert.Equal(string.Empty, output);
    }

    [Fact]
    public async Task FailedRequest_WritesTheAddressAndTheReason_WhenUsingTheConsole()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Throw(new HttpRequestException("refused"));
        TestableApiClient client = CreateClient(handler, true);

        string output = await CaptureConsole(() => client.Get("/tasks/list"));

        Assert.Contains("[ERROR] request to http://localhost:5028/api/v1/tasks/list failed: refused", output,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task FailedRequest_WritesNothing_WhenNotUsingTheConsole()
    {
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Throw(new HttpRequestException("refused"));
        TestableApiClient client = CreateClient(handler, false);

        string output = await CaptureConsole(() => client.Get("/tasks/list"));

        Assert.Equal(string.Empty, output);
    }

    [Fact]
    public async Task ErrorResponse_LogsTheStatusAndTheBody()
    {
        Mock<ILogger> logger = CreateLogger(true);
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.Conflict, "busy");
        TestableApiClient client = CreateClient(handler, false, logger.Object);

        await client.Get("/tasks/list");

        VerifyErrorLogged(logger, "Api returned error 409 Conflict: busy", e => e is null);
    }

    [Fact]
    public async Task ErrorResponse_LogsNothing_WhenTheErrorLevelIsDisabled()
    {
        Mock<ILogger> logger = CreateLogger(false);
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.Conflict, "busy");
        TestableApiClient client = CreateClient(handler, false, logger.Object);

        await client.Get("/tasks/list");

        AssertNothingLogged(logger);
    }

    [Fact]
    public async Task FailedRequest_LogsTheAddressAndTheException()
    {
        Mock<ILogger> logger = CreateLogger(true);
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Throw(new HttpRequestException("refused"));
        TestableApiClient client = CreateClient(handler, false, logger.Object);

        await client.Get("/tasks/list");

        VerifyErrorLogged(logger, "Request to http://localhost:5028/api/v1/tasks/list failed",
            e => e is HttpRequestException { Message: "refused" });
    }

    [Fact]
    public async Task FailedRequest_LogsNothing_WhenTheErrorLevelIsDisabled()
    {
        Mock<ILogger> logger = CreateLogger(false);
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Throw(new HttpRequestException("refused"));
        TestableApiClient client = CreateClient(handler, false, logger.Object);

        await client.Get("/tasks/list");

        AssertNothingLogged(logger);
    }

    [Fact]
    public async Task InvalidJson_LogsTheTypeAndTheJsonException()
    {
        Mock<ILogger> logger = CreateLogger(true);
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "<html>x</html>");
        TestableApiClient client = CreateClient(handler, false, logger.Object);

        await client.GetReturn<SampleDto>("/tasks/list", false);

        VerifyErrorLogged(logger, "Api response could not be deserialized to SampleDto", e => e is JsonException);
    }

    [Fact]
    public async Task InvalidJson_LogsNothing_WhenTheErrorLevelIsDisabled()
    {
        Mock<ILogger> logger = CreateLogger(false);
        using StubHttpMessageHandler handler = StubHttpMessageHandler.Respond(HttpStatusCode.OK, "<html>x</html>");
        TestableApiClient client = CreateClient(handler, false, logger.Object);

        await client.GetReturn<SampleDto>("/tasks/list", false);

        AssertNothingLogged(logger);
    }
}
