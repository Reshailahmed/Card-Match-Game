using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image frontImage;
    public GameObject back;
    public string cardType;
    private bool isRevealed = false;
    private GameManager gameManager;

    public void Initialize(Sprite image, GameManager manager)
    {
        frontImage.sprite = image;
        cardType = image.name;
        gameManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogError("isRevealed: " + isRevealed);
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
