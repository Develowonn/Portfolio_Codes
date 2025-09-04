using UnityEngine;

public abstract class Card : MonoBehaviour
{
    private string      cardName;
    private CardType    cardType;

    public string   GetCardName() { return cardName; }
    public CardType GetCardType() { return cardType; }

    public void Initialize(string cardName, CardType cardType)
    {
        this.cardName = cardName;
        this.cardType = cardType;
    }

    public abstract void Execute();
}
