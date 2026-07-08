using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using SystemTools.ApiContracts;
using SystemTools.ReCounterContracts.V1.Routes;

namespace SystemTools.ReCounterContracts;

public sealed class ReCounterMessageHubClient : IMessageHubClient
{
    private readonly string? _apiKey;
    private readonly Dictionary<string, int> _keyLines = new();
    private readonly List<int> _lineLengths = [];
    private readonly string _server;
    private string? _accessToken;

    private HubConnection? _connection;
    private int _startLine = -1;

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

            //პროცესი დასრულებულია — მიმდინარეობის ინფორმაცია აღარ დაიბეჭდოს
            if (!ProcessMonitoringManager.Instance.ProcessIsRunning)
            {
                if (_startLine >= 0)
                {
                    Console.SetCursorPosition(0, _startLine + _keyLines.Count + 1);
                }

                return;
            }

            if (_startLine < 0)
            {
                _startLine = Console.CursorTop;
            }

            if (progressData.StrData.Count > 0)
            {
                foreach (KeyValuePair<string, string> strDataItem in new SortedDictionary<string, string>(progressData
                             .StrData))
                {
                    if (!_keyLines.TryGetValue(strDataItem.Key, out int lineOffset))
                    {
                        lineOffset = _keyLines.Count;
                        _keyLines[strDataItem.Key] = lineOffset;
                    }

                    WriteOnLine(lineOffset, $"{strDataItem.Key}: {strDataItem.Value}");
                }
            }

            int? procPosition = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcPosition);
            int? procLength = progressData.IntData.GetValueOrDefault(ReCounterConstants.ProcLength);
            string? progressValueName = progressData.StrData.GetValueOrDefault(ReCounterConstants.ProcProgressMessage);

            if (procPosition is not null && procLength is not null && (decimal)procLength > 0)
            {
                decimal procPercentage = Math.Round((decimal)procPosition / (decimal)procLength * 100);
                string conMessage =
                    $"[{_server}]: {progressValueName ?? ""} {procPosition}-{procLength} {procPercentage}%";
                WriteOnLine(_keyLines.Count, conMessage);
            }

            Console.SetCursorPosition(0, _startLine + _keyLines.Count);
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
        finally
        {
            //კურსორი პროგრესის ბლოკის ქვემოთ გადავიდეს, რომ შემდეგი ტექსტი პროგრესის ხაზებს არ გადაეწეროს
            //და მდგომარეობა განულდეს შემდეგი მონიტორინგის სესიისთვის
            if (_startLine >= 0)
            {
                Console.SetCursorPosition(0, _startLine + _keyLines.Count + 1);
                _startLine = -1;
                _keyLines.Clear();
                _lineLengths.Clear();
            }
        }

        return false;
    }

    public void SetToken(string accessToken)
    {
        _accessToken = accessToken;
    }

    private void WriteOnLine(int lineOffset, string text)
    {
        while (_lineLengths.Count <= lineOffset)
        {
            _lineLengths.Add(0);
        }

        int textLength = text.Length;
        if (_lineLengths[lineOffset] > textLength)
        {
            text = text.PadRight(_lineLengths[lineOffset]);
        }

        _lineLengths[lineOffset] = textLength;
        Console.SetCursorPosition(0, _startLine + lineOffset);
        Console.Write(text);
    }
}
