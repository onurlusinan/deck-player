using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;
using DG.Tweening;
using DeckPlayer.Managers;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("CardManager Config")]
    public float initialCardPosOffsetY = -600f;
    public float cardDrawDelay = 0.2f;
    public GameObject cardPrefab;
    public List<Card> currentCards; // 11 cards

    private int initialCardAmount = 11;
    private CardDataCollection cardCollection; // 52 card datas

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Load card data collection
        cardCollection = Resources.Load<CardDataCollection>("CardData/CardDataCollection");
    }

    /// <summary>
    /// Draws random cards as much as the initialCardAmount
    /// </summary>
    public IEnumerator DrawRandomCards(Action OnComplete = null)
    {
        CardData[] collection = cardCollection.GetCollection();
        int collectionAmount = collection.Length;

        Vector3 initialCardOffset = new Vector3(0f, initialCardPosOffsetY, 0f);

        WaitForSeconds cardDrawDelay = new WaitForSeconds(0.1f);

        int randomIndex;

        for (int i = 0; i < initialCardAmount; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, collectionAmount);
            Card newCard = CreateCard(collection[randomIndex]);
            CardSlot cardSlot = DeckManager.Instance.GetCardSlot(i).GetComponent<CardSlot>();

            newCard.transform.SetParent(cardSlot.transform, false);

            // set current card slot
            newCard.currentSlot = cardSlot;
            cardSlot.currentCard = newCard;
            
            // draw animation
            RectTransform cardRect = newCard.GetComponent<RectTransform>();
            cardRect.anchoredPosition = initialCardOffset;
            cardRect.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.25f).SetEase(Ease.OutExpo);

            yield return cardDrawDelay;
        }

        OnComplete?.Invoke();
    }

    /// <summary>
    /// Creates and initiates card from card data
    /// </summary>
    private Card CreateCard(CardData cardData)
    {
        GameObject cardObject = Instantiate(cardPrefab);
        Card card = cardObject.GetComponent<Card>();
        currentCards.Add(card);
        card.InitCard(cardData);
        return card;
    }

    #region Sorting

    public List<Card> OneTwoThreeSort()
    {
        // sort here
        currentCards.Reverse();
        return currentCards;
    }
    public List<Card> TripleSevenSort()
    {
        // sort here
        currentCards.Reverse();
        return currentCards;
    }
    public List<Card> SmartSort()
    {
        // sort here
        currentCards.Reverse();
        return currentCards;
    }

    #endregion
}
