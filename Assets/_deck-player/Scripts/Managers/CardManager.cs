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
    private Tuple<List<List<Card>>, List<Card>> FindConsecutiveCards(List<Card> sortedCardList)
    {
        List<List<Card>> consecutiveLists = sortedCardList.Select((item, idx) => new { I = item, G = item.GetValue() - idx })
                                                          .Distinct()
                                                          .GroupBy( ig => ig.G,
                                                                    ig => ig.I,
                                                                    (k, g) => g.ToList()).ToList();
        List<List<Card>> resultLists = new List<List<Card>>();
        List<Card> leftovers = new List<Card>();

        for (int i = 0; i < consecutiveLists.Count; i++)
        {
            if (consecutiveLists[i].Count >= 3)
            {
                resultLists.Add(consecutiveLists[i]);
            }
            else
                leftovers.AddRange(consecutiveLists[i]);
        }

        return Tuple.Create(resultLists, leftovers);

        #region old Algo
        //List<List<Card>> result = new List<List<Card>>();
        //List<Card> leftovers = new List<Card>();

        //if (sortedCardList.Count == 0)
        //    return Tuple.Create(result, leftovers);
        //else if(sortedCardList.Count > 0 && sortedCardList.Count < 3)
        //{
        //    foreach(Card card in sortedCardList)
        //        leftovers.Add(card);    
        //    return Tuple.Create(result, leftovers);
        //}
        //else
        //{
        //    int firstValue;
        //    int secondValue;
        //    int thirdValue;
        //    int skipLoop = 0;

        //    for (int i = 0; i < sortedCardList.Count; i++)
        //    {
        //        if(i == sortedCardList.Count - 1)
        //        {
        //            leftovers.Add(sortedCardList[i]);
        //            return Tuple.Create(result, leftovers);
        //        }
        //        else if(i == sortedCardList.Count - 2)
        //        {
        //            leftovers.Add(sortedCardList[i]);
        //            leftovers.Add(sortedCardList[i + 1]);
        //            return Tuple.Create(result, leftovers);
        //        }
        //        else
        //        {
        //            if (skipLoop != 0)
        //            {
        //                if (skipLoop <= 3)
        //                {
        //                    skipLoop++;
        //                    continue;
        //                }
        //                else
        //                    skipLoop = 0;
        //            }

        //            // Check if this is a consecutive trio
        //            firstValue = sortedCardList[i].GetValue();
        //            secondValue = sortedCardList[i + 1].GetValue();
        //            thirdValue = sortedCardList[i + 2].GetValue();

        //            if (secondValue - firstValue == 1 && thirdValue - secondValue == 1)
        //            {
        //                // a consecutive trio!
        //                List<Card> tempList = new List<Card>();
        //                tempList.Add(sortedCardList[i]);
        //                tempList.Add(sortedCardList[i + 1]);
        //                tempList.Add(sortedCardList[i + 2]);

        //                for (int j = 0; j < sortedCardList.Count - (i + 3); j++)
        //                {
        //                    if (sortedCardList[(i + 2) + (j + 1)].GetValue() - sortedCardList[(i + 2) + j].GetValue() == 1)
        //                    {
        //                        tempList.Add(sortedCardList[(i + 2) + j]);
        //                    }
        //                    else
        //                        skipLoop = 1;

        //                    result.Add(tempList);
        //                    break;
        //                }
        //            }
        //            else
        //                leftovers.Add(sortedCardList[i]);
        //        }

        //    }

        //    return Tuple.Create(result, leftovers);
        //}
        #endregion
    }

    public Tuple<List<List<Card>>, List<Card>> OneTwoThreeSort()
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

        Tuple<List<List<Card>>, List<Card>> resultTuple;
        List<List<Card>> sortedLists = new List<List<Card>>();

        resultTuple = FindConsecutiveCards(spades);
        if(resultTuple.Item1.Count > 0)
            foreach(List<Card> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(diamonds);
        if (resultTuple.Item1.Count > 0)
            foreach (List<Card> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(hearts);
        if (resultTuple.Item1.Count > 0)
            foreach (List<Card> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(clubs);
        if (resultTuple.Item1.Count > 0)
            foreach (List<Card> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        return Tuple.Create(sortedLists, leftovers);
    }
    public Tuple<List<List<Card>>, List<Card>> TripleSevenSort()
    {
        List<List<Card>> sortedResultGroups = new List<List<Card>>();
        List<Card> leftovers = new List<Card>();

        // group same numbers
        IEnumerable<List<Card>> sameNumberCardGroups = currentCards.GroupBy(card => card.GetValue()).Select(group => group.ToList()).ToList();
        List<Card> sortedGroup = new List<Card>();

        foreach (List<Card> group in sameNumberCardGroups)
        {     
            sortedGroup = group.GroupBy(card => card.GetSymbol())
                                              .Select(symbol => symbol.First())
                                              .ToList();

            if(sortedGroup.Count == 3 || sortedGroup.Count == 4)
            {
                sortedResultGroups.Add(sortedGroup);
                leftovers.AddRange(group.Except(sortedGroup));
            }
            else
                leftovers.AddRange(group);   
        }

        return Tuple.Create(sortedResultGroups, leftovers);
    }
    public List<Card> SmartSort()
    {
        // sort here
        currentCards.Reverse();
        return currentCards;
    }

    #endregion
}
