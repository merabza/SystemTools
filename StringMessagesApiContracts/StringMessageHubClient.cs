using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiContracts;
using Microsoft.AspNetCore.SignalR.Client;
using StringMessagesApiContracts.V1.Routes;
using SystemToolsShared;

namespace StringMessagesApiContracts;

public sealed class StringMessageHubClient : IMessageHubClient
{
    private readonly string? _apiKey;
    private readonly string _server;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public StringMessageHubClient(string server, string? apiKey)
    {
        _server = server.RemoveNotNeedLastPart('/');
        _apiKey = apiKey;
    }

    public async Task<bool> RunMessages(CancellationToken cancellationToken = default)
    {
        var url =
            $"{_server}{MessagesRoutes.Messages.MessagesRoute}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?{ApiKeysConstants.ApiKeyParameterName}={_apiKey}")}";
        _connection = new HubConnectionBuilder().WithUrl(url).Build();

        _connection.On<string>(StringEvents.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        try
        {
            await _connection.StartAsync(cancellationToken);
            return true;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error when connecting {ex.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    public async ValueTask<bool> StopMessages(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_connection is null)
            {
                return true;
            }

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
