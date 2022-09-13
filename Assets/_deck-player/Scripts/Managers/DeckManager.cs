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
        public float sortCardDelay = 0.1f;

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

            for(int i = 0; i < cardSlots.Count; i++)
            {
                RectTransform card = cardSlots[i].GetComponent<CardSlot>().currentCard.GetComponent<RectTransform>();
                card.DOAnchorPosY(card.anchoredPosition.y + 300f, 0.25f);
            }

            yield return null;

            for(int i = 0; i < sortedList.Count; i++)
            {
                SetCardToSlot(sortedList[i], cardSlots[i].GetComponent<CardSlot>(), 0.25f, true);
                yield return sortDelay;
            }
                
            GameManager.Instance.EnableInput(true);
        }

        private void MoveCardToSlot(Card card, CardSlot cardSlot, float duration)
        {
            Transform cardTansform = card.transform;

            cardTansform.SetParent(cardSlot.transform, true);
            cardTansform.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);

            cardTansform.DOMove(cardSlot.transform.position, duration).OnComplete(() =>
            {
                cardTansform.localPosition = Vector3.zero;
            });   
        }

        public void SetCardToSlot(Card card, CardSlot cardSlot, float duration, bool isSortMode = false)
        {
            if (cardSlot.currentCard && !isSortMode)
                return;

            settingCard = true;

            card.currentSlot = cardSlot;
            card.targetCardSlot = null;
            cardSlot.currentCard = card;

            MoveCardToSlot(card, cardSlot, duration);

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

                    if (i > 0)
                        slotPrev = cardSlots[i - 1].GetComponent<CardSlot>();
                    if (i < cardSlots.Count - 1)
                        slotNext = cardSlots[i + 1].GetComponent<CardSlot>();

                    if (!slotPrev?.currentCard && slotCurrent.currentCard)
                    {
                        Debug.Log("HOOP: 1");
                        SetCardToSlot(slotCurrent.currentCard, slotPrev, 0.2f);
                        slotCurrent.currentCard = null;
                        draggedCard.targetCardSlot = slotCurrent;
                        return;
                    }
                    else if (!slotNext?.currentCard && slotCurrent.currentCard)
                    {
                        Debug.Log("HOOP: 2");
                        SetCardToSlot(slotCurrent.currentCard, slotNext, 0.2f);
                        slotCurrent.currentCard = null;
                        draggedCard.targetCardSlot = slotCurrent;
                        return;
                    }
                    else
                        return;
                }
            }
        }
    }
}
