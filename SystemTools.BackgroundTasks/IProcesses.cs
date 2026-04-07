using System.Threading.Tasks;

namespace SystemTools.BackgroundTasks;

public interface IProcesses
{
    bool IsBusy();
    Task WaitForFinishAll();
    void CancelProcesses();
    ProcessManager GetNewProcessManager();
}
