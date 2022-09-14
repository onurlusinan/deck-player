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

        WaitForSeconds sortDelay;
        WaitForSeconds sortedGroupsDelay;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            cardSlots = new List<RectTransform>();

            PrepareDeck();
            sortDelay = new WaitForSeconds(sortCardDelay);
            sortedGroupsDelay = new WaitForSeconds(sortCardDelay * 2);
        }

        private void Update()
        {
            if(draggingCard && !settingCard)
                ArrangeDeckPositions();
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

        public IEnumerator SortCardsFromList(Tuple<List<List<Card>>, List<Card>> listsTuple)
        {
            for(int i = 0; i < cardSlots.Count; i++)
            {
                RectTransform card = cardSlots[i].GetComponent<CardSlot>().currentCard.GetComponent<RectTransform>();
                card.DOAnchorPosY(card.anchoredPosition.y + 300f, 0.25f);
            }

            yield return null;

            int slottedCards = 0;

            foreach(List<Card> list in listsTuple.Item1)
            {
                for(int i = 0; i < list.Count; i++)
                {
                    SetCardToSlot(list[i], cardSlots[slottedCards].GetComponent<CardSlot>(), 0.25f, true);
                    slottedCards++;
                    yield return sortDelay;
                }
                //yield return sortedGroupsDelay;
            }

            for(int i = 0; i < listsTuple.Item2.Count; i++)
            {
                SetCardToSlot(listsTuple.Item2[i], cardSlots[i + slottedCards].GetComponent<CardSlot>(), 0.25f, true);
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

                // card slot's X value
                slotCurrent = cardSlots[i].GetComponent<CardSlot>();
                RectTransform slotCurrentRect = slotCurrent.GetComponent<RectTransform>();

                // get world corners of the slot rect
                Vector3[] slotWorldCorners = new Vector3[4];
                slotCurrentRect.GetWorldCorners(slotWorldCorners);
                float leftBorder = slotWorldCorners[1].x;
                float rightBorder = slotWorldCorners[2].x;

                if (leftBorder < draggedCardX && draggedCardX < rightBorder) // between borders of a slot
                {
                    slotCurrent = cardSlots[i].GetComponent<CardSlot>();

                    if (i > 0)
                        slotPrev = cardSlots[i - 1].GetComponent<CardSlot>();
                    if (i < cardSlots.Count - 1)
                        slotNext = cardSlots[i + 1].GetComponent<CardSlot>();

                    if (!slotPrev?.currentCard && slotCurrent.currentCard)
                    {
                        SetCardToSlot(slotCurrent.currentCard, slotPrev, 0.2f);
                        slotCurrent.currentCard = null;
                        draggedCard.targetCardSlot = slotCurrent;
                        return;
                    }
                    else if (!slotNext?.currentCard && slotCurrent.currentCard)
                    {
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
