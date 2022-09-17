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
        private List<CardData> ConvertResultToList(Tuple<List<List<CardData>>, List<CardData>> result)
        {
            List<CardData> resultList = new List<CardData>();   

            foreach(List<CardData> cardList in result.Item1)
            {
                foreach(CardData cardData in cardList)
                    resultList.Add(cardData);
            }

            foreach (CardData cardData in result.Item2)
                resultList.Add(cardData);

            return resultList;
        }

        [UnityTest]
        public IEnumerator OneTwoThreeSort()
        {
            Tuple<List<List<CardData>>, List<CardData>> oneTwoThreeSort = CardManager.Instance.OneTwoThreeSort();
            List<CardData> resultingList = ConvertResultToList(oneTwoThreeSort);

            yield return null;

            Assert.AreEqual(TestManager.Instance.oneTwoThreeExpectedResult, resultingList);
        }
    }

}

