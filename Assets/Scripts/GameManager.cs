using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public DynamicGridLayout dynamicGridLayout;
    public RectTransform panelTransform;
    public List<CardDataScriptableObject> cardDataList; // List of ScriptableObjects
    public int numPairs;
    private Queue<Card> cardQueue = new Queue<Card>();
    private bool canSelect = false;
    private GridLayoutGroup layoutGroup;
    public GameObject EmptyGameObject;

    void Start()
    {
        layoutGroup = panelTransform.GetComponent<GridLayoutGroup>();

        if (!GameConstants.GameState && PlayerPrefs.HasKey("SavedGame"))
        {
            LoadGame();
        }
        else
        {

            // Ensuring the total grid cell number isn't odd or zero
            if (GameConstants.Rows * GameConstants.Columns % 2 > 0 || GameConstants.Rows * GameConstants.Columns == 0)
            {
                GameConstants.Rows = 4; GameConstants.Columns = 4;
            }

            SetupGame(GameConstants.Rows, GameConstants.Columns);
        }
    }

    void SetupGame(int rows, int columns)
    {
        dynamicGridLayout.SetGridSize(rows, columns);

        // Clear existing cards
        foreach (Transform child in panelTransform)
        {
            Destroy(child.gameObject);
        }

        // Calculate the total number of cards
        int totalCards = rows * columns;

        // Adjust numPairs if necessary
        if (numPairs <= 0 || numPairs > cardDataList.Count)
            numPairs = Mathf.Min(cardDataList.Count, totalCards / 2);

        // Calculate the total number of pairs needed
        int totalPairs = totalCards / 2;

        // Create and shuffle card data
        List<CardDataScriptableObject> selectedCards = new List<CardDataScriptableObject>();

        // Add pairs of each available card image
        for (int i = 0; i < numPairs; i++)
        {
            selectedCards.Add(cardDataList[i]);
            selectedCards.Add(cardDataList[i]);
        }

        // If there are fewer pairs than needed, repeat the existing pairs
        while (selectedCards.Count < totalCards)
        {
            for (int i = 0; i < numPairs && selectedCards.Count < totalCards; i++)
            {
                selectedCards.Add(cardDataList[i]);
                selectedCards.Add(cardDataList[i]);
            }
        }

        // Shuffle the list of card data
        Shuffle(selectedCards);

        // Instantiate cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, panelTransform);
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(selectedCards[i].sprite, selectedCards[i].cardName, this);
        }

        StartCoroutine(ShowCardsForSeconds(3));
    }

    IEnumerator ShowCardsForSeconds(int seconds)
    {
        foreach (Transform child in panelTransform)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                card.Flip();
            }
        }
        yield return new WaitForSeconds(seconds);
        foreach (Transform child in panelTransform)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                card.Flip();
            }
        }
        DisableLayoutComponents();
        canSelect = true; // Allow card selection after the initial display period
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

            yield return new WaitForSeconds(1f); // Adjust delay as needed

            if (firstCard.cardName == secondCard.cardName) // Compare by card name
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
            }
            else
            {
                // Flip cards back if they do not match
                firstCard.Flip();
                secondCard.Flip();
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
        List<CardSaveData> saveData = new List<CardSaveData>();

        // Iterate through each child (card) in the panel
        for (int i = 0; i < panelTransform.childCount; i++)
        {
            Transform cardTransform = panelTransform.GetChild(i);
            Card card = cardTransform.GetComponent<Card>();
            if (card != null)
            {
                // Create save data for the card
                CardSaveData data = new CardSaveData();
                data.cardName = card.cardName;
                data.tag = "Card";

                // Calculate the row and column indices
                int row = i / GameConstants.Columns;
                int column = i % GameConstants.Columns;

                data.row = row;
                data.column = column;

                // Add the data to the save list
                saveData.Add(data);
            }
            else
            {
                // Create save data for the card
                CardSaveData data = new CardSaveData();
                data.cardName = "Empty";

                // Calculate the row and column indices
                int row = i / GameConstants.Columns;
                int column = i % GameConstants.Columns;

                data.row = row;
                data.column = column;

                // Add the data to the save list
                saveData.Add(data);
            }
        }

        CardSaveDataList saveDataList = new CardSaveDataList();
        saveDataList.cards = saveData;
        saveDataList.gridRows = GameConstants.Rows;
        saveDataList.gridColumns = GameConstants.Columns;

        // Convert save data to JSON string and save to PlayerPrefs
        string jsonData = JsonUtility.ToJson(saveDataList);
        PlayerPrefs.SetString("SavedGame", jsonData);
        PlayerPrefs.Save();

        Debug.Log("SavedGame: " + jsonData);
    }


    public void LoadGame()
    {
        // Destroy existing cards
        foreach (Transform child in panelTransform)
        {
            Destroy(child.gameObject);
        }

        // Load saved data
        string jsonData = PlayerPrefs.GetString("SavedGame");
        CardSaveDataList saveDataList = JsonUtility.FromJson<CardSaveDataList>(jsonData);
        Debug.Log("Loading: " + jsonData);

        // Update GameConstants with saved grid rows and columns
        GameConstants.Rows = saveDataList.gridRows;
        GameConstants.Columns = saveDataList.gridColumns;

        // Set the grid size
        dynamicGridLayout.SetGridSize(GameConstants.Rows, GameConstants.Columns);

        // Instantiate cards based on saved data
        foreach (CardSaveData data in saveDataList.cards)
        {
            if (data.tag == "Card")
            {
                // Calculate the index in the child list based on row and column
                int index = data.row * GameConstants.Columns + data.column;

                GameObject cardObj = Instantiate(cardPrefab, panelTransform);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(GetSpriteByName(data.cardName), data.cardName, this);

                // Move the instantiated card to its saved position (row and column)
                card.transform.SetSiblingIndex(index);
            }
            else
            {
                // Calculate the index in the child list based on row and column
                int index = data.row * GameConstants.Columns + data.column;

                GameObject tempObject = Instantiate(EmptyGameObject, panelTransform);

                tempObject.transform.SetSiblingIndex(index);
            }
        }

        PlayerPrefs.DeleteKey("SavedGame");

        Debug.Log("Game loaded.");
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

    private Sprite GetSpriteByName(string cardName)
    {
        foreach (CardDataScriptableObject cardData in cardDataList)
        {
            if (cardData.cardName == cardName)
            {
                return cardData.sprite;
            }
        }
        return null; // Handle case where sprite is not found
    }
}