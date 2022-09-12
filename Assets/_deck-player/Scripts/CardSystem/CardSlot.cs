using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace DeckPlayer.CardSystem
{
    public class CardSlot : MonoBehaviour
    {
        private RectTransform slotRect;
        public Card currentCard;

        private void Awake()
        {
            slotRect = GetComponent<RectTransform>();
        }
    }
}

