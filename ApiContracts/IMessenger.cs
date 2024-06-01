namespace ApiContracts;

public interface IMessenger
{
    Task SendMessage(string message, CancellationToken cancellationToken);
}