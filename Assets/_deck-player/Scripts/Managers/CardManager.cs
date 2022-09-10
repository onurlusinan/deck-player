using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("Card-Manager Config")]
    public int initialCardAmount;
    public GameObject cardPrefab;
    public Transform cardDeck;
    
    public List<Card> currentCards; // 11 cards
    private CardDataCollection cardCollection; // 52 card datas
    private List<Transform> cardSlots;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cardCollection = Resources.Load<CardDataCollection>("CardData/CardDataCollection");
        cardSlots = new List<Transform>();  
        PrepareDeck();
    }

    public void PrepareDeck()
    {
        foreach(Transform cardSlot in cardDeck)
            cardSlots.Add(cardSlot); // TODO: Make this dynamic
    }

    public IEnumerator DrawRandomCards(Action OnComplete = null)
    {
        CardData[] collection = cardCollection.GetCollection();
        int collectionAmount = collection.Length;
        Vector3 initialCardOffset = new Vector3(0f, -600f, 0f);
        WaitForSeconds cardDrawDelay = new WaitForSeconds(0.2f);

        for (int i = 0; i < initialCardAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, collectionAmount);
            GameObject newCard = CreateCard(collection[randomIndex]);
            newCard.transform.SetParent(cardSlots[i], false);
            
            RectTransform cardRect = newCard.GetComponent<RectTransform>();
            cardRect.anchoredPosition = initialCardOffset;
            cardRect.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBounce);

            yield return cardDrawDelay;
        }

        OnComplete?.Invoke();
    }

    private GameObject CreateCard(CardData cardData)
    {
        GameObject cardObject = Instantiate(cardPrefab);
        Card card = cardObject.GetComponent<Card>();
        currentCards.Add(card);
        card.InitCard(cardData);
        return cardObject;
    }
}
