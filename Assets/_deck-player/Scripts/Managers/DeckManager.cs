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

        public Transform cardDeck;

        [Header("Card-Dragging")]
        public bool draggingCard = false;
        public float draggedCardX = 0f;

        private List<Transform> cardSlots;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            cardSlots = new List<Transform>();

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
            foreach (Transform cardSlot in cardDeck)
                cardSlots.Add(cardSlot);
        }

        public Transform GetCardSlot(int index)
        {
            return cardSlots[index];
        }

        public void SetCardToSlot(Card card, CardSlot cardSlot)
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();
            RectTransform slotRect = cardSlot.GetComponent<RectTransform>();

            cardRect.SetParent(slotRect, false);
            cardRect.DOAnchorPos(Vector3.zero, 0.2f);
            cardRect.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);

            card.currentSlot = cardSlot;
        }

        private void ArrangeDeckPositions()
        {
            for(int i = 0; i < cardSlots.Count; i++)
            {
                if (i + 1 >= cardSlots.Count)
                    return;

                float slotXPrev = cardSlots[i].GetComponent<RectTransform>().anchoredPosition.x;
                float slotXNext = cardSlots[i+1].GetComponent<RectTransform>().anchoredPosition.x;

                if (draggedCardX > slotXPrev && draggedCardX < slotXNext)
                    Debug.Log("cardSlot " + (i+1));
            }
        }
    }

}
