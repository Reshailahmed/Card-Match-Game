using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image frontImage; // Reference to the front image of the card
    public GameObject back; // Reference to the back of the card (optional if you have animations or other functionality)
    public Sprite cardImage;
    public string cardType; // Use a string to represent the card type
    private bool isRevealed = false;
    private GameManager gameManager;

    public void Initialize(Sprite image, GameManager manager)
    {
        cardImage = image;
        frontImage.sprite = image;
        cardType = image.name;
        gameManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isRevealed && gameManager.CanSelectCard())
        {
            Flip();
            gameManager.CardSelected(this);
        }
    }

    public void Flip()
    {
        isRevealed = !isRevealed;
        frontImage.gameObject.SetActive(isRevealed);
        if (back != null)
        {
            back.SetActive(!isRevealed);
        }
    }

    public bool IsRevealed()
    {
        return isRevealed;
    }
}
