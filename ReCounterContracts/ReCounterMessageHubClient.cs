﻿using ApiContracts;
using ApiContracts.V1.Routes;
using Microsoft.AspNetCore.SignalR.Client;

namespace ReCounterContracts;

public sealed class ReCounterMessageHubClient : IMessageHubClient
{
    private readonly string? _apiKey;
    private readonly string _server;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public ReCounterMessageHubClient(string server, string? apiKey)
    {
        _server = server;
        _apiKey = apiKey;
    }

    public async Task RunMessages(CancellationToken cancellationToken)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(
                $"{_server}{MessagesRoutes.Messages.MessagesRoute}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?{ApiKeysConstants.ApiKeyParameterName}={_apiKey}")}")
            .Build();

        //_connection.On<string>(Events.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        _connection.On<ProgressData>(RecounterEvents.ProgressDataReceived, progressData =>
        {
            var lineNo = Console.CursorTop;

            int? procPosition = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcPosition);
            int? procLength = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcLength);
            if (procPosition is not null && procLength is not null)
            {
                var procPercentage = Math.Round((decimal)procPosition / (decimal)procLength * 100);
                Console.WriteLine($"[{_server}]: {procPosition}-{procLength} {procPercentage}%");
            }

            Console.SetCursorPosition(0, lineNo);
        });

        await _connection.StartAsync(cancellationToken);
    }

    public async Task StopMessages(CancellationToken cancellationToken)
    {
        if (_connection is not null)
            await _connection.StopAsync(cancellationToken);
    }
}