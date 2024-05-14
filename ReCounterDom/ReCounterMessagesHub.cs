using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRContracts;

namespace ReCounterDom;

// ReSharper disable once ClassNeverInstantiated.Global
public class ReCounterMessagesHub : Hub<IProgressDataMessenger>
{
    private readonly ILogger<ReCounterMessagesHub> _logger;
    private readonly IProgressDataManager _progressDataManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ReCounterMessagesHub(IProgressDataManager progressDataManager, ILogger<ReCounterMessagesHub> logger)
    {
        _progressDataManager = progressDataManager;
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        //_userCount ++;
        _logger.LogInformation("OnConnectedAsync");
        _progressDataManager.UserConnected(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        //_userCount --;
        _logger.LogInformation("OnDisconnectedAsync");
        _progressDataManager.UserDisconnected(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}