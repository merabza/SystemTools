
namespace ReCounterContracts;

public interface IProgressDataMessenger
{
    Task SendProgressData(ProgressData progressData, CancellationToken cancellationToken);
}