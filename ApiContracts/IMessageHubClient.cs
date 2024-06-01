namespace ApiContracts;

public interface IMessageHubClient
{
    Task RunMessages(CancellationToken cancellationToken);
    Task StopMessages(CancellationToken cancellationToken);
}