using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReCounterContracts;
using SystemToolsShared.Errors;

namespace ReCounterDom;

public class ReCounter
{
    private readonly string? _userName;
    private readonly string _processName;
    private readonly IProgressDataManager _progressDataManager;

    private readonly ReCounterLogger? _rLogger;

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


    protected async Task LogErrors(IEnumerable<Err> errors, CancellationToken cancellationToken)
    {
        foreach (var error in errors)
            await LogMessage(ReCounterConstants.Error, error.ErrorMessage, true, cancellationToken);
    }

    protected async Task LogMessage(string name, string message, bool instantly, CancellationToken cancellationToken)
    {
        _rLogger?.LogMessage(message);
        await _progressDataManager.SetProgressData(_userName, name, message, instantly, cancellationToken);
    }

    private async Task SetProgressValue(string name, int value, bool instantly, CancellationToken cancellationToken)
    {
        await _progressDataManager.SetProgressData(_userName, name, value, instantly, cancellationToken);
    }

    protected async Task SetProcLength(int length, CancellationToken cancellationToken)
    {
        _procPosition = 0;
        await SetProcPosition(cancellationToken);
        await SetProgressValue(ReCounterConstants.ProcLength, length, true, cancellationToken);
    }

    private async Task SetProcessRun(bool runState, CancellationToken cancellationToken)
    {
        await _progressDataManager.SetProgressData(_userName, ReCounterConstants.ProcessRun, runState, true,
            cancellationToken);
    }

    private async Task SetProcPosition(CancellationToken cancellationToken)
    {
        await SetProgressValue(ReCounterConstants.ProcPosition, _procPosition, false, cancellationToken);
    }

    private async Task ClearProgress(CancellationToken cancellationToken)
    {
        await SetByLevelLength(0, -1, cancellationToken);
        await SetProcLength(0, cancellationToken);
    }

    //protected საჭიროა AppGrammarGe პროექტისათვის
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task SetByLevelLength(int length, int realToDo, CancellationToken cancellationToken)
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

    private async Task SetByLevelPosition(CancellationToken cancellationToken)
    {
        await SetProgressValue(ReCounterConstants.ByLevelPosition, _byLevelPosition, false, cancellationToken);
    }

    protected async Task IncreaseProcPosition(CancellationToken cancellationToken)
    {
        _procPosition++;
        await SetProcPosition(cancellationToken);
    }

    protected async Task IncreaseByLevelPosition(CancellationToken cancellationToken)
    {
        _byLevelPosition++;
        await SetByLevelPosition(cancellationToken);
    }

    private async Task OnFinishReCounter(CancellationToken cancellationToken)
    {
        await ClearProgress(cancellationToken);
        _progressDataManager.StopTimer();
    }

    protected virtual async Task LogLevelMessage(string message, CancellationToken cancellationToken)
    {
        await LogMessage(ReCounterConstants.LevelName, message, true, cancellationToken);
    }

    protected virtual async Task LogProcMessage(string message, CancellationToken cancellationToken)
    {
        await LogMessage(ReCounterConstants.LevelName, string.Empty, true, cancellationToken);
        await LogMessage(ReCounterConstants.ProcName, message, true, cancellationToken);
    }

    protected virtual async Task<Exception> LogProcMessageAndException(string message,
        CancellationToken cancellationToken)
    {
        await LogProcMessage(message, cancellationToken);
        return new Exception(message);
    }

    //protected საჭიროა AppGrammarGe პროექტისათვის
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<bool> IsCancellationRequested(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
            return false;

        await LogProcMessage($"{_processName} შეჩერებულია", cancellationToken);
        return true;
    }

    protected virtual Task RunRecount(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task Recount(CancellationToken cancellationToken)
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