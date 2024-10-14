using ApiContracts.V1.Routes;
using Microsoft.AspNetCore.SignalR.Client;

namespace ApiContracts;

public sealed class StringMessageHubClient : IMessageHubClient
{
    private readonly string? _apiKey;
    private readonly string _server;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public StringMessageHubClient(string server, string? apiKey)
    {
        _server = server;
        _apiKey = apiKey;
    }

    public async Task<bool> RunMessages(CancellationToken cancellationToken)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(
                $"{_server}{MessagesRoutes.Messages.MessagesRoute}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?{ApiKeysConstants.ApiKeyParameterName}={_apiKey}")}")
            .Build();

        _connection.On<string>(StringEvents.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        try
        {
            await _connection.StartAsync(cancellationToken);
            return true;
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when connecting");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    public async Task<bool> StopMessages(CancellationToken cancellationToken)
    {
        try
        {
            if (_connection is not null)
                await _connection.StopAsync(cancellationToken);
            return true;
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when Stop connection");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }
}