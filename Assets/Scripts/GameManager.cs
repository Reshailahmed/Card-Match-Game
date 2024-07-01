using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public DynamicGridLayout dynamicGridLayout;
    public RectTransform panelTransform;
    public List<CardDataScriptableObject> cardDataList;
    public int numPairs;
    private Queue<Card> cardQueue = new Queue<Card>();
    private bool canSelect = false;
    private GridLayoutGroup layoutGroup;
    public GameObject EmptyGameObject;
    public ScoreManager scoreManager;

    void Start()
    {
        layoutGroup = panelTransform.GetComponent<GridLayoutGroup>();

        if (!GameConstants.GameState && PlayerPrefs.HasKey("SavedGame"))
        {
            LoadGame();
        }
        else
        {
            if (GameConstants.Rows * GameConstants.Columns % 2 > 0 || GameConstants.Rows * GameConstants.Columns == 0)
            {
                GameConstants.Rows = 4;
                GameConstants.Columns = 4;
            }

            SetupGame(GameConstants.Rows, GameConstants.Columns);
        }

        if (scoreManager != null)
        {
            scoreManager.LoadScore();
        }
    }

    void SetupGame(int rows, int columns)
    {
        dynamicGridLayout.SetGridSize(rows, columns);
        PlayerPrefs.SetInt("Score", 0);

        foreach (Transform child in panelTransform)
        {
            Destroy(child.gameObject);
        }

        int totalCards = rows * columns;

        if (numPairs <= 0 || numPairs > cardDataList.Count)
            numPairs = Mathf.Min(cardDataList.Count, totalCards / 2);

        int totalPairs = totalCards / 2;

        List<CardDataScriptableObject> selectedCards = new List<CardDataScriptableObject>();

        for (int i = 0; i < numPairs; i++)
        {
            selectedCards.Add(cardDataList[i]);
            selectedCards.Add(cardDataList[i]);
        }

        while (selectedCards.Count < totalCards)
        {
            for (int i = 0; i < numPairs && selectedCards.Count < totalCards; i++)
            {
                selectedCards.Add(cardDataList[i]);
                selectedCards.Add(cardDataList[i]);
            }
        }

        Shuffle(selectedCards);

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, panelTransform);
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(selectedCards[i].sprite, selectedCards[i].cardName, this);
        }

        StartCoroutine(ShowCardsForSeconds(3));
    }

    public IEnumerator ShowCardsForSeconds(int seconds)
    {
        foreach (Transform child in panelTransform)
        {
            if(child.CompareTag("Card"))
            {
                Card card = child.GetComponent<Card>();
                if (card != null)
                {
                    StartCoroutine(card.RotateCard());
                }
            }
        }
        yield return new WaitForSeconds(seconds);
        foreach (Transform child in panelTransform)
        {
            if (child.CompareTag("Card"))
            {
                            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                StartCoroutine(card.RotateCard());
            }
            }
        }
        DisableLayoutComponents();
        canSelect = true;
    }

    public bool CanSelectCard()
    {
        return canSelect;
    }

    public void CardSelected(Card card)
    {
        cardQueue.Enqueue(card);
        if (cardQueue.Count >= 2)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    IEnumerator ProcessQueue()
    {
        while (cardQueue.Count >= 2)
        {
            Card firstCard = cardQueue.Dequeue();
            Card secondCard = cardQueue.Dequeue();

            yield return new WaitForSeconds(1f);

            if (firstCard.cardName == secondCard.cardName)
            {
                int index = firstCard.gameObject.transform.GetSiblingIndex();

                firstCard.gameObject.SetActive(false);
                secondCard.gameObject.SetActive(false);
                Destroy(firstCard.gameObject);

                GameObject tempObject = Instantiate(EmptyGameObject, panelTransform);
                tempObject.transform.SetSiblingIndex(index);

                yield return new WaitForEndOfFrame();
                int index2 = secondCard.gameObject.transform.GetSiblingIndex();
                GameObject tempObject2 = Instantiate(EmptyGameObject, panelTransform);
                tempObject2.transform.SetSiblingIndex(index2);
                Destroy(secondCard.gameObject);
                OnMatch();
            }
            else
            {
                StartCoroutine(firstCard.RotateCard());
                StartCoroutine(secondCard.RotateCard());
                OnMissMatch();
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        EnableLayoutComponents();
        scoreManager.SaveScore();
        List<CardSaveData> saveData = new List<CardSaveData>();

        for (int i = 0; i < panelTransform.childCount; i++)
        {
            Transform cardTransform = panelTransform.GetChild(i);
            Card card = cardTransform.GetComponent<Card>();
            if (card != null)
            {
                CardSaveData data = new CardSaveData();
                data.cardName = card.cardName;
                data.tag = "Card";

                int row = i / GameConstants.Columns;
                int column = i % GameConstants.Columns;

                data.row = row;
                data.column = column;

                saveData.Add(data);
            }
            else
            {
                CardSaveData data = new CardSaveData();
                data.cardName = "Empty";

                int row = i / GameConstants.Columns;
                int column = i % GameConstants.Columns;

                data.row = row;
                data.column = column;

                saveData.Add(data);
            }
        }

        CardSaveDataList saveDataList = new CardSaveDataList();
        saveDataList.cards = saveData;
        saveDataList.gridRows = GameConstants.Rows;
        saveDataList.gridColumns = GameConstants.Columns;

        string jsonData = JsonUtility.ToJson(saveDataList);
        PlayerPrefs.SetString("SavedGame", jsonData);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        foreach (Transform child in panelTransform)
        {
            Destroy(child.gameObject);
        }

        string jsonData = PlayerPrefs.GetString("SavedGame");
        CardSaveDataList saveDataList = JsonUtility.FromJson<CardSaveDataList>(jsonData);

        GameConstants.Rows = saveDataList.gridRows;
        GameConstants.Columns = saveDataList.gridColumns;

        dynamicGridLayout.SetGridSize(GameConstants.Rows, GameConstants.Columns);

        foreach (CardSaveData data in saveDataList.cards)
        {
            if (data.tag == "Card")
            {
                int index = data.row * GameConstants.Columns + data.column;

                GameObject cardObj = Instantiate(cardPrefab, panelTransform);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(GetSpriteByName(data.cardName), data.cardName, this);
                card.transform.SetSiblingIndex(index);
            }
            else
            {
                int index = data.row * GameConstants.Columns + data.column;

                GameObject tempObject = Instantiate(EmptyGameObject, panelTransform);
                tempObject.transform.SetSiblingIndex(index);
            }
        }

        PlayerPrefs.DeleteKey("SavedGame");
        scoreManager.LoadScore();

        StartCoroutine(ShowCardsForSeconds(3));
    }

    void DisableLayoutComponents()
    {
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }
    }

    void EnableLayoutComponents()
    {
        if (layoutGroup != null)
        {
            layoutGroup.enabled = true;
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private Sprite GetSpriteByName(string cardName)
    {
        foreach (CardDataScriptableObject cardData in cardDataList)
        {
            if (cardData.cardName == cardName)
            {
                return cardData.sprite;
            }
        }
        return null;
    }

    #region SoundManager

    public void OnMatch()
    {
        EventManager.Instance.NotifyObservers("MatchImpact");
    }

    public void OnMissMatch()
    {
        EventManager.Instance.NotifyObservers("MissMatchImpact");
    }

    #endregion

    [System.Serializable]
    private class CardSaveData
    {
        public string cardName;
        public int row;
        public int column;
        public string tag;
    }

    [System.Serializable]
    private class CardSaveDataList
    {
        public List<CardSaveData> cards;
        public int gridRows;
        public int gridColumns;
    }
}
