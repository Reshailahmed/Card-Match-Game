using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ScoreManager : MonoBehaviour, IGameEventObserver
{
    public TMP_Text scoreText;
    private int score;
    private int comboCounter;
    private Coroutine comboCoroutine;
    private GameManager gameManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EventManager.Instance.RegisterObserver("MatchImpact", this);
        EventManager.Instance.RegisterObserver("MissMatchImpact", this);

        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnDestroy()
    {
        EventManager.Instance.UnregisterObserver("MatchImpact", this);
        EventManager.Instance.UnregisterObserver("MissMatchImpact", this);
    }

    public void OnEventRaised(string eventType, object parameter)
    {
        if (eventType == "MatchImpact")
        {
            OnMatch();
        }
        else if (eventType == "MissMatchImpact")
        {
            OnMissMatch();
        }
    }

    private void OnMatch()
    {
        if (comboCounter == 1)
        {
            score += 20;
            comboCounter = 0;
            StartCoroutine(RevealAllCardsForSeconds(1));
        }
        else
        {
            score += 5;
            comboCounter++;
        }

        UpdateScoreText();

        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
        comboCoroutine = StartCoroutine(ComboTimer());
    }

    private void OnMissMatch()
    {
        comboCounter = 0;
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
            comboCoroutine = null;
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(5);
        comboCounter = 0;
    }

    private IEnumerator RevealAllCardsForSeconds(int seconds)
    {
        if (gameManager != null)
        {
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(gameManager.ShowCardsForSeconds(seconds));
        }
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    public void LoadScore()
    {
        if (PlayerPrefs.HasKey("Score"))
        {
            score = PlayerPrefs.GetInt("Score");
            UpdateScoreText();
        }
    }
}
