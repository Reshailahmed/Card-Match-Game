using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image frontImage;
    public GameObject back;
    public string cardName;
    private bool isRevealed = false;
    private GameManager gameManager;
    private bool coroutineAllowed = true;

    public void Initialize(Sprite image, string name, GameManager manager)
    {
        frontImage.sprite = image;
        cardName = name;
        gameManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isRevealed && gameManager.CanSelectCard() && coroutineAllowed)
        {
            StartCoroutine(RotateCard());
            gameManager.CardSelected(this);
        }
    }

    public IEnumerator RotateCard()
    {
        coroutineAllowed = false;

        if (!isRevealed)
        {
            for (float i = 0f; i <= 180f; i += 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    frontImage.gameObject.SetActive(true);
                    if (back != null)
                    {
                        back.SetActive(false);
                    }
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            for (float i = 180f; i >= 0f; i -= 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    frontImage.gameObject.SetActive(false);
                    if (back != null)
                    {
                        back.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        coroutineAllowed = true;
        isRevealed = !isRevealed;
    }

    public bool IsRevealed()
    {
        return isRevealed;
    }

    public void OnCardClick()
    {
        EventManager.Instance.NotifyObservers("ClickOnCard");
    }
}
