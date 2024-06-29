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
    private bool canSelect = true;
    public GridLayoutGroup layoutGroup;

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

        // Calculate how many pairs to instantiate per card image
        if (numPairs <= 0)
            numPairs = cardImages.Count;
        int pairsPerImage = Mathf.CeilToInt((float)(rows * columns) / numPairs);

        // Create and shuffle cards
        List<Sprite> selectedImages = new List<Sprite>();

        // Add pairs of each available card image
        for (int i = 0; i < numPairs; i++)
        {
            for (int j = 0; j < pairsPerImage; j++)
            {
                selectedImages.Add(cardImages[i % cardImages.Count]);
            }
        }

        // Shuffle the list of card images
        Shuffle(selectedImages);

        // Instantiate cards
        for (int i = 0; i < rows * columns; i++)
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
            canSelect = false;

            Card firstCard = cardQueue.Dequeue();
            Card secondCard = cardQueue.Dequeue();

            yield return new WaitForSeconds(1);
            Debug.Log("firstCard: " + firstCard.cardType + " " + "secondCard: " + secondCard.cardType);
            if (firstCard.cardType == secondCard.cardType)
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

        canSelect = true;
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
