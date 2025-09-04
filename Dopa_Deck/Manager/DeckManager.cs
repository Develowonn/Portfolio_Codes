// # System
using System;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private Transform   deckParent;

    [Header("UI")]
    [SerializeField]
    private CardUI[]    cardUiList;
	[SerializeField]
	private float		usedDisappearDelay;
	[SerializeField]
	private float		cardCooltime;

	private List<Card>  myDeck;

    private Card        selectedCard;
    private int         selectedCardIndex;
	private bool		isUsingCard;

	private void Awake()
	{
		myDeck = new List<Card>();
	}

	private void Start()
    {
        InitializeDeck();
    }

	private void Update()
	{
		if (!_03_Game.Instance.IsIntro()) return;
			SelectCardByInput();
	}

	private void InitializeDeck()
    {
        for(int i = 0; i < Constants.MyDeck.MaxDeckSize; i++)
        {
            CardSO  cardSO  = CardManager.Instance.GetRandomCardSO();
            cardUiList[i].Initialize(cardSO.cardName, cardSO.cardInfo, cardSO.cardSprite, cardSO.cardType);
            cardUiList[i].ActivateBack();

            Card card = Instantiate(cardSO.card);
            card.transform.SetParent(deckParent);
			myDeck.Add(card);
        }
    }

	public void SelectCardByInput()
	{
		if (Input.GetKeyDown(KeyManager.Instance.GetFirstCardKey()))
		{
            SelectCard(myDeck[Constants.MyDeck.FirstCard], Constants.MyDeck.FirstCard);
		}
		else if (Input.GetKeyDown(KeyManager.Instance.GetSecondCardKey()))
		{
			SelectCard(myDeck[Constants.MyDeck.SecondCard], Constants.MyDeck.SecondCard);
		}
		else if (Input.GetKeyDown(KeyManager.Instance.GetThirdCardKey()))
		{
			SelectCard(myDeck[Constants.MyDeck.ThirdCard], Constants.MyDeck.ThirdCard);
		}
	}

	public void SelectCard(Card selectedCard, int selectedCardIndex)
	{
		if(isUsingCard) return;

		// ���õ� ī�带 �� �� �� Ŭ�� �ߴٸ� ��ų ���
		if (IsSameCardSelected(selectedCardIndex))
		{
			isUsingCard = true;
			UseCard().Forget();

			return;
		}

		// ������ ������ ī�尡 �ִٸ� �������� 
		UnSelectPreviousCard(selectedCardIndex);

		this.selectedCardIndex = selectedCardIndex;
		this.selectedCard      = selectedCard;

		cardUiList[selectedCardIndex].OnSelected();
	}

	public async UniTask UseCard()
    {
		// ���õ� ī�带 ���� ������ ���̰�
		cardUiList[selectedCardIndex].transform.SetAsLastSibling();

		cardUiList[selectedCardIndex].ActivateFront();
		cardUiList[selectedCardIndex].OnUsed();

		selectedCard.Execute();

		await UniTask.Delay(TimeSpan.FromSeconds(cardUiList[selectedCardIndex].GetUsedMoveDuration() + usedDisappearDelay));
		cardUiList[selectedCardIndex].gameObject.SetActive(false);

		// ī�� ��Ÿ�� UI�� ǥ��
		foreach(var cardUI in cardUiList)
        {
			cardUI.ProcessCooltime(cardCooltime).Forget();
        }

		await UniTask.Delay(TimeSpan.FromSeconds(cardCooltime));
		isUsingCard = false;
	}

	private bool IsSameCardSelected(int newIndex)
	{
		return selectedCard != null && selectedCardIndex == newIndex;
	}

	private void UnSelectPreviousCard(int newIndex)
	{
		if(selectedCard != null && selectedCardIndex != newIndex)
		{
			cardUiList[selectedCardIndex].OnUnSelected();
		}
	}

	public void RemoveCard(Card card)
    {
		Destroy(card);
    }
}