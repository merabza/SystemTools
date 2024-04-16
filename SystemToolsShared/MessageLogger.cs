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
        Logger.LogInformation(message);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, message, cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        Logger.LogInformation(message, arg1);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, CancellationToken cancellationToken)
    {
        Logger.LogInformation(message, arg1, arg2);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken)
    {
        Logger.LogInformation(message, arg1, arg2, arg3);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2, arg3), cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, CancellationToken cancellationToken)
    {
        Logger.LogWarning(message);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, message,
                cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        Logger.LogWarning(message, arg1);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, object? arg2, CancellationToken cancellationToken)
    {
        Logger.LogWarning(message, arg1, arg2);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(string errorCode, string message, CancellationToken cancellationToken)
    {
        Logger.LogError(message);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, message, cancellationToken);
        return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(Err error, CancellationToken cancellationToken)
    {
        Logger.LogError(error.ErrorMessage);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        return [error];
    }
}