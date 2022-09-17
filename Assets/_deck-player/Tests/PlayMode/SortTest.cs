using System.Collections;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using DeckPlayer.CardSystem;
using System.Collections.Generic;

namespace DeckPlayer.Tests
{ 
    public class SortTests
    {
        private List<CardData> CreateListOfCards()
        {
            List<CardData> result = new List<CardData>();

            // spades
            CardData spades1 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades1.cardSuit = CardSuit.spades;
            spades1.cardType = CardType.ace;
            spades1.value = 1;

            CardData spades2 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades2.cardSuit = CardSuit.spades;
            spades2.cardType = CardType.numbered;
            spades2.value = 2;

            CardData spades3 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades3.cardSuit = CardSuit.spades;
            spades3.cardType = CardType.numbered;
            spades3.value = 3;

            CardData spades4 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades4.cardSuit = CardSuit.spades;
            spades4.cardType = CardType.numbered;
            spades4.value = 4;

            // diamonds
            CardData diamonds3 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds3.cardSuit = CardSuit.diamonds;
            diamonds3.cardType = CardType.numbered;
            diamonds3.value = 3;

            CardData diamonds4 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds4.cardSuit = CardSuit.diamonds;
            diamonds4.cardType = CardType.numbered;
            diamonds4.value = 4;

            CardData diamonds5 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds5.cardSuit = CardSuit.diamonds;
            diamonds5.cardType = CardType.numbered;
            diamonds5.value = 5;

            // leftovers
            CardData diamonds1 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds1.cardSuit = CardSuit.diamonds;
            diamonds1.cardType = CardType.ace;
            diamonds1.value = 1;

            CardData hearts1 = ScriptableObject.CreateInstance("CardData") as CardData;
            hearts1.cardSuit = CardSuit.hearts;
            hearts1.cardType = CardType.ace;
            hearts1.value = 1;

            CardData hearts4 = ScriptableObject.CreateInstance("CardData") as CardData;
            hearts4.cardSuit = CardSuit.hearts;
            hearts4.cardType = CardType.numbered;
            hearts4.value = 4;

            CardData clubs4 = ScriptableObject.CreateInstance("CardData") as CardData;
            clubs4.cardSuit = CardSuit.clubs;
            clubs4.cardType = CardType.numbered;
            clubs4.value = 4;

            result.Add(spades1);
            result.Add(spades2);
            result.Add(spades3);
            result.Add(spades4);
            result.Add(diamonds1);
            result.Add(diamonds3);
            result.Add(diamonds4);
            result.Add(diamonds5);
            result.Add(hearts1);
            result.Add(hearts4);
            result.Add(clubs4);

            return result;
        }

        [UnityTest]
        public IEnumerator OneTwoThreeSort()
        {
            List<CardData> listToSort = new List<CardData>();
            List<List<CardData>> expectedValueGroups = new List<List<CardData>>();
            List<CardData> expectedLeftovers = new List<CardData>();

            #region create expected results
            // spades
            CardData spades1 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades1.Init(1, CardSuit.spades, CardType.ace);
            listToSort.Add(spades1);

            CardData spades2 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades2.Init(2, CardSuit.spades, CardType.numbered);
            listToSort.Add(spades2);

            CardData spades3 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades3.Init(3, CardSuit.spades, CardType.numbered);
            listToSort.Add(spades3);

            CardData spades4 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades4.Init(4, CardSuit.spades, CardType.numbered);
            listToSort.Add(spades4);

            // diamonds
            CardData diamonds3 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds3.Init(3, CardSuit.diamonds, CardType.numbered);
            listToSort.Add(diamonds3);

            CardData diamonds4 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds4.Init(4, CardSuit.diamonds, CardType.numbered);
            listToSort.Add(diamonds4);

            CardData diamonds5 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds5.Init(5, CardSuit.diamonds, CardType.numbered);
            listToSort.Add(diamonds5);

            // leftovers
            CardData diamonds1 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds1.Init(1, CardSuit.diamonds, CardType.ace);
            listToSort.Add(diamonds1);

            CardData hearts1 = ScriptableObject.CreateInstance("CardData") as CardData;
            hearts1.Init(1, CardSuit.hearts, CardType.ace);
            listToSort.Add(hearts1);

            CardData hearts4 = ScriptableObject.CreateInstance("CardData") as CardData;
            hearts4.Init(4, CardSuit.hearts, CardType.numbered);
            listToSort.Add(hearts4);

            CardData clubs4 = ScriptableObject.CreateInstance("CardData") as CardData;
            clubs4.Init(4, CardSuit.clubs, CardType.numbered);
            listToSort.Add(clubs4);

            // creating Expected Value Groups to compare
            List<CardData> spadesList = new List<CardData>
            {
                spades1,
                spades2,
                spades3,
                spades4
            };
            expectedValueGroups.Add(spadesList);

            List<CardData> diamondsList = new List<CardData>
            {
                diamonds3,
                diamonds4,
                diamonds5
            };
            expectedValueGroups.Add(diamondsList);

            expectedLeftovers.Add(diamonds1);
            expectedLeftovers.Add(hearts1);
            expectedLeftovers.Add(hearts4);
            expectedLeftovers.Add(clubs4);
            #endregion

            // use the algorithm
            Tuple<List<List<CardData>>, List<CardData>> oneTwoThreeSort = CardManager.OneTwoThreeSort(listToSort);

            CollectionAssert.AreEqual(expectedValueGroups, oneTwoThreeSort.Item1);
            CollectionAssert.AreEqual(expectedLeftovers, oneTwoThreeSort.Item2);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TripleSevenSort()
        {
            List<CardData> listToSort = new List<CardData>();
            List<List<CardData>> expectedValueGroups = new List<List<CardData>>();
            List<CardData> expectedLeftovers = new List<CardData>();

            #region create expected results
            
            // aces
            CardData hearts1 = ScriptableObject.CreateInstance("CardData") as CardData;
            hearts1.Init(1, CardSuit.hearts, CardType.ace);
            listToSort.Add(hearts1);

            CardData spades1 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades1.Init(1, CardSuit.spades, CardType.ace);
            listToSort.Add(spades1);

            CardData diamonds1 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds1.Init(1, CardSuit.diamonds, CardType.ace);
            listToSort.Add(diamonds1);

            // cards with value = 4
            CardData hearts4 = ScriptableObject.CreateInstance("CardData") as CardData;
            hearts4.Init(4, CardSuit.hearts, CardType.numbered);
            listToSort.Add(hearts4);

            CardData clubs4 = ScriptableObject.CreateInstance("CardData") as CardData;
            clubs4.Init(4, CardSuit.clubs, CardType.numbered);
            listToSort.Add(clubs4);

            CardData spades4 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades4.Init(4, CardSuit.spades, CardType.numbered);
            listToSort.Add(spades4);

            CardData diamonds4 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds4.Init(4, CardSuit.diamonds, CardType.numbered);
            listToSort.Add(diamonds4);

            // leftovers
            CardData spades2 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades2.Init(2, CardSuit.spades, CardType.numbered);
            listToSort.Add(spades2);

            CardData diamonds5 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds5.Init(5, CardSuit.diamonds, CardType.numbered);
            listToSort.Add(diamonds5);

            CardData diamonds3 = ScriptableObject.CreateInstance("CardData") as CardData;
            diamonds3.Init(3, CardSuit.diamonds, CardType.numbered);
            listToSort.Add(diamonds3);

            CardData spades3 = ScriptableObject.CreateInstance("CardData") as CardData;
            spades3.Init(3, CardSuit.spades, CardType.numbered);
            listToSort.Add(spades3);

            // creating Expected Value Groups to compare
            List<CardData> acesList = new List<CardData>
            {
                hearts1,
                spades1,
                diamonds1
            };
            expectedValueGroups.Add(acesList);

            List<CardData> value4List = new List<CardData>
            {
                hearts4,
                clubs4,
                spades4,
                diamonds4
            };
            expectedValueGroups.Add(value4List);

            expectedLeftovers.Add(spades2);
            expectedLeftovers.Add(diamonds5);
            expectedLeftovers.Add(diamonds3);
            expectedLeftovers.Add(spades3);
            #endregion

            // use the algorithm
            Tuple<List<List<CardData>>, List<CardData>> tripleSevenSort = CardManager.TripleSevenSort(listToSort);

            CollectionAssert.AreEqual(expectedValueGroups, tripleSevenSort.Item1);
            CollectionAssert.AreEqual(expectedLeftovers, tripleSevenSort.Item2);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SmartSort()
        {
           

            yield return null;
        }
    }

}

