using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRContracts.V1.Routes;

namespace SignalRContracts;

public sealed class MessageHubClient
{
    private readonly string? _apiKey;
    private readonly string _server;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public MessageHubClient(string server, string? apiKey)
    {
        _server = server;
        _apiKey = apiKey;
    }

    public async Task RunMessages(CancellationToken cancellationToken)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(
                $"{_server}{MessagesRoutes.Messages.MessagesRoute}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}")
            .Build();

        _connection.On<string>(Events.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        await _connection.StartAsync(cancellationToken);
    }

    public async Task StopMessages(CancellationToken cancellationToken)
    {
        if (_connection is not null)
            await _connection.StopAsync(cancellationToken);
    }
}