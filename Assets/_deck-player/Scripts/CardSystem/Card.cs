using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace DeckPlayer.CardSystem
{
    public enum CardType
    {
        spades,
        diamonds,
        hearts,
        clubs
    }

    public class Card : MonoBehaviour
    {
        private CardType _type;

        [Header("Card-Display Configuration")]
        public TextMeshProUGUI value;
        public Image icon;
        public Image image;

        /// <summary>
        /// Fills the card UI based on the CardData
        /// </summary>
        public void FillCard(CardData cardData)
        {
            _type = cardData.cardType;

            this.value.SetText(cardData.value.ToString());
            this.icon = cardData.icon;
            this.image = cardData.image;
        }
    }
}
