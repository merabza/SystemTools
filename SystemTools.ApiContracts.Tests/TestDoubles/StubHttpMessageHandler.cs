using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTools.ApiContracts.Tests.TestDoubles;

//ტესტებისთვის: იმახსოვრებს ბოლო მოთხოვნას და წინასწარ განსაზღვრულ პასუხს აბრუნებს
internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;

    private StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
    {
        _responder = responder;
    }

    public Uri? LastRequestUri { get; private set; }
    public HttpMethod? LastRequestMethod { get; private set; }
    public string? LastRequestBody { get; private set; }
    public string? LastRequestContentType { get; private set; }

    public static StubHttpMessageHandler Respond(HttpStatusCode statusCode, string? body,
        string mediaType = MediaTypeNames.Application.Json)
    {
        return new StubHttpMessageHandler((_, _) => Task.FromResult(CreateResponse(statusCode, body, mediaType)));
    }

    public static StubHttpMessageHandler Throw(Exception exception)
    {
        return new StubHttpMessageHandler((_, _) => Task.FromException<HttpResponseMessage>(exception));
    }

    public static StubHttpMessageHandler Delay(TimeSpan delay)
    {
        return new StubHttpMessageHandler(async (_, cancellationToken) =>
        {
            await Task.Delay(delay, cancellationToken);
            return CreateResponse(HttpStatusCode.OK, "true", MediaTypeNames.Application.Json);
        });
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        LastRequestUri = request.RequestUri;
        LastRequestMethod = request.Method;
        LastRequestContentType = request.Content?.Headers.ContentType?.MediaType;
        LastRequestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

        HttpResponseMessage response = await _responder(request, cancellationToken);
        response.RequestMessage = request;
        return response;
    }

    private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string? body, string mediaType)
    {
        var response = new HttpResponseMessage(statusCode);
        if (body is not null)
        {
            response.Content = new StringContent(body, Encoding.UTF8, mediaType);
        }

        return response;
    }
}
