using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;

public class CardManager : MonoBehaviour
{
    [Header("Card-Manager Config")]
    public int initialCardAmount;
    public GameObject cardPrefab;
    public Transform cardDeck;
    
    public List<CardData> currentCardDatas; // 11 cards
    private CardDataCollection cardCollection; // 52 card datas
    private List<Transform> cardSlots;

    private void Awake()
    {
        cardCollection = Resources.Load<CardDataCollection>("CardData/CardDataCollection");
        cardSlots = new List<Transform>();  
        PrepareDeck();
    }

    private void Start()
    {
        SelectCards();
    }
    public void PrepareDeck()
    {
        foreach(Transform cardSlot in cardDeck)
            cardSlots.Add(cardSlot); // TODO: Make this dynamic
    }

    private void SelectCards()
    {
        CardData[] collection = cardCollection.GetCollection();
        int collectionAmount = collection.Length;

        for (int i = 0; i < initialCardAmount; i++)
        {
            int randomIndex = Random.Range(0, collectionAmount);
            currentCardDatas.Add(collection[randomIndex]);
            GameObject newCard = CreateCard(collection[randomIndex]);
            newCard.transform.SetParent(cardSlots[i], false);
        }
    }

    private GameObject CreateCard(CardData cardData)
    {
        GameObject cardObject = Instantiate(cardPrefab);
        Card card = cardObject.GetComponent<Card>();
        card.InitCard(cardData);
        return cardObject;
    }
}
