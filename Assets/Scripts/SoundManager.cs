using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour, IGameEventObserver
{
    public AudioClip buttonClickSound;
    public AudioClip clickOnCardSound;
    public AudioClip matchImpactSound;
    public AudioClip missMatchImpactSound;

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> soundDictionary;

    private static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();

            soundDictionary = new Dictionary<string, AudioClip>
            {
                { "ButtonClick", buttonClickSound },
                { "ClickOnCard", clickOnCardSound },
                { "MatchImpact", matchImpactSound },
                { "MissMatchImpact", missMatchImpactSound }
            };
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterObserver("ButtonClick", this);
        EventManager.Instance.RegisterObserver("ClickOnCard", this);
        EventManager.Instance.RegisterObserver("MatchImpact", this);
        EventManager.Instance.RegisterObserver("MissMatchImpact", this);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterObserver("ButtonClick", this);
        EventManager.Instance.UnregisterObserver("ClickOnCard", this);
        EventManager.Instance.UnregisterObserver("MatchImpact", this);
        EventManager.Instance.UnregisterObserver("MissMatchImpact", this);
    }

    public void OnEventRaised(string eventType, object parameter)
    {
        if (soundDictionary.TryGetValue(eventType, out AudioClip clip))
        {
            PlaySound(clip);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
