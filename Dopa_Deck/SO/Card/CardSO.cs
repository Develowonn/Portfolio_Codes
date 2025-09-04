using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    public string      cardName;
    public Sprite      cardSprite;
    [TextArea(2, 10)]
    public string      cardInfo;
    public CardType    cardType;
    [Space(10)]
    public Card        card;
}