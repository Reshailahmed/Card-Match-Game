using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Card Data")]
public class CardDataScriptableObject : ScriptableObject
{
    public string cardName;
    public Sprite sprite;
}
