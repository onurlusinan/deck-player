using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;
using DG.Tweening;
using System;

namespace DeckPlayer.Managers
{ 
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance;

        public RectTransform cardDeck;
        public CardSlot lastVisitedSlot = null;

        [HideInInspector]
        public bool draggingCard = false;
        [HideInInspector]
        public Card draggedCard;

        private List<RectTransform> cardSlots;

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
            if(draggingCard)
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

        public void SetCardToSlot(Card card, CardSlot cardSlot, float duration)
        {
            if (cardSlot.currentCard)
                return;

            RectTransform cardRect = card.GetComponent<RectTransform>();
            RectTransform slotRect = cardSlot.GetComponent<RectTransform>();

            cardRect.SetParent(slotRect, false);
            cardRect.DOAnchorPos(Vector3.zero, duration);
            cardRect.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);

            card.currentSlot = cardSlot;
            cardSlot.currentCard = card;
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
                float slotCurrentX = slotCurrentRect.position.x;

                Vector3[] slotWorldCorners = new Vector3[4];
                slotCurrentRect.GetWorldCorners(slotWorldCorners);
                float leftBorder = slotWorldCorners[1].x;
                float rightBorder = slotWorldCorners[2].x;

                Debug.Log("CardSlotIndex: " + i + " => Borders: " + leftBorder + "/" + rightBorder);

                if (leftBorder < draggedCardX && draggedCardX < rightBorder) // between borders of a slot
                {
                    Debug.Log("Visiting slot " + i);

                    
                }
            }


            #region prev-algo

            /************************************************************/

            // move cardSlots[i] to a slot that's current card is null
            //slotPrev = cardSlots[i-1].GetComponent<CardSlot>();
            //slotNext = cardSlots[i+1].GetComponent<CardSlot>();

            //if (!slotNext.currentCard)
            //    SetCardToSlot(slotCurrent.currentCard, slotNext, 0.75f);
            //else if(!slotPrev.currentCard)
            //    SetCardToSlot(slotCurrent.currentCard, slotPrev, 0.75f);

            //draggedCard.targetCardSlot = slotCurrent;

            /************************************************************/

            //if (lastVisitedSlotIndex > i && !slotNext.currentCard)
            //{
            //    SetCardToSlot(cardPrev, slotNext);

            //    draggedCard.targetCardSlot = slotPrev;
            //    lastVisitedSlot = slotPrev;
            //}
            //else if (lastVisitedSlotIndex < i && !slotPrev.currentCard)
            //{
            //    SetCardToSlot(cardNext, slotPrev);

            //    draggedCard.targetCardSlot = slotNext;
            //    lastVisitedSlot = slotNext;
            //}

            //lastVisitedSlotIndex = i;

            #endregion
        }
    }

}
