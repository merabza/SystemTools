using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SystemToolsShared;

namespace ReCounterDom;

public class ReCounter
{
    public const string Error = "Error";
    public const string ProcLength = "ProcLength";
    public const string ProcessRun = "ProcessRun";
    public const string ProcPosition = "ProcPosition";
    public const string ByLevelLength = "ByLevelLength";
    public const string ByLevelPosition = "ByLevelPosition";
    public const string LevelName = "LevelName";
    public const string ProcName = "ProcName";
        
            
    


    private readonly string _processName;
    private readonly IProgressDataManager _progressDataManager;

    private readonly ReCounterLogger? _rLogger;

    private int _byLevelPosition;
    private int _procPosition;

    protected ReCounter(string superName, string processName, IProgressDataManager progressDataManager,
        string? reCounterLogsFolderName = null)
    {
        _rLogger = reCounterLogsFolderName is not null
            ? ReCounterLogger.Create(reCounterLogsFolderName, superName)
            : null;
        _processName = processName;
        _progressDataManager = progressDataManager;
    }

    
    public void LogErrors(IEnumerable<Err> errors)
    {
        foreach (var error in errors)
            LogMessage(Error, error.ErrorMessage, true);
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
        SetProgressValue(ProcLength, length, true);
    }

    private void SetProcessRun(bool runState)
    {
        _progressDataManager.SetProgressData(ProcessRun, runState);
    }

    private void SetProcPosition()
    {
        SetProgressValue(ProcPosition, _procPosition);
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
        SetProgressValue(ByLevelLength, length, true);
    }

    private void SetByLevelPosition()
    {
        SetProgressValue(ByLevelPosition, _byLevelPosition);
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
        LogMessage(LevelName, message, true);
    }

    protected virtual void LogProcMessage(string message)
    {
        LogMessage(LevelName, string.Empty);
        LogMessage(ProcName, message, true);
    }

    protected virtual Exception LogProcMessageAndException(string message)
    {
        LogProcMessage(message);
        return new Exception(message);
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

            SetProcessRun(true);

            LogProcMessage($"დაიწყო {_processName}");

            await RunRecount(cancellationToken);

            LogProcMessage($"{_processName} დასრულდა");
        }
        catch (TaskCanceledException)
        {
            LogMessage(ProcName, "Operation was canceled");
            throw;
        }
        catch (OperationCanceledException)
        {
            LogMessage(ProcName, "Operation was canceled");
            throw;
        }
        catch (Exception e)
        {
            LogMessage(Error, e.Message);
            throw;
        }
        finally
        {
            OnFinishReCounter();
            SetProcessRun(false);
        }
    }

        


}