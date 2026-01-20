using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.SystemToolsShared;

public /*open*/ class MessageLogger
{
    private readonly ILogger? _logger;
    private readonly IMessagesDataManager? _messagesDataManager;
    private readonly string? _userName;
    protected readonly bool UseConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MessageLogger(ILogger? logger, IMessagesDataManager? messagesDataManager, string? userName, bool useConsole)
    {
        _logger = logger;
        _messagesDataManager = messagesDataManager;
        _userName = userName;
        UseConsole = useConsole;
    }

    public async ValueTask LogInfoAndSendMessage(string message, CancellationToken cancellationToken = default)
    {
        if (UseConsole)
        {
            Console.WriteLine(message);
        }
        else
        {
#pragma warning disable CA2254 // Template should be a static expression
            _logger?.LogInformation(message);
#pragma warning restore CA2254 // Template should be a static expression
        }

        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
        }
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1,
        CancellationToken cancellationToken = default)
    {
        if (UseConsole)
        {
            Console.WriteLine(message, arg1);
        }
        else
        {
#pragma warning disable CA2254 // Template should be a static expression
            _logger?.LogInformation(message, arg1);
#pragma warning restore CA2254 // Template should be a static expression
        }

        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName,
                string.Format(CultureInfo.InvariantCulture, message, arg1), cancellationToken);
        }
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken = default)
    {
        if (UseConsole)
        {
            Console.WriteLine(message, arg1, arg2);
        }
        else
        {
#pragma warning disable CA2254
            _logger?.LogInformation(message, arg1, arg2);
#pragma warning restore CA2254
        }

        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName,
                string.Format(CultureInfo.InvariantCulture, message, arg1, arg2), cancellationToken);
        }
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
        CancellationToken cancellationToken = default)
    {
        if (UseConsole)
        {
#pragma warning disable S2234
            Console.WriteLine(message, arg1, arg2, arg3);
#pragma warning restore S2234
        }
        else
        {
#pragma warning disable CA2254
            _logger?.LogInformation(message, arg1, arg2, arg3);
#pragma warning restore CA2254
        }

        if (_messagesDataManager is not null)
            // Fix S2234: Use an explicit array to ensure argument order matches
        {
            await _messagesDataManager.SendMessage(_userName,
                string.Format(CultureInfo.InvariantCulture, message, new[] { arg1, arg2, arg3 }), cancellationToken);
        }
    }

    protected async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
        object? arg4, CancellationToken cancellationToken = default)
    {
        if (UseConsole)
        {
            Console.WriteLine(message, arg1, arg2, arg3, arg4);
        }
        else
        {
#pragma warning disable CA2254
            _logger?.LogInformation(message, arg1, arg2, arg3, arg4);
#pragma warning restore CA2254
        }

        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName,
                string.Format(CultureInfo.InvariantCulture, message, arg1, arg2, arg3, arg4), cancellationToken);
        }
    }

    protected async ValueTask LogWarningAndSendMessage(string message, CancellationToken cancellationToken = default)
    {
        StShared.WriteWarningLine(message, UseConsole, _logger);
        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
        }
    }

    protected async ValueTask LogWarningAndSendMessage(string message, object? arg1,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteWarningLine(string.Format(CultureInfo.InvariantCulture, message, arg1), UseConsole, _logger);
        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName,
                string.Format(CultureInfo.InvariantCulture, message, arg1), cancellationToken);
        }
    }

    protected async ValueTask LogWarningAndSendMessage(string message, object? arg1, object? arg2,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteWarningLine(string.Format(CultureInfo.InvariantCulture, message, arg1, arg2), UseConsole,
            _logger);
        if (_messagesDataManager is not null)
        {
            await _messagesDataManager.SendMessage(_userName,
                string.Format(CultureInfo.InvariantCulture, message, arg1, arg2), cancellationToken);
        }
    }

    protected async ValueTask<Err[]> LogErrorAndSendMessageFromError(string errorCode, string message,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteErrorLine(message, UseConsole, _logger);

        if (_messagesDataManager is null)
        {
            return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
        }

        await _messagesDataManager.SendMessage(_userName, message, cancellationToken);
        return [new Err { ErrorCode = errorCode, ErrorMessage = message }];
    }

    public async ValueTask<Err[]> LogErrorAndSendMessageFromError(Err error,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteErrorLine(error.ErrorMessage, UseConsole, _logger);

        if (_messagesDataManager is null)
        {
            return [error];
        }

        await _messagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        return [error];
    }

    protected async ValueTask<Err> LogErrorAndSendMessageFromException(Exception ex, string methodName,
        CancellationToken cancellationToken = default)
    {
        StShared.WriteException(ex, UseConsole, _logger);
        var error = SystemToolsErrors.ErrorCaught(methodName, ex.Message);
        if (_messagesDataManager is null)
        {
            return error;
        }

        await _messagesDataManager.SendMessage(_userName, error.ErrorMessage, cancellationToken);
        return error;
    }
}
