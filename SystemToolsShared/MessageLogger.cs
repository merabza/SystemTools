using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SystemToolsShared;

public class MessageLogger
{
    protected readonly ILogger Logger;
    protected readonly IMessagesDataManager? MessagesDataManager;
    protected readonly string? UserName;
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected MessageLogger(ILogger logger, IMessagesDataManager? messagesDataManager, string? userName, bool useConsole)
    {
        Logger = logger;
        MessagesDataManager = messagesDataManager;
        UserName = userName;
        _useConsole = useConsole;
    }

    protected async Task LogInfoAndSendMessage(string message, CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message);
        else
            Logger.LogInformation(message);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, message, cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1);
        else
            Logger.LogInformation(message, arg1);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, string.Format(message, arg1), cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1, arg2);
        else
            Logger.LogInformation(message, arg1, arg2);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
        CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1, arg2, arg3);
        else
            Logger.LogInformation(message, arg1, arg2, arg3);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, string.Format(message, arg1, arg2, arg3),
                cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3, object? arg4,
        CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1, arg2, arg3, arg4);
        else
            Logger.LogInformation(message, arg1, arg2, arg3, arg4);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, string.Format(message, arg1, arg2, arg3, arg4),
                cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, CancellationToken cancellationToken)
    {
        StShared.WriteWarningLine(message, _useConsole, Logger);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, message, cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        StShared.WriteWarningLine(string.Format(message,arg1), _useConsole, Logger);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, string.Format(message, arg1), cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken)
    {
        StShared.WriteWarningLine(string.Format(message,arg1,arg2), _useConsole, Logger);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(string errorCode, string message,
        CancellationToken cancellationToken)
    {
        StShared.WriteErrorLine(message, _useConsole, Logger);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, message, cancellationToken);
        return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(Err error, CancellationToken cancellationToken)
    {
        StShared.WriteErrorLine(error.ErrorMessage, _useConsole, Logger);
        if (MessagesDataManager is not null)
            await MessagesDataManager.SendMessage(UserName, error.ErrorMessage, cancellationToken);
        return [error];
    }
}