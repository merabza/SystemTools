using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using ReCounterContracts;
using ReCounterContracts.V1.Routes;
using SystemTools.ApiContracts;

namespace SystemTools.ReCounterContracts;

public sealed class ReCounterMessageHubClient : IMessageHubClient
{
    private readonly string? _apiKey;
    private readonly string _server;
    private string? _accessToken;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public ReCounterMessageHubClient(string server, string? apiKey)
    {
        _server = server;
        _apiKey = apiKey;
    }

    public async Task<bool> RunMessages(CancellationToken cancellationToken = default)
    {
        _connection = new HubConnectionBuilder().WithUrl(
            $"{_server}{RecountMessagesRoutes.ReCounterRoute.Recounter}{RecountMessagesRoutes.ReCounterRoute.Messages}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?{ApiKeysConstants.ApiKeyParameterName}={_apiKey}")}",
            options =>
            {
                options.Transports = HttpTransportType.LongPolling;
                options.AccessTokenProvider = () => Task.FromResult(_accessToken);
            }).Build();

        //_connection.On<string>(Events.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        _connection.On<ProgressData>(RecounterEvents.ProgressDataReceived, progressData =>
        {
            if (progressData.BoolData.Count > 0 &&
                progressData.BoolData.TryGetValue(ReCounterConstants.ProcessRun, out bool processIsRunning))
            {
                ProcessMonitoringManager.Instance.ProcessIsRunning = processIsRunning;
            }

            if (progressData.StrData.Count > 0)
            {
                string? procName = progressData.StrData.GetValueOrDefault(ReCounterConstants.ProcName);
                if (ProcessMonitoringManager.Instance.LastProcName != procName)
                {
                    Console.WriteLine(procName);
                    ProcessMonitoringManager.Instance.LastProcName = procName;
                }
            }

            int? procPosition = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcPosition);
            int? procLength = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcLength);
            string? progressValueName = progressData.StrData.GetValueOrDefault(ReCounterConstants.ProcProgressMessage);

            int lineNo = Console.CursorTop;
            if (procPosition is not null && procLength is not null && (decimal)procLength > 0)
            {
                decimal procPercentage = Math.Round((decimal)procPosition / (decimal)procLength * 100);
                string conMessage =
                    $"[{_server}]: {progressValueName ?? ""} {procPosition}-{procLength} {procPercentage}%";
                int conMessageLength = conMessage.Length;
                if (ProcessMonitoringManager.Instance.LastLength > conMessageLength)
                {
                    conMessage = conMessage.PadRight(ProcessMonitoringManager.Instance.LastLength);
                }

                ProcessMonitoringManager.Instance.LastLength = conMessageLength;
                Console.Write(conMessage);
                Console.SetCursorPosition(0, lineNo);
            }

            Console.SetCursorPosition(0, lineNo);
        });
        try
        {
            await _connection.StartAsync(cancellationToken);
            return true;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Error when connecting");
            Console.WriteLine(ex.Message);
            //Console.WriteLine(ex.StackTrace);
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

    public void SetToken(string accessToken)
    {
        _accessToken = accessToken;
    }
}
