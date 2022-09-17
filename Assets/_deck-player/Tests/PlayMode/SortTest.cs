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
        private List<Card> ConvertResultToList(Tuple<List<List<Card>>, List<Card>> result)
        {
            List<Card> resultList = new List<Card>();   

            foreach(List<Card> cardList in result.Item1)
            {
                foreach(Card card in cardList)
                    resultList.Add(card);
            }

            foreach (Card card in result.Item2)
                resultList.Add(card);

            return resultList;
        }

        [UnityTest]
        public IEnumerator OneTwoThreeSort()
        {
            Tuple<List<List<Card>>, List<Card>> oneTwoThreeSort = CardManager.Instance.OneTwoThreeSort(CardManager.Instance.currentCards);
            List<Card> resultingList = ConvertResultToList(oneTwoThreeSort);

            yield return null;

            Assert.AreEqual(TestManager.Instance.oneTwoThreeExpectedResult, resultingList);
        }
    }

}

