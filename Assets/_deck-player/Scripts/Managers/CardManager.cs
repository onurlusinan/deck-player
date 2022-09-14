using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;
using DG.Tweening;
using DeckPlayer.Managers;
using System.Linq;

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

    #region CARD-CREATION
    /// <summary>
    /// Draws random cards as much as the initialCardAmount
    /// </summary>
    public IEnumerator DrawRandomCards(Action OnComplete = null)
    {
        CardData[] collection = cardCollection.GetCollection();
        int collectionAmount = collection.Length;

        Vector3 initialCardOffset = new Vector3(0f, initialCardPosOffsetY, 0f);

        WaitForSeconds cardDrawDelay = new WaitForSeconds(0.1f);


        List<int> uniqueRandomList = GenerateRandomUniqueIntegers(initialCardAmount, 52);

        foreach(int uniqueRandom in uniqueRandomList)
        {
            Debug.Log(uniqueRandom);
        }

        int randomIndex;

        for (int i = 0; i < initialCardAmount; i++)
        {
            randomIndex = uniqueRandomList[i];
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

    private static List<int> GenerateRandomUniqueIntegers(int amount, int maxValue)
    {
        List<int> randomList = new List<int>(amount);

        for (int i = 0; i < amount; i++)
        {
            int numToAdd = UnityEngine.Random.Range(0, maxValue);

            while (randomList.Contains(numToAdd))
                numToAdd = UnityEngine.Random.Range(0, maxValue);

            randomList.Add(numToAdd);
        }

        return randomList;
    }
    #endregion

    #region SORTING

    // Function to find consecutive ranges
    private static List<string> ConsecutiveRanges(List<Card> cardList)
    {
        int length = 1;
        List<string> list = new List<string>();

        if (cardList.Count == 0)
            return list;

        for (int i = 1; i <= cardList.Count; i++)
        {
            if (i == cardList.Count || cardList[i].GetValue() - cardList[i - 1].GetValue() != 1)
            {
                if (length == 1)
                    list.Add(string.Join("", cardList[i - length].GetValue()));
                else
                    list.Add(cardList[i - length].GetValue() +" -> " + cardList[i - 1].GetValue());
                
                length = 1;
            }
            else
                length++;
        }
        return list;
    }

    /// <summary>
    /// Finds the cards with consecutive values in a card list sorted by value
    /// </summary>
    /// <param name="sortedCardList"> Has to be sorted </param>
    /// <returns> Tuple of the result and leftovers </returns>
    private Tuple<List<Card>, List<Card>> FindConsecutiveCards(List<Card> sortedCardList)
    {
        List<Card> result = new List<Card>();
        List<Card> leftovers = new List<Card>();

        if (sortedCardList.Count == 0)
            return Tuple.Create(result, leftovers);
        else if(sortedCardList.Count == 1)
        {
            leftovers.Add(sortedCardList[0]);
            return Tuple.Create(result, leftovers);
        }    

        for (int i = 1; i < sortedCardList.Count; i++)
        {
            int valueCurrent = sortedCardList[i].GetValue();
            int valuePrev = sortedCardList[i - 1].GetValue();

            if (valueCurrent - valuePrev == 1)
            {
                if (i == 1)
                    result.Add(sortedCardList[i - 1]);

                result.Add(sortedCardList[i]);
            }
            else
            {
                if (i == 1)
                    leftovers.Add(sortedCardList[i - 1]);

                leftovers.Add(sortedCardList[i]);
            }
        }
        
        if(result.Count < 3) // 3 or more check
        {
            leftovers.AddRange(result);
            result.Clear();
        }

        return Tuple.Create(result, leftovers);
    }

    public List<Card> OneTwoThreeSort()
    {
        List<Card> sortedResult = new List<Card>();
        List<Card> leftovers = new List<Card>();

        // group same symbols
        List<Card> spades = new List<Card>();
        List<Card> diamonds = new List<Card>();
        List<Card> hearts = new List<Card>();
        List<Card> clubs = new List<Card>();

        for(int i = 0; i < currentCards.Count; i++)
        {
            CardSymbol currentSymbol = currentCards[i].GetSymbol();

            if (currentSymbol == CardSymbol.spades)
                spades.Add(currentCards[i]);
            else if (currentSymbol == CardSymbol.diamonds)
                diamonds.Add(currentCards[i]);
            else if (currentSymbol == CardSymbol.hearts)
                hearts.Add(currentCards[i]);
            else if (currentSymbol == CardSymbol.clubs)
                clubs.Add(currentCards[i]);
        }

        // find consecutive numbers in groups (3 OR MORE)
        spades = spades.OrderBy(Card => Card.GetValue()).ToList();
        diamonds = diamonds.OrderBy(Card => Card.GetValue()).ToList();
        hearts = hearts.OrderBy(Card => Card.GetValue()).ToList();
        clubs = clubs.OrderBy(Card => Card.GetValue()).ToList();

        // In order to test for now
        foreach(string x in ConsecutiveRanges(spades)) {
            Debug.Log("spades: " + x.ToString());
        }
        foreach(string x in ConsecutiveRanges(diamonds)) {
            Debug.Log("diamonds: " + x.ToString());
        }
        foreach(string x in ConsecutiveRanges(hearts)) {
            Debug.Log("hearts: " + x.ToString());
        }
        foreach(string x in ConsecutiveRanges(clubs)) {
            Debug.Log("clubs: " + x.ToString());
        }

        Tuple<List<Card>, List<Card>> resultTuple;

        resultTuple = FindConsecutiveCards(spades);
        sortedResult.AddRange(resultTuple.Item1);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(diamonds);
        sortedResult.AddRange(resultTuple.Item1);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(hearts);
        sortedResult.AddRange(resultTuple.Item1);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(clubs);
        sortedResult.AddRange(resultTuple.Item1);
        leftovers.AddRange(resultTuple.Item2);

        sortedResult.AddRange(leftovers);

        return sortedResult;
    }
    public List<Card> TripleSevenSort()
    {
        // group same numbers
        // get one from each symbol (3 OR 4)
        // group those again
        // leave the rest
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
