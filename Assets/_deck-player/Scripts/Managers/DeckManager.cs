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

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            cardSlots = new List<RectTransform>();

            PrepareDeck();
            sortDelay = new WaitForSeconds(sortCardDelay);
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

            List<List<Card>> sortedGroups = listsTuple.Item1;
            int slottedCards = 0;

            if(sortedGroups.Count > 0) // sorted groups, if there are any
            {
                for (int i = 0; i < sortedGroups.Count; i++)
                {
                    for(int j = 0; j < sortedGroups[i].Count; j++)
                    {
                        Card card = sortedGroups[i][j];

                        SetCardToSlot(card, cardSlots[slottedCards].GetComponent<CardSlot>(), 0.25f, true, (i+1)/2f);
                        slottedCards++;

                        yield return sortDelay;
                    }
                }
            }

            List<Card> leftOvers = listsTuple.Item2;

            for (int i = 0; i < leftOvers.Count; i++) // leftovers
            {
                SetCardToSlot(leftOvers[i], cardSlots[i + slottedCards].GetComponent<CardSlot>(), 0.25f, true);
                yield return sortDelay;
            }
                
            GameManager.Instance.EnableInput(true);
        }

        public void SetCardToSlot(Card card, CardSlot cardSlot, float duration, bool isSortMode = false, float sortGroupHeight = 0)
        {
            if (cardSlot.currentCard && !isSortMode)
                return;

            settingCard = true;

            card.currentSlot = cardSlot;
            card.targetCardSlot = null;
            cardSlot.currentCard = card;

            card.transform.SetParent(cardSlot.transform, true);
            card.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);

            if (sortGroupHeight > 0f)
            {
                card.transform.DOMove(new Vector3(cardSlot.transform.position.x, sortGroupHeight, cardSlot.transform.position.z), duration);

                //Sequence groupCardSequence = DOTween.Sequence();
                //groupCardSequence.Append(card.transform.DOMove(cardSlot.transform.position, duration))
                //                 .Append(card.transform.DOMoveY(sortGroupHeight, 0.2f));
            }
            else
                card.transform.DOMove(cardSlot.transform.position, duration).OnComplete(() =>
                                card.transform.localPosition = Vector3.zero
                );

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
