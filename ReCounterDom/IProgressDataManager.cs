namespace ReCounterDom;

public interface IProgressDataManager
{
    void SetProgressData(string name, string message, bool instantly);
    void SetProgressData(string name, int value, bool instantly = false);
    void SetProgressData(string name, bool value, bool instantly = true);
    void StopTimer();
    void UserConnected(string connectionId);
    void UserDisconnected(string connectionId);
}