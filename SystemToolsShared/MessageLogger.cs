using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SystemToolsShared;

public class MessageLogger
{
    protected readonly ILogger Logger;
    protected readonly IMessagesDataManager? MessagesDataManager;
    private readonly string? _userName;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected MessageLogger(ILogger logger, IMessagesDataManager? messagesDataManager, string? userName)
    {
        Logger = logger;
        MessagesDataManager = messagesDataManager;
        _userName = userName;
    }

    protected async Task LogInfoAndSendMessage(string message, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, message, cancellationToken);
        Logger.LogInformation(message);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
        Logger.LogInformation(message, arg1);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
        Logger.LogInformation(message, arg1, arg2);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2, arg3), cancellationToken);
        Logger.LogInformation(message, arg1, arg2, arg3);
    }

    protected async Task LogWarningAndSendMessage(string message, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, message,
                cancellationToken);
        Logger.LogWarning(message);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
        Logger.LogWarning(message, arg1);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, object? arg2, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
        Logger.LogWarning(message, arg1, arg2);
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(string errorCode, string message, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, message, cancellationToken);
        Logger.LogError(message);
        return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(Err error, CancellationToken cancellationToken)
    {
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        Logger.LogError(error.ErrorMessage);
        return [error];
    }
}