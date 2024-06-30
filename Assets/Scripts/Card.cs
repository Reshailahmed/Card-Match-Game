using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image frontImage;
    public GameObject back;
    public string cardName; // Updated to cardName
    private bool isRevealed = false;
    private GameManager gameManager;

    public void Initialize(Sprite image, string name, GameManager manager)
    {
        frontImage.sprite = image;
        cardName = name;
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
