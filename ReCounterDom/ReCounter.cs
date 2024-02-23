using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReCounterDom;

public class ReCounter
{
    private readonly string _processName;
    private readonly IProgressDataManager _progressDataManager;

    private readonly ReCounterLogger? _rLogger;

    //private readonly IServiceScopeFactory _scopeFactory;
    private int _byLevelPosition;
    private int _procPosition;

    protected ReCounter(string superName, string processName, IProgressDataManager progressDataManager,
        string? reCounterLogsFolderName = null)
    {
        //var projectSettings = DataBaseReCounterSettings.Create(configuration);
        //_rLogger = projectSettings is not null
        //    ? ReCounterLogger.Create(projectSettings.ReCounterLogsFolderName, superName)
        //    : null;
        _rLogger = reCounterLogsFolderName is not null
            ? ReCounterLogger.Create(reCounterLogsFolderName, superName)
            : null;
        _processName = processName;
        _progressDataManager = progressDataManager;
        //_scopeFactory = scopeFactory;
    }


    protected void LogMessage(string name, string message, bool instantly = false)
    {
        _rLogger?.LogMessage(message);
        _progressDataManager.SetProgressData(name, message, instantly);
    }

    private void SetProgressValue(string name, int value, bool instantly = false)
    {
        _progressDataManager.SetProgressData(name, value, instantly);
    }

    protected void SetProcLength(int length)
    {
        _procPosition = 0;
        SetProcPosition();
        SetProgressValue("procLength", length, true);
    }

    private void SetProcessRun(bool runState)
    {
        _progressDataManager.SetProgressData("ProcessRun", runState);
    }

    private void SetProcPosition()
    {
        SetProgressValue("procPosition", _procPosition);
    }

    private void ClearProgress()
    {
        SetByLevelLength(0);
        SetProcLength(0);
    }

    protected void SetByLevelLength(int length, int realToDo = -1)
    {
        _byLevelPosition = 0;
        if (realToDo != -1)
        {
            _byLevelPosition = length - realToDo;
            _procPosition += _byLevelPosition;
            SetProcPosition();
        }

        SetByLevelPosition();
        SetProgressValue("byLevelLength", length, true);
    }

    private void SetByLevelPosition()
    {
        SetProgressValue("byLevelPosition", _byLevelPosition);
    }

    protected void IncreaseProcPosition()
    {
        _procPosition++;
        SetProcPosition();
    }

    protected void IncreaseByLevelPosition()
    {
        _byLevelPosition++;
        SetByLevelPosition();
    }

    private void OnFinishReCounter()
    {
        ClearProgress();
        _progressDataManager.StopTimer();
    }

    protected virtual void LogLevelMessage(string message)
    {
        LogMessage("checkBase", "");
        LogMessage("changedBase", "");
        LogMessage("levelName", message, true);
    }

    protected virtual void LogProcMessage(string message)
    {
        LogMessage("levelName", "");
        LogMessage("procName", message, true);
    }

    protected bool IsCancellationRequested(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
            return false;

        LogProcMessage($"{_processName} შეჩერებულია");
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
            if (IsCancellationRequested(cancellationToken))
                return;

            //using var scope = _scopeFactory.CreateScope();
            SetProcessRun(true);

            LogProcMessage($"დაიწყო {_processName}");

            await RunRecount(cancellationToken);

            LogProcMessage($"{_processName} დასრულდა");
        }
        catch (TaskCanceledException)
        {
            LogMessage("procName", "Operation was canceled");
            throw;
        }
        catch (OperationCanceledException)
        {
            LogMessage("procName", "Operation was canceled");
            throw;
        }
        catch (Exception e)
        {
            LogMessage("error", e.Message);
            throw;
        }
        finally
        {
            OnFinishReCounter();
            SetProcessRun(false);
        }
    }
}