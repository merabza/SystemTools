using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SystemToolsShared;

public class MessageLogger
{
    private readonly ILogger _logger;
    private readonly IMessagesDataManager? _messagesDataManager;
    private readonly string? _userName;
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected MessageLogger(ILogger logger, IMessagesDataManager? messagesDataManager, string? userName, bool useConsole)
    {
        _logger = logger;
        _messagesDataManager = messagesDataManager;
        _userName = userName;
        _useConsole = useConsole;
    }

    protected async Task LogInfoAndSendMessage(string message, CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message);
        else
            _logger.LogInformation(message);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1);
        else
            _logger.LogInformation(message, arg1);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1, arg2);
        else
            _logger.LogInformation(message, arg1, arg2);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
        CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1, arg2, arg3);
        else
            _logger.LogInformation(message, arg1, arg2, arg3);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2, arg3),
                cancellationToken);
    }

    protected async Task LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3, object? arg4,
        CancellationToken cancellationToken)
    {
        if (_useConsole)
            Console.WriteLine(message, arg1, arg2, arg3, arg4);
        else
            _logger.LogInformation(message, arg1, arg2, arg3, arg4);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2, arg3, arg4),
                cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, CancellationToken cancellationToken)
    {
        StShared.WriteWarningLine(message, _useConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, CancellationToken cancellationToken)
    {
        StShared.WriteWarningLine(string.Format(message,arg1), _useConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1), cancellationToken);
    }

    protected async Task LogWarningAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken)
    {
        StShared.WriteWarningLine(string.Format(message,arg1,arg2), _useConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, string.Format(message, arg1, arg2), cancellationToken);
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(string errorCode, string message,
        CancellationToken cancellationToken)
    {
        StShared.WriteErrorLine(message, _useConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
        return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
    }

    protected async Task<Err[]> LogErrorAndSendMessageFromError(Err error, CancellationToken cancellationToken)
    {
        StShared.WriteErrorLine(error.ErrorMessage, _useConsole, _logger);
        if (_messagesDataManager is not null)
            await _messagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        return [error];
    }
}