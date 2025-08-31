using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SystemToolsShared.Errors;

namespace SystemToolsShared;

public /*open*/ class MessageLogger
{
    private readonly ILogger? _logger;
    private readonly IMessagesDataManager? _messagesDataManager;
    protected readonly bool UseConsole;
    private readonly string? _userName;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected MessageLogger(ILogger? logger, IMessagesDataManager? messagesDataManager, string? userName,
        bool useConsole)
    {
        _logger = logger;
        _messagesDataManager = messagesDataManager;
        _userName = userName;
        UseConsole = useConsole;
    }

    protected async ValueTask LogInfoAndSendMessage(string message, CancellationToken cancellationToken = default)
    {
        if (UseConsole)
            Console.WriteLine(message);
        else
            _logger?.LogInformation(message);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1,
        CancellationToken cancellationToken = default)
    {
        if (UseConsole)
            Console.WriteLine(message, arg1);
        else
            _logger?.LogInformation(message, arg1);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken = default)
    {
        if (UseConsole)
            Console.WriteLine(message, arg1, arg2);
        else
            _logger?.LogInformation(message, arg1, arg2);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
        CancellationToken cancellationToken = default)
    {
        if (UseConsole)
            Console.WriteLine(message, arg1, arg2, arg3);
        else
            _logger?.LogInformation(message, arg1, arg2, arg3);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2, arg3),
                cancellationToken);
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
        object? arg4, CancellationToken cancellationToken = default)
    {
        if (UseConsole)
            Console.WriteLine(message, arg1, arg2, arg3, arg4);
        else
            _logger?.LogInformation(message, arg1, arg2, arg3, arg4);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2, arg3, arg4),
                cancellationToken);
    }

    protected async ValueTask LogWarningAndSendMessage(string message, CancellationToken cancellationToken = default)
    {
        StShared.WriteWarningLine(message, UseConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
    }

    protected async ValueTask LogWarningAndSendMessage(string message, object? arg1,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteWarningLine(string.Format(message, arg1), UseConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
    }

    protected async ValueTask LogWarningAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteWarningLine(string.Format(message, arg1, arg2), UseConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromError(string errorCode, string message,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteErrorLine(message, UseConsole, _logger);

        if (_messagesDataManager is null)
            return [new Err { ErrorCode = errorCode, ErrorMessage = message }];

        await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
        return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
    }

    protected async ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromError(Err error,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteErrorLine(error.ErrorMessage, UseConsole, _logger);

        if (_messagesDataManager is null)
            return [error];

        await _messagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        return [error];
    }

    protected async ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromException(Exception ex, string methodName,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteException(ex, UseConsole, _logger);
        var error = SystemToolsErrors.ErrorCaught(methodName, ex.Message);
        if (_messagesDataManager is null)
            return [error];

        await _messagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        return [error];
    }
}