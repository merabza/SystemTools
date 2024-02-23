using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ReCounterDom;

public class MessagesHub : Hub
{
    private readonly IProgressDataManager _progressDataManager;
    private readonly ILogger<MessagesHub> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MessagesHub(IProgressDataManager progressDataManager , ILogger<MessagesHub> logger)
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