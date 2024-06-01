using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRContracts;
using SignalRContracts.Models;
using SignalRContracts.V1.Routes;

namespace ApiToolsShared;

public sealed class StingMessageHubClient : IMessageHubClient
{
    private readonly string? _apiKey;
    private readonly string _server;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public StingMessageHubClient(string server, string? apiKey)
    {
        _server = server;
        _apiKey = apiKey;
    }

    public async Task RunMessages(CancellationToken cancellationToken)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(
                $"{_server}{MessagesRoutes.Messages.MessagesRoute}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?{ApiKeysChecker.ApiKeyParameterName}={_apiKey}")}")
            .Build();

        _connection.On<string>(Events.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        //_connection.On<ProgressData>(Events.ProgressDataReceived, progressData =>
        //{
        //    var lineNo = Console.CursorTop;

        //    int? procPosition = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcPosition);
        //    int? procLength = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcLength);
        //    if (procPosition is not null && procLength is not null)
        //    {
        //        var procPercentage = Math.Round((decimal)procPosition / (decimal)procLength * 100);
        //        Console.WriteLine($"[{_server}]: {procPosition}-{procLength} {procPercentage}%");
        //    }
        //    Console.SetCursorPosition(0, lineNo);
        //});

        await _connection.StartAsync(cancellationToken);
    }

    public async Task StopMessages(CancellationToken cancellationToken)
    {
        if (_connection is not null)
            await _connection.StopAsync(cancellationToken);
    }
}