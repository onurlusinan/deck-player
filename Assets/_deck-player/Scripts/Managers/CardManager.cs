using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

using DeckPlayer.CardSystem;
using DeckPlayer.Audio;
using DeckPlayer.Helpers;

namespace DeckPlayer.Managers
{
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

        // preloading the sorted lists
        internal Tuple<List<List<CardData>>, List<CardData>> oneTwoThreeSortResult;
        internal Tuple<List<List<CardData>>, List<CardData>> tripleSevenSortResult;
        internal Tuple<List<List<CardData>>, List<CardData>> smartSortResult;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            // Load card data collection
            cardCollection = Resources.Load<CardDataCollection>("CardData/CardDataCollection");
        }

        #region CARD-CREATION & CARD-DRAWING

        /// <summary>
        /// Draws random cards as much as the initialCardAmount
        /// </summary>
        /// <param name="OnComplete"> OnComplete is called after the method </param>
        public IEnumerator DrawRandomCards(Action OnComplete = null)
        {
            CardData[] collection = cardCollection.GetCollection();
            int collectionAmount = collection.Length;

            Vector3 initialCardOffset = new Vector3(0f, initialCardPosOffsetY, 0f);

            WaitForSeconds cardDrawDelay = new WaitForSeconds(0.1f);


            List<int> uniqueRandomList = Helpers.GenerateRandomUniqueIntegers(initialCardAmount, 52);

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
                cardRect.GetComponent<RectTransform>().DOAnchorPosY(0f, Constants.cardDrawDuration).SetEase(Ease.OutExpo);

                if(SoundManager.Instance)
                    SoundManager.Instance.Play(Sounds.cardDraw);

                yield return cardDrawDelay;
            }

            OnComplete?.Invoke();
            PreSortLists();
        }

        /// <summary>
        /// Draws the test cards specified in the inspector
        /// </summary>
        /// <param name="OnComplete"> OnComplete is called after the method </param>
        public IEnumerator DrawTestCards(Action OnComplete = null)
        {
            Vector3 initialCardOffset = new Vector3(0f, initialCardPosOffsetY, 0f);
            WaitForSeconds cardDrawDelay = new WaitForSeconds(0.1f);

            for (int i = 0; i < initialCardAmount; i++)
            {
                Card newCard = CreateCard(GameManager.Instance.testInputCardDatas[i]);
                CardSlot cardSlot = DeckManager.Instance.GetCardSlot(i).GetComponent<CardSlot>();

                newCard.transform.SetParent(cardSlot.transform, false);

                // set current card slot
                newCard.currentSlot = cardSlot;
                cardSlot.currentCard = newCard;

                // draw animation
                RectTransform cardRect = newCard.GetComponent<RectTransform>();
                cardRect.anchoredPosition = initialCardOffset;
                cardRect.GetComponent<RectTransform>().DOAnchorPosY(0f, Constants.cardDrawDuration).SetEase(Ease.OutExpo);

                if (SoundManager.Instance)
                    SoundManager.Instance.Play(Sounds.cardDraw);

                yield return cardDrawDelay;
            }

            OnComplete?.Invoke();
            PreSortLists();
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
        #endregion

        #region CARD-THEMES
        /// <summary>
        /// Changes all cards' theme (color)
        /// </summary>
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

        public void PreSortLists()
        {
            List<CardData> listOfCards = new List<CardData>();
            foreach (Card card in currentCards)
                listOfCards.Add(card.cardData);

            oneTwoThreeSortResult = OneTwoThreeSort(listOfCards);
            tripleSevenSortResult = TripleSevenSort(listOfCards);
            smartSortResult = SmartSort(listOfCards);
        }

        /// <summary>
        /// Sorts by consecutive values and same card suits
        /// </summary>
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
            spades = spades.OrderBy(cardData => cardData.value).ToList();
            diamonds = diamonds.OrderBy(cardData => cardData.value).ToList();
            hearts = hearts.OrderBy(cardData => cardData.value).ToList();
            clubs = clubs.OrderBy(cardData => cardData.value).ToList();

            Tuple<List<List<CardData>>, List<CardData>> resultTuple;
            List<List<CardData>> sortedLists = new List<List<CardData>>();
            List<CardData> currentConsecutiveList = new List<CardData>();

            for(int i = 0; i < Enum.GetNames(typeof(CardSuit)).Length; i++)
            {
                CardSuit cardSuit = (CardSuit)i;
            
                switch(cardSuit)
                {
                    case CardSuit.spades:
                        currentConsecutiveList = spades;
                        break;
                    case CardSuit.diamonds:
                        currentConsecutiveList = diamonds;
                        break;
                    case CardSuit.hearts:
                        currentConsecutiveList = hearts;
                        break;
                    case CardSuit.clubs:
                        currentConsecutiveList = clubs;
                        break;
                }

                resultTuple = Helpers.FindConsecutiveCards(currentConsecutiveList, Constants.oneTwoThreeSortMinAmount);

                if (resultTuple.Item1.Count > 0)
                    foreach (List<CardData> sortedList in resultTuple.Item1)
                        sortedLists.Add(sortedList);

                // add the leftovers
                leftovers.AddRange(resultTuple.Item2);
            }

            return Tuple.Create(sortedLists, leftovers);
        }

        /// <summary>
        /// Sorts by same values but different card suits
        /// </summary>
        public static Tuple<List<List<CardData>>, List<CardData>> TripleSevenSort(List<CardData> listOfCards)
        {
            List<List<CardData>> sortedResultGroups = new List<List<CardData>>();
            List<CardData> leftovers = new List<CardData>();

            List<CardData> tempList = listOfCards;
            List<CardData> nonNumberedCards = tempList.Where(card => card.cardType != CardType.numbered && card.cardType != CardType.ace).ToList();

            foreach (CardData card in nonNumberedCards)
                leftovers.Add(card);

            tempList = tempList.Except(nonNumberedCards).ToList();

            // group same numbers to their respective groups
            IEnumerable<List<CardData>> sameNumberCardGroups = tempList.GroupBy(cardData => cardData.value).Select(cardDataGroup => cardDataGroup.ToList()).ToList();
            List<CardData> sortedGroup = new List<CardData>();

            foreach (List<CardData> group in sameNumberCardGroups)
            {     
                sortedGroup = group.GroupBy(cardData => cardData.cardSuit)
                                            .Select(cardSuit => cardSuit.First())
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

        /// <summary>
        /// Sorts with both algorithms while assuring the sum of the leftovers are minimum
        /// </summary>
        public static Tuple<List<List<CardData>>, List<CardData>> SmartSort(List<CardData> listOfCards)
        {
            List<List<CardData>> sortedResultGroups = new List<List<CardData>>();
            List<List<CardData>> allSortedGroups = new List<List<CardData>>();
            List<CardData> leftovers = new List<CardData>();

            // First do a 1-2-3 sort, and 7-7-7 for it's leftovers for maximum possible combinations
            Tuple<List<List<CardData>>, List<CardData>> oneTwoThreeSort = OneTwoThreeSort(listOfCards);
            Tuple<List<List<CardData>>, List<CardData>> extraTripleSevenTrios = TripleSevenSort(oneTwoThreeSort.Item2);

            // check the possible combinations of consecutive groups with larger values
            IEnumerable<IEnumerable<CardData>> oneTwoThreeCombinations = null;
            foreach (List<CardData> cardGroup in oneTwoThreeSort.Item1)
            {
                if (cardGroup.Count > 3)
                    oneTwoThreeCombinations = cardGroup.GetOrderedSubEnumerables();

                if (oneTwoThreeCombinations != null)
                    allSortedGroups.AddRange(oneTwoThreeCombinations.Select(i => i.ToList())
                                                                    .Where(list => list.Count >= 3).ToList());
            }

            // Then do a 7-7-7 sort, and 1-2-3 for it's leftovers
            Tuple<List<List<CardData>>, List<CardData>> tripleSevenSort = TripleSevenSort(listOfCards);
            Tuple<List<List<CardData>>, List<CardData>> extraOneTwoThreeForTrios = OneTwoThreeSort(tripleSevenSort.Item2);

            // check the possible combinations of groups with larger values
            IEnumerable<IEnumerable<CardData>> tripleSevenCombinations = null;
            foreach (List<CardData> cardGroup in tripleSevenSort.Item1)
            {
                if (cardGroup.Count > 3)
                    tripleSevenCombinations = cardGroup.DifferentCombinations(3);
            
                if(tripleSevenCombinations != null)
                    allSortedGroups.AddRange(tripleSevenCombinations.Select(i => i.ToList()).ToList());
            }

            allSortedGroups.AddRange(oneTwoThreeSort.Item1);
            allSortedGroups.AddRange(extraTripleSevenTrios.Item1);
            allSortedGroups.AddRange(tripleSevenSort.Item1);
            allSortedGroups.AddRange(extraOneTwoThreeForTrios.Item1);

            int minSum = Int32.MaxValue;
            List<List<CardData>> distinctGroupsList = new List<List<CardData>>();
            List<CardData> tempLeftoverList = new List<CardData>();

            for (int i = 0; i < allSortedGroups.Count; i++)
            {
                // clear temp lists
                if(distinctGroupsList.Count > 0)
                    distinctGroupsList.Clear();
                if (tempLeftoverList.Count > 0)
                    tempLeftoverList.Clear();

                // add the first value to check for its distinct groups
                distinctGroupsList.Add(allSortedGroups[i]);

                // look for any intersections between the first value and all other groups
                for(int j = 0; j < allSortedGroups.Count; j++)
                {
                    bool isDistinct = true;

                    foreach (List<CardData> distinctGroup in distinctGroupsList)
                    {
                        if (allSortedGroups[j].Intersect(distinctGroup).Count() > 0)
                            isDistinct = false;
                    }

                    if(isDistinct)
                        distinctGroupsList.Add(allSortedGroups[j]);
                }

                // get the leftovers of the current group combination
                tempLeftoverList = listOfCards;
                for (int a = 0; a < distinctGroupsList.Count; a++)
                    tempLeftoverList = tempLeftoverList.Except(distinctGroupsList[a]).ToList();

                // calculate the sum
                int sum = 0;
                for (int a = 0; a < tempLeftoverList.Count; a++)
                    sum += tempLeftoverList[a].value;

                // overwrite prev minimum if leftovers' sum is less
                if (sum < minSum)
                {
                    minSum = sum;

                    sortedResultGroups.Clear();
                    leftovers.Clear();

                    foreach (List<CardData> cardDatas in distinctGroupsList)
                        sortedResultGroups.Add(cardDatas);
                    foreach (CardData cardData in tempLeftoverList)
                        leftovers.Add(cardData);
                }
            }

            if(sortedResultGroups.Count == 0)
            {
                leftovers.Clear();
                leftovers = listOfCards;
            }

            return Tuple.Create(sortedResultGroups, leftovers);
        }

        #endregion
    }

    public static class Helpers
    {
        /// <summary>
        /// Generates random positive unique integers
        /// </summary>
        /// <param name="amount"> amount of integers </param>
        /// <param name="maxValue"> Max Value </param>
        internal static List<int> GenerateRandomUniqueIntegers(int amount, int maxValue)
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

        /// <summary>
        /// Finds the cards with consecutive values in a card list sorted by value
        /// </summary>
        internal static Tuple<List<List<CardData>>, List<CardData>> FindConsecutiveCards(List<CardData> sortedCardList, int minAmount)
        {
            List<List<CardData>> consecutiveLists = sortedCardList.Select((item, idx) => new { I = item, G = item.value - idx })
                                                              .Distinct()
                                                              .GroupBy(ig => ig.G,
                                                                        ig => ig.I,
                                                                        (k, g) => g.ToList()).ToList();
            List<List<CardData>> resultLists = new List<List<CardData>>();
            List<CardData> leftovers = new List<CardData>();

            for (int i = 0; i < consecutiveLists.Count; i++)
            {
                if (consecutiveLists[i].Count >= minAmount)
                {
                    resultLists.Add(consecutiveLists[i]);
                }
                else
                    leftovers.AddRange(consecutiveLists[i]);
            }

            return Tuple.Create(resultLists, leftovers);
        }

        /// <summary>
        /// Helper function to get all possible amount combinations of an IEnumerable
        /// </summary>
        /// <param name="k"> minimum amount for the combinations </param>
        internal static IEnumerable<IEnumerable<T>> DifferentCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).DifferentCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        /// <summary>
        /// Returns the ordered sub enumerables of an IEnumerable
        /// </summary>
        internal static IEnumerable<IEnumerable<T>> GetOrderedSubEnumerables<T>(this IEnumerable<T> collection)
        {
            var builder = new List<T>();
            foreach (var element in collection)
            {
                builder.Add(element);
                yield return builder;
            }
        }
    }
}

