using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    private Dictionary<string, List<IGameEventObserver>> eventObservers = new Dictionary<string, List<IGameEventObserver>>();

    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EventManager");
                    instance = obj.AddComponent<EventManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        // Ensure the instance is properly destroyed when the application quits
        if (instance == this)
        {
            instance = null;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void RegisterObserver(string eventType, IGameEventObserver observer)
    {
        if (!eventObservers.ContainsKey(eventType))
        {
            eventObservers[eventType] = new List<IGameEventObserver>();
        }
        eventObservers[eventType].Add(observer);
    }

    public void UnregisterObserver(string eventType, IGameEventObserver observer)
    {
        if (eventObservers.ContainsKey(eventType))
        {
            eventObservers[eventType].Remove(observer);
        }
    }

    public void NotifyObservers(string eventType, object parameter = null)
    {
        if (eventObservers.ContainsKey(eventType))
        {
            foreach (IGameEventObserver observer in eventObservers[eventType])
            {
                observer.OnEventRaised(eventType, parameter);
            }
        }
    }
}
