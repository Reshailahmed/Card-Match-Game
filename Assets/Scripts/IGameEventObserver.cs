public interface IGameEventObserver
{
    void OnEventRaised(string eventType, object parameter);
}
