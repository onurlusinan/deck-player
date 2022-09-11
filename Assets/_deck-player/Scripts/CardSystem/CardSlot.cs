using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace DeckPlayer.CardSystem
{
    public class CardSlot : MonoBehaviour
    {
        private RectTransform slotRect;

        internal float xPos;

        private void Awake()
        {
            slotRect = GetComponent<RectTransform>();
        }
    }
}

