using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;
using DG.Tweening;
using DeckPlayer.Managers;
using System.Linq;

struct CardGroupSumInfo
{
    public List<CardData> cardGroup;
    public int sumOfValues;
}

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
    public Dictionary<CardData, Card> cardDict = new Dictionary<CardData, Card>();

    private CardTheme _currentTheme = CardTheme.white;

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

    public IEnumerator DrawTestCards(Action OnComplete = null)
    {
        Vector3 initialCardOffset = new Vector3(0f, initialCardPosOffsetY, 0f);
        WaitForSeconds cardDrawDelay = new WaitForSeconds(0.1f);

        for (int i = 0; i < initialCardAmount; i++)
        {
            Card newCard = CreateCard(TestManager.Instance.testInputCardDatas[i]);
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
        cardDict.Add(cardData, card);
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

    #region CARD-THEMES

    public void ChangeCardsTheme()
    {
        Array values = Enum.GetValues(typeof(CardTheme));

        System.Random random = new System.Random();
        CardTheme randomTheme = (CardTheme)values.GetValue(random.Next(values.Length));

        if(_currentTheme != randomTheme)
        {
            foreach (Card card in currentCards)
                card.ChangeTheme(randomTheme);

            _currentTheme = randomTheme;
        }
        else
            ChangeCardsTheme();
    }

    #endregion

    #region SORTING

    // Function to find consecutive ranges
    private static List<string> ConsecutiveRanges(List<CardData> cardList)
    {
        int length = 1;
        List<string> list = new List<string>();

        if (cardList.Count == 0)
            return list;

        for (int i = 1; i <= cardList.Count; i++)
        {
            if (i == cardList.Count || cardList[i].value - cardList[i - 1].value != 1)
            {
                if (length == 1)
                    list.Add(string.Join("", cardList[i - length].value));
                else
                    list.Add(cardList[i - length].value + " -> " + cardList[i - 1].value);
                
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
    private static Tuple<List<List<CardData>>, List<CardData>> FindConsecutiveCards(List<CardData> sortedCardList)
    {
        List<List<CardData>> consecutiveLists = sortedCardList.Select((item, idx) => new { I = item, G = item.value - idx })
                                                          .Distinct()
                                                          .GroupBy( ig => ig.G,
                                                                    ig => ig.I,
                                                                    (k, g) => g.ToList()).ToList();
        List<List<CardData>> resultLists = new List<List<CardData>>();
        List<CardData> leftovers = new List<CardData>();

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
    }

    public static Tuple<List<List<CardData>>, List<CardData>> OneTwoThreeSort(List<CardData> listOfCards)
    {
        List<CardData> sortedResult = new List<CardData>();
        List<CardData> leftovers = new List<CardData>();

        // group same card suits
        List<CardData> spades = new List<CardData>();
        List<CardData> diamonds = new List<CardData>();
        List<CardData> hearts = new List<CardData>();
        List<CardData> clubs = new List<CardData>();
            

        for(int i = 0; i < listOfCards.Count; i++)
        {
            CardSuit currentSuit = listOfCards[i].cardSuit;

            if (currentSuit == CardSuit.spades)
                spades.Add(listOfCards[i]);
            else if (currentSuit == CardSuit.diamonds)
                diamonds.Add(listOfCards[i]);
            else if (currentSuit == CardSuit.hearts)
                hearts.Add(listOfCards[i]);
            else if (currentSuit == CardSuit.clubs)
                clubs.Add(listOfCards[i]);
        }

        // find consecutive numbers in groups (3 OR MORE)
        spades = spades.OrderBy(Card => Card.value).ToList();
        diamonds = diamonds.OrderBy(Card => Card.value).ToList();
        hearts = hearts.OrderBy(Card => Card.value).ToList();
        clubs = clubs.OrderBy(Card => Card.value).ToList();

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

        Tuple<List<List<CardData>>, List<CardData>> resultTuple;
        List<List<CardData>> sortedLists = new List<List<CardData>>();

        resultTuple = FindConsecutiveCards(spades);
        if(resultTuple.Item1.Count > 0)
            foreach(List<CardData> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(diamonds);
        if (resultTuple.Item1.Count > 0)
            foreach (List<CardData> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(hearts);
        if (resultTuple.Item1.Count > 0)
            foreach (List<CardData> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        resultTuple = FindConsecutiveCards(clubs);
        if (resultTuple.Item1.Count > 0)
            foreach (List<CardData> sortedList in resultTuple.Item1)
                sortedLists.Add(sortedList);
        leftovers.AddRange(resultTuple.Item2);

        return Tuple.Create(sortedLists, leftovers);
    }

    public static Tuple<List<List<CardData>>, List<CardData>> TripleSevenSort(List<CardData> listOfCards, bool trios = true, bool quartets = true)
    {
        List<List<CardData>> sortedResultGroups = new List<List<CardData>>();
        List<CardData> leftovers = new List<CardData>();

        List<CardData> tempList = listOfCards;
        List<CardData> nonNumberedCards = tempList.Where(card => card.cardType != CardType.numbered && card.cardType != CardType.ace).ToList();

        foreach (CardData card in nonNumberedCards)
            leftovers.Add(card);

        tempList = tempList.Except(nonNumberedCards).ToList();

        // group same numbers
        IEnumerable<List<CardData>> sameNumberCardGroups = tempList.GroupBy(cardData => cardData.value).Select(cardDataGroup => cardDataGroup.ToList()).ToList();
        List<CardData> sortedGroup = new List<CardData>();

        foreach (List<CardData> group in sameNumberCardGroups)
        {     
            sortedGroup = group.GroupBy(cardData => cardData.cardSuit)
                                              .Select(cardSuit => cardSuit.First())
                                              .ToList();

            if(sortedGroup.Count == 3 && trios)
            {
                sortedResultGroups.Add(sortedGroup);
                leftovers.AddRange(group.Except(sortedGroup));
            }
            else if(sortedGroup.Count == 4 && quartets)
            {
                sortedResultGroups.Add(sortedGroup);
                leftovers.AddRange(group.Except(sortedGroup));
            }
            else
                leftovers.AddRange(group);   
        }

        return Tuple.Create(sortedResultGroups, leftovers);
    }

    public static Tuple<List<List<CardData>>, List<CardData>> SmartSort(List<CardData> listOfCards)
    {
        List<List<CardData>> sortedResultGroups = new List<List<CardData>>();
        List<CardData> leftovers = new List<CardData>();

        // First do a 1-2-3 sort, and 7-7-7 for it's leftovers
        Tuple<List<List<CardData>>, List<CardData>> oneTwoThreeSort = OneTwoThreeSort(listOfCards);
        Tuple<List<List<CardData>>, List<CardData>> extraTripleSevenTrios = TripleSevenSort(oneTwoThreeSort.Item2, true, false);
        Tuple<List<List<CardData>>, List<CardData>> extraTripleSevenQuartets = TripleSevenSort(oneTwoThreeSort.Item2, false, true);

        // Then do a 7-7-7 sort, and 1-2-3 for it's leftovers
        Tuple<List<List<CardData>>, List<CardData>> tripleSevenSortTrios = TripleSevenSort(listOfCards, true, false);
        Tuple<List<List<CardData>>, List<CardData>> extraOneTwoThreeForTrios = OneTwoThreeSort(tripleSevenSortTrios.Item2);

        Tuple<List<List<CardData>>, List<CardData>> tripleSevenSortQuartets = TripleSevenSort(listOfCards, false, true);
        Tuple<List<List<CardData>>, List<CardData>> extraOneTwoThreeForQuartets = OneTwoThreeSort(tripleSevenSortQuartets.Item2);

        #region old-algo
        //List<CardGroupSumInfo> cardGroupSumInfos = new List<CardGroupSumInfo>();

        //int sumOfValues = 0;
        //foreach(List<CardData> cardGroup in oneTwoThreeValues.Union(tripleSevenValues))
        //{
        //    foreach(CardData card in cardGroup)           
        //        sumOfValues = sumOfValues + card.value;

        //    CardGroupSumInfo groupInfo = new CardGroupSumInfo();
        //    groupInfo.cardGroup = cardGroup;
        //    groupInfo.sumOfValues = sumOfValues;
        //    cardGroupSumInfos.Add(groupInfo);

        //    sumOfValues = 0;
        //}

        //// Order the list by descending sums
        //cardGroupSumInfos.Sort((s1, s2) => s1.sumOfValues.CompareTo(s2.sumOfValues));
        //cardGroupSumInfos.Reverse();

        //int totalAdded = 0;
        //for(int i = 0; i < cardGroupSumInfos.Count; i++)
        //{
        //    if (totalAdded + cardGroupSumInfos[i].cardGroup.Count > listOfCards.Count)
        //        break;
        //    else
        //    {
        //        sortedResultGroups.Add(cardGroupSumInfos[i].cardGroup);
        //        totalAdded = totalAdded + cardGroupSumInfos[i].cardGroup.Count;
        //    }
        //}

        #endregion

        //List<List<CardData>> unionOfSortedGroups = oneTwoThreeValues.Union(extraTripleSevenTrios.Item1).Union(extraTripleSevenQuartets.Item1)
        //                                                            .Union(tripleSevenTrioValues).Union(extraOneTwoThreeForTrios.Item1)
        //                                                            .Union(tripleSevenQuartetValues).Union(extraOneTwoThreeForQuartets.Item1)
        //                                                            .ToList();

        List<List<CardData>> unionOfSortedGroups = new List<List<CardData>>();
        unionOfSortedGroups.AddRange(oneTwoThreeSort.Item1);
        unionOfSortedGroups.AddRange(extraTripleSevenTrios.Item1);
        unionOfSortedGroups.AddRange(extraTripleSevenQuartets.Item1);
        unionOfSortedGroups.AddRange(tripleSevenSortTrios.Item1);
        unionOfSortedGroups.AddRange(extraOneTwoThreeForTrios.Item1);
        unionOfSortedGroups.AddRange(tripleSevenSortQuartets.Item1);
        unionOfSortedGroups.AddRange(extraOneTwoThreeForQuartets.Item1);

        List<List<List<CardData>>> distinctCombinationsOfGroups = new List<List<List<CardData>>>();

        bool hasIntersection = false;
        List<List<CardData>> distinctGroupsList = new List<List<CardData>>();

        for (int i = 0; i < unionOfSortedGroups.Count; i++)
        {
            distinctGroupsList.Add(unionOfSortedGroups[i]);

            for (int j = 0; j < unionOfSortedGroups.Count; j++)
            {
                for(int k = 0; k < distinctGroupsList.Count; k++)
                {
                    List<CardData> distinctGroup = distinctGroupsList[k];

                    if (unionOfSortedGroups[j] == distinctGroup || unionOfSortedGroups[j].Intersect(distinctGroup).Count() > 0)
                    {
                        hasIntersection = true;
                        break;
                    }
                    else
                        hasIntersection = false;
                }

                if (!hasIntersection)
                    distinctGroupsList.Add(unionOfSortedGroups[j]);
            }

            if(distinctGroupsList.Count() > 1)
            {
                distinctCombinationsOfGroups.Add(distinctGroupsList);
                distinctGroupsList.Clear();
            }
        }

        foreach(List<List<CardData>> distinctCombination in distinctCombinationsOfGroups)
        {
            
        }

        // find leftovers
        leftovers = listOfCards;
        for (int i = 0; i < sortedResultGroups.Count; i++)
            leftovers = leftovers.Except(sortedResultGroups[i]).ToList();

        return Tuple.Create(sortedResultGroups, leftovers);
    }

    #endregion
}
