using ApiContracts;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using ReCounterContracts.V1.Routes;

namespace ReCounterContracts;

public sealed class ReCounterMessageHubClient : IMessageHubClient
{
    private static int _lastLength;
    private static string? _lastProcName;
    private readonly string? _apiKey;
    private readonly string? _progressValueName;
    private readonly string _server;
    private string? _accessToken;

    private HubConnection? _connection;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToPrimaryConstructor
    public ReCounterMessageHubClient(string server, string? apiKey, string? progressValueName)
    {
        _server = server;
        _apiKey = apiKey;
        _progressValueName = progressValueName;
    }

    public async Task<bool> RunMessages(CancellationToken cancellationToken)
    {
        _connection = new HubConnectionBuilder().WithUrl(
            $"{_server}{RecountMessagesRoutes.ReCounterRoute.Recounter}{RecountMessagesRoutes.ReCounterRoute.Messages}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?{ApiKeysConstants.ApiKeyParameterName}={_apiKey}")}",
            options =>
            {
                //options.SkipNegotiation = true;
                options.Transports = HttpTransportType.LongPolling;
                options.AccessTokenProvider = () => Task.FromResult(_accessToken);
            }).Build();

        //_connection.On<string>(Events.MessageReceived, message => Console.WriteLine($"[{_server}]: {message}"));

        _connection.On<ProgressData>(RecounterEvents.ProgressDataReceived, progressData =>
        {
            if (progressData.StrData.Count > 0)
            {
                var procName = progressData.StrData.GetValueOrDefault(ReCounterConstants.ProcName);
                if (_lastProcName != procName)
                {
                    Console.WriteLine(procName);
                    _lastProcName = procName;
                }
            }

            int? procPosition = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcPosition);
            int? procLength = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcLength);
            var progressValueName = _progressValueName is null
                ? null
                : progressData.StrData.GetValueOrDefault(_progressValueName);

            var lineNo = Console.CursorTop;
            if (procPosition is not null && procLength is not null)
                if ((decimal)procLength > 0)
                {
                    var procPercentage = Math.Round((decimal)procPosition / (decimal)procLength * 100);
                    var conMessage =
                        $"[{_server}]: {progressValueName ?? ""} {procPosition}-{procLength} {procPercentage}%";
                    var conMessageLength = conMessage.Length;
                    if (_lastLength > conMessageLength)
                        conMessage = conMessage.PadRight(_lastLength);

                    _lastLength = conMessageLength;
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
            Console.WriteLine(ex.StackTrace);
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


    public void SetToken(string accessToken)
    {
        _accessToken = accessToken;
    }
}