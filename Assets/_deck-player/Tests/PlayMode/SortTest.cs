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
        [UnityTest]
        public IEnumerator OneTwoThreeSort()
        {
            List<List<CardData>> expectedValueGroups = new List<List<CardData>>();
            List<CardData> expectedLeftovers = new List<CardData>();

            #region create expected results
            // spades
            CardData spades1 = new CardData();
            spades1.cardSuit = CardSuit.spades;
            spades1.cardType = CardType.ace;
            spades1.value = 1;

            CardData spades2 = new CardData();
            spades2.cardSuit = CardSuit.spades;
            spades2.cardType = CardType.numbered;
            spades2.value = 2;

            CardData spades3 = new CardData();
            spades3.cardSuit = CardSuit.spades;
            spades3.cardType = CardType.numbered;
            spades3.value = 3;

            CardData spades4 = new CardData();
            spades4.cardSuit = CardSuit.spades;
            spades4.cardType = CardType.numbered;
            spades4.value = 4;

            List<CardData> spadesList = new List<CardData>();
            spadesList.Add(spades1);
            spadesList.Add(spades2);
            spadesList.Add(spades3);
            spadesList.Add(spades4);
            expectedValueGroups.Add(spadesList);

            // diamonds
            CardData diamonds3 = new CardData();
            diamonds3.cardSuit = CardSuit.diamonds;
            diamonds3.cardType = CardType.ace;
            diamonds3.value = 3;

            CardData diamonds4 = new CardData();
            diamonds4.cardSuit = CardSuit.diamonds;
            diamonds4.cardType = CardType.numbered;
            diamonds4.value = 4;

            CardData diamonds5 = new CardData();
            diamonds5.cardSuit = CardSuit.diamonds;
            diamonds5.cardType = CardType.numbered;
            diamonds5.value = 5;

            List<CardData> diamondsList = new List<CardData>();
            diamondsList.Add(diamonds3);
            diamondsList.Add(diamonds4);
            diamondsList.Add(diamonds5);
            expectedValueGroups.Add(diamondsList);

            // leftovers
            CardData diamonds1 = new CardData();
            diamonds1.cardSuit = CardSuit.diamonds;
            diamonds1.cardType = CardType.ace;
            diamonds1.value = 1;

            CardData hearts1 = new CardData();
            hearts1.cardSuit = CardSuit.hearts;
            hearts1.cardType = CardType.ace;
            hearts1.value = 1;

            CardData hearts4 = new CardData();
            hearts4.cardSuit = CardSuit.hearts;
            hearts4.cardType = CardType.numbered;
            hearts4.value = 4;

            CardData clubs4 = new CardData();
            clubs4.cardSuit = CardSuit.clubs;
            clubs4.cardType = CardType.numbered;
            clubs4.value = 4;

            expectedLeftovers.Add(diamonds1);
            expectedLeftovers.Add(hearts1);
            expectedLeftovers.Add(hearts4);
            expectedLeftovers.Add(clubs4);
            #endregion

            Debug.Log("[TEST123] Created expected results.");
            yield return new WaitForSeconds(10f);

            // use the algorithm
            Tuple<List<List<CardData>>, List<CardData>> oneTwoThreeSort = CardManager.Instance.OneTwoThreeSort(TestManager.Instance.testInputCardDatas);
            Debug.Log("[TEST123] Sorted list.");
            yield return new WaitForSeconds(2f);
            Debug.Log("[TEST123] Testing expected results...");

            Tuple<List<List<CardData>>, List<CardData>> expectedResult = Tuple.Create(expectedValueGroups, expectedLeftovers);
            CollectionAssert.AreEquivalent(expectedResult.Item1, oneTwoThreeSort.Item1);
            CollectionAssert.AreEquivalent(expectedResult.Item2, oneTwoThreeSort.Item2);
        }
    }

}

