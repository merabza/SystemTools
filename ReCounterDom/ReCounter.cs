using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReCounterContracts;
using SystemToolsShared.Errors;

namespace ReCounterDom;

public class ReCounter
{
    private readonly string _processName;
    private readonly IProgressDataManager _progressDataManager;

    private readonly ReCounterLogger? _rLogger;
    private readonly string? _userName;

    private int _byLevelPosition;
    private int _procPosition;

    protected ReCounter(string? userName, string superName, string processName,
        IProgressDataManager progressDataManager, string? reCounterLogsFolderName = null)
    {
        _rLogger = reCounterLogsFolderName is not null
            ? ReCounterLogger.Create(reCounterLogsFolderName, superName)
            : null;
        _userName = userName;
        _processName = processName;
        _progressDataManager = progressDataManager;
    }


    protected async Task LogErrors(IEnumerable<Err> errors, CancellationToken cancellationToken = default)
    {
        foreach (var error in errors)
            await LogMessage(ReCounterConstants.Error, error.ErrorMessage, true, cancellationToken);
    }

    protected ValueTask LogMessage(string name, string message, bool instantly,
        CancellationToken cancellationToken = default)
    {
        _rLogger?.LogMessage(message);
        return _progressDataManager.SetProgressData(_userName, name, message, instantly, cancellationToken);
    }

    private ValueTask SetProgressValue(string name, int value, bool instantly,
        CancellationToken cancellationToken = default)
    {
        return _progressDataManager.SetProgressData(_userName, name, value, instantly, cancellationToken);
    }

    protected async Task SetProcLength(int length, CancellationToken cancellationToken = default)
    {
        _procPosition = 0;
        await SetProcPosition(cancellationToken);
        await SetProgressValue(ReCounterConstants.ProcLength, length, true, cancellationToken);
    }

    private ValueTask SetProcessRun(bool runState, CancellationToken cancellationToken = default)
    {
        return _progressDataManager.SetProgressData(_userName, ReCounterConstants.ProcessRun, runState, true,
            cancellationToken);
    }

    private ValueTask SetProcPosition(CancellationToken cancellationToken = default)
    {
        return SetProgressValue(ReCounterConstants.ProcPosition, _procPosition, false, cancellationToken);
    }

    private async Task ClearProgress(CancellationToken cancellationToken = default)
    {
        await SetByLevelLength(0, -1, cancellationToken);
        await SetProcLength(0, cancellationToken);
    }

    //protected საჭიროა AppGrammarGe პროექტისათვის
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task SetByLevelLength(int length, int realToDo, CancellationToken cancellationToken = default)
    {
        _byLevelPosition = 0;
        if (realToDo != -1)
        {
            _byLevelPosition = length - realToDo;
            _procPosition += _byLevelPosition;
            await SetProcPosition(cancellationToken);
        }

        await SetByLevelPosition(cancellationToken);
        await SetProgressValue(ReCounterConstants.ByLevelLength, length, true, cancellationToken);
    }

    private ValueTask SetByLevelPosition(CancellationToken cancellationToken = default)
    {
        return SetProgressValue(ReCounterConstants.ByLevelPosition, _byLevelPosition, false, cancellationToken);
    }

    protected ValueTask IncreaseProcPosition(CancellationToken cancellationToken = default)
    {
        _procPosition++;
        return SetProcPosition(cancellationToken);
    }

    protected ValueTask IncreaseByLevelPosition(CancellationToken cancellationToken = default)
    {
        _byLevelPosition++;
        return SetByLevelPosition(cancellationToken);
    }

    private async Task OnFinishReCounter(CancellationToken cancellationToken = default)
    {
        await ClearProgress(cancellationToken);
        _progressDataManager.StopTimer();
    }

    protected virtual ValueTask LogLevelMessage(string message, CancellationToken cancellationToken = default)
    {
        return LogMessage(ReCounterConstants.LevelName, message, true, cancellationToken);
    }

    protected virtual async Task LogProcMessage(string message, CancellationToken cancellationToken = default)
    {
        await LogMessage(ReCounterConstants.LevelName, string.Empty, true, cancellationToken);
        await LogMessage(ReCounterConstants.ProcName, message, true, cancellationToken);
    }

    protected virtual async Task<Exception> LogProcMessageAndException(string message,
        CancellationToken cancellationToken = default)
    {
        await LogProcMessage(message, cancellationToken);
        return new Exception(message);
    }

    //protected საჭიროა AppGrammarGe პროექტისათვის
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<bool> IsCancellationRequested(CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
            return false;

        await LogProcMessage($"{_processName} შეჩერებულია", cancellationToken);
        return true;
    }

    protected virtual Task RunRecount(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task Recount(CancellationToken cancellationToken = default)
    {
        try
        {
            //შემოწმდეს გაჩერება ხომ არ მოითხოვეს
            if (await IsCancellationRequested(cancellationToken))
                return;

            await SetProcessRun(true, cancellationToken);

            await LogProcMessage($"დაიწყო {_processName}", cancellationToken);

            await RunRecount(cancellationToken);

            await LogProcMessage($"{_processName} დასრულდა", cancellationToken);
        }
        catch (TaskCanceledException)
        {
            await LogMessage(ReCounterConstants.ProcName, "Operation was canceled", false, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await LogMessage(ReCounterConstants.ProcName, "Operation was canceled", false, cancellationToken);
        }
        catch (Exception e)
        {
            await LogMessage(ReCounterConstants.Error, e.Message, false, cancellationToken);
            throw;
        }
        finally
        {
            await OnFinishReCounter(cancellationToken);
            await SetProcessRun(false, cancellationToken);
        }
    }
}