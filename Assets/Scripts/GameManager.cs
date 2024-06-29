using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public DynamicGridLayout dynamicGridLayout;
    public RectTransform panelTransform;
    public List<Sprite> cardImages;
    public int numPairs;
    public int gridRows = 4;
    public int gridColumns = 4;
    private Queue<Card> cardQueue = new Queue<Card>();
    private bool canSelect = false;
    private GridLayoutGroup layoutGroup;

    void Start()
    {
        layoutGroup = panelTransform.GetComponent<GridLayoutGroup>();
        if (gridRows * gridColumns % 2 > 0 || gridRows * gridColumns == 0) // ensuring the total grid cell number isnt odd or zero
            gridRows = 4; gridColumns = 4;
        SetupGame(gridRows, gridColumns);
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
        if (numPairs <= 0 || numPairs > cardImages.Count)
            numPairs = Mathf.Min(cardImages.Count, totalCards / 2);

        // Calculate the total number of pairs needed
        int totalPairs = totalCards / 2;

        // Create and shuffle cards
        List<Sprite> selectedImages = new List<Sprite>();

        // Add pairs of each available card image
        for (int i = 0; i < numPairs; i++)
        {
            selectedImages.Add(cardImages[i]);
            selectedImages.Add(cardImages[i]);
        }

        // If there are fewer pairs than needed, repeat the existing pairs
        while (selectedImages.Count < totalCards)
        {
            for (int i = 0; i < numPairs && selectedImages.Count < totalCards; i++)
            {
                selectedImages.Add(cardImages[i]);
                selectedImages.Add(cardImages[i]);
            }
        }

        // Shuffle the list of card images
        Shuffle(selectedImages);

        // Instantiate cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, panelTransform);
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(selectedImages[i], this);
        }

        StartCoroutine(ShowCardsForSeconds(3));
    }


    IEnumerator ShowCardsForSeconds(int seconds)
    {
        foreach (Card card in FindObjectsOfType<Card>())
        {
            card.Flip();
        }
        yield return new WaitForSeconds(seconds);
        foreach (Card card in FindObjectsOfType<Card>())
        {
            card.Flip();
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

            yield return new WaitForSeconds(5f);

            if (firstCard.cardImage.name == secondCard.cardImage.name) // Compare by sprite name
            {
                Destroy(firstCard.gameObject);
                Destroy(secondCard.gameObject);
            }
            else
            {
                firstCard.Flip();
                secondCard.Flip();
            }
        }
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
}
