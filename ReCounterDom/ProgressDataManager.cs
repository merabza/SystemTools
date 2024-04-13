using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ReCounterDom.Models;

namespace ReCounterDom;

public class ProgressDataManager : IProgressDataManager, IDisposable, IAsyncDisposable
{
    private static readonly object SyncRoot = new();
    private readonly HashSet<string> _connectedIds = [];
    private readonly IHubContext<MessagesHub> _hub;
    private readonly ILogger<ProgressDataManager> _logger;
    private int _currentChangeId;
    private ProgressData? _currentData;
    private ProgressData? _lastChangesData;
    private int _sentChangeId;

    private Timer? _timer;
    private bool _timerStarted;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ProgressDataManager(IHubContext<MessagesHub> hub, ILogger<ProgressDataManager> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_timer != null) await _timer.DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer?.Dispose();
    }


    //public void Dispose()
    //{
    //    _timer?.Dispose();
    //}

    public void UserConnected(string connectionId)
    {
        _connectedIds.Add(connectionId);
        _lastChangesData = _currentData;
    }

    public void UserDisconnected(string connectionId)
    {
        _connectedIds.Remove(connectionId);
    }

    public void StopTimer()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timerStarted = false;
        _logger.LogInformation("ProgressDataManager Timer stopped.");
    }


    public void SetProgressData(string name, string message, bool instantly = false)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            _lastChangesData ??= new ProgressData();
            _lastChangesData.Add(name, message);
            _currentData ??= new ProgressData();
            _currentData.Add(name, message);
            _currentChangeId++;
        }

        if (instantly && _connectedIds.Count > 0)
            SendData(_lastChangesData);
    }

    public void SetProgressData(string name, bool value, bool instantly = true)
    {
        lock (SyncRoot)
        {
            _lastChangesData ??= new ProgressData();
            _lastChangesData.Add(name, value);
            _currentData ??= new ProgressData();
            _currentData.Add(name, value);
            _currentChangeId++;
        }

        if (instantly && _connectedIds.Count > 0)
            SendData(_lastChangesData);
    }

    public void SetProgressData(string name, int value, bool instantly = false)
    {
        CheckTimer();
        lock (SyncRoot)
        {
            _lastChangesData ??= new ProgressData();
            _lastChangesData.Add(name, value);
            _currentData ??= new ProgressData();
            _currentData.Add(name, value);
            _currentChangeId++;
        }

        if (instantly && _connectedIds.Count > 0)
            SendData(_lastChangesData);
    }

    private void StartTimer()
    {
        _logger.LogInformation("ProgressDataManager Timer running.");
        _timerStarted = true;
        // ReSharper disable once DisposableConstructor
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void DoWork(object? state)
    {
        if (_sentChangeId == _currentChangeId)
            return;
        if (_lastChangesData is not null)
            SendData(_lastChangesData);
    }

    private void SendData(ProgressData progressData)
    {
        _sentChangeId = _currentChangeId;
        SendNotificationAsync(progressData).Wait();
        _lastChangesData = null;
    }

    private Task SendNotificationAsync(ProgressData progressData)
    {
        return _hub.Clients.All.SendAsync("sendtoall", progressData);
    }

    private void CheckTimer()
    {
        if (!_timerStarted && _connectedIds.Count > 0)
            StartTimer();
        if (_timerStarted && _connectedIds.Count == 0)
            StopTimer();
    }
}