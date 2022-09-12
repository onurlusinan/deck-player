using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;
using DG.Tweening;
using System;
using System.Collections;

namespace DeckPlayer.Managers
{ 
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance;

        public RectTransform cardDeck;
        public float sortCardDelay = 0.25f;

        [HideInInspector]
        public bool draggingCard = false;
        [HideInInspector]
        public Card draggedCard;

        private List<RectTransform> cardSlots;

        private bool settingCard = false;
        private float draggedCardX;
        private CardSlot slotCurrent;
        private CardSlot slotPrev;
        private CardSlot slotNext;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            cardSlots = new List<RectTransform>();

            PrepareDeck();
        }

        private void Update()
        {
            if(draggingCard && !settingCard)
            {
                ArrangeDeckPositions();
            }
        }

        public void PrepareDeck()
        {
            foreach (RectTransform cardSlot in cardDeck)
                cardSlots.Add(cardSlot);
        }

        public Transform GetCardSlot(int index)
        {
            return cardSlots[index];
        }

        public IEnumerator SortCardsFromList(List<Card> sortedList)
        {
            WaitForSeconds sortDelay = new WaitForSeconds(sortCardDelay);

            for(int i = 0; i < sortedList.Count; i++)
            {
                SetCardToSlot(sortedList[i], cardSlots[i].GetComponent<CardSlot>(), 0.25f);
                yield return sortDelay;
            }
                
            GameManager.Instance.EnableInput(true);
        }

        public void SetCardToSlot(Card card, CardSlot cardSlot, float duration)
        {
            if (cardSlot.currentCard)
                return;

            settingCard = true;

            RectTransform cardRect = card.GetComponent<RectTransform>();
            RectTransform slotRect = cardSlot.GetComponent<RectTransform>();

            card.currentSlot = cardSlot;
            card.targetCardSlot = null;
            cardSlot.currentCard = card;

            cardRect.SetParent(slotRect, false);
            cardRect.DOAnchorPos(Vector3.zero, duration);
            cardRect.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);

            settingCard = false;
        }

        private void ArrangeDeckPositions()
        {
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (i + 1 > cardSlots.Count)
                    return;

                // card's anchored X value
                draggedCardX = draggedCard.GetComponent<RectTransform>().position.x;
                Debug.Log("dragged Card X: " + draggedCardX);

                // card slot's X value
                slotCurrent = cardSlots[i].GetComponent<CardSlot>();
                RectTransform slotCurrentRect = slotCurrent.GetComponent<RectTransform>();

                Vector3[] slotWorldCorners = new Vector3[4];
                slotCurrentRect.GetWorldCorners(slotWorldCorners);
                float leftBorder = slotWorldCorners[1].x;
                float rightBorder = slotWorldCorners[2].x;

                Debug.Log("CardSlotIndex: " + i + " => Borders: " + leftBorder + "/" + rightBorder);

                if (leftBorder < draggedCardX && draggedCardX < rightBorder) // between borders of a slot
                {
                    Debug.Log("Visiting slot " + i);

                    slotCurrent = cardSlots[i].GetComponent<CardSlot>();

                    slotPrev = cardSlots[i-1]?.GetComponent<CardSlot>();
                    slotNext = cardSlots[i+1]?.GetComponent<CardSlot>();

                    if (!slotNext?.currentCard && slotCurrent.currentCard)
                    {
                        SetCardToSlot(slotCurrent.currentCard, slotNext, 0.2f);
                    }

                    if (!slotPrev?.currentCard && slotCurrent.currentCard)
                    {
                        SetCardToSlot(slotCurrent.currentCard, slotPrev, 0.2f);
                    }

                    slotCurrent.currentCard = null;
                    draggedCard.targetCardSlot = slotCurrent;
                }
            }
        }
    }

}
