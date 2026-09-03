using System;
using System.Net.Http;

namespace SystemTools.ApiContracts.Tests.TestDoubles;

internal sealed class FakeHttpClientFactory : IHttpClientFactory
{
    private readonly HttpMessageHandler _handler;
    private readonly TimeSpan? _timeout;

    public FakeHttpClientFactory(HttpMessageHandler handler, TimeSpan? timeout = null)
    {
        _handler = handler;
        _timeout = timeout;
    }

    public HttpClient CreateClient(string name)
    {
        var client = new HttpClient(_handler, false);
        if (_timeout is not null)
        {
            client.Timeout = _timeout.Value;
        }

        return client;
    }
}
