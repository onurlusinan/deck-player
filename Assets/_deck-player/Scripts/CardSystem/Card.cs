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

    public class CardDisplay
    {
        public TextMeshProUGUI value;
        public Image icon;
        public Image image;
    }

    public class Card : MonoBehaviour
    {
        private CardType _type;

        [Header("Card-Display Configuration")]
        public CardDisplay display; 
        
        public void FillCard(CardData cardData)
        {
                
        }
    }
}
