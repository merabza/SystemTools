using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using SystemTools.SystemToolsShared;

namespace SystemTools.BackgroundTasks;

public /*open*/ class ToolAction : MessageLogger
{
    //protected საჭიროა SupportTools პროექტისათვის
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly ILogger? Logger;

    //protected საჭიროა ProcessorWorker პროექტისათვის
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly string ToolActionName;

    private readonly ResiliencePipeline<bool>? _retryPipeline;

    protected ToolAction(ILogger? logger, string actionName, IMessagesDataManager? messagesDataManager,
        string? userName, bool useConsole = false, ResiliencePipeline<bool>? retryPipeline = null) : base(logger,
        messagesDataManager, userName, useConsole)
    {
        Logger = logger;
        ToolActionName = actionName;
        _retryPipeline = retryPipeline;
    }

    public async Task<bool> Run(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!CheckValidate())
            {
                return false;
            }

            await LogInfoAndSendMessage($"{ToolActionName} Started...", UseConsole, cancellationToken);

            //დავინიშნოთ დრო პროცესისათვის
            DateTime startDateTime = DateTime.Now;

            bool success = _retryPipeline is null
                ? await RunAction(cancellationToken)
                : await _retryPipeline.ExecuteAsync(RunAction, cancellationToken);

            string timeTakenMessage = StShared.TimeTakenMessage(startDateTime);

            await LogInfoAndSendMessage($"{ToolActionName} Finished. {timeTakenMessage}", UseConsole,
                cancellationToken);

            //StShared.Pause();

            return success;
        }
        catch (OperationCanceledException)
        {
            StShared.WriteErrorLine("Operation Canceled", UseConsole, Logger);
        }
        catch (Exception e)
        {
            StShared.WriteErrorLine($"Error when run Tool Action: {e.Message}", UseConsole, Logger);
        }

        return false;
    }

    //გამოყენებულაი SupportTools-ში
    protected virtual bool CheckValidate()
    {
        return true;
    }

    protected virtual ValueTask<bool> RunAction(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(true);
    }
}
