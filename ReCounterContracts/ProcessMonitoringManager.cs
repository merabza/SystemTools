namespace ReCounterContracts;

public sealed class ProcessMonitoringManager
{
    private static ProcessMonitoringManager? _instance;
    private static readonly object SyncRoot = new();
    //private WaitKeyboardEscapeBackgroundService? _waitKeyboardEscapeBackgroundService;


    // ReSharper disable once MemberCanBePrivate.Global
    private ProcessMonitoringManager()
    {
    }


    public static ProcessMonitoringManager Instance
    {
        get
        {
            if (_instance is not null)
                return _instance;
            lock (SyncRoot) //thread safe singleton
            {
                _instance ??= new ProcessMonitoringManager();
            }

            return _instance;
        }
    }

    public static void SetTestInstance(ProcessMonitoringManager newInstance)
    {
        _instance = newInstance;
    }

    //public bool StopWaitingKeyboard(CancellationToken cancellationToken)
    //{
    //    if (_waitKeyboardEscapeBackgroundService is null)
    //        return true;
    //    _waitKeyboardEscapeBackgroundService.StopAsync(cancellationToken);
    //    return true;
    //}

    //public Task StartWaitingKeyboard(CancellationToken cancellationToken)
    //{
    //    _waitKeyboardEscapeBackgroundService = new WaitKeyboardEscapeBackgroundService();

    //    return _waitKeyboardEscapeBackgroundService.StartAsync(cancellationToken);
    //}

    public bool ProcessIsRunning { get; set; }
}