using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace DeckPlayer.CardSystem
{
    public enum CardSymbol
    {
        spades,
        diamonds,
        hearts,
        clubs
    }

    public enum CardType
    {
        numbered,
        jack,
        king,
        queen
    }

    public class Card : MonoBehaviour
    {
        [Header("Card Info:")]
        [SerializeField] private CardSymbol _symbol;
        [SerializeField] private CardType _type;
        [SerializeField] private int _value;

        [Header("Card-Display Configuration")]
        public TextMeshProUGUI valueText;
        public Image iconImage;
        public Image cardImage;

        public CardSymbol GetSymbol() => _symbol;
        public int GetValue() => _value;

        /// <summary>
        /// Fills the card UI based on the CardData
        /// </summary>
        public void InitCard(CardData cardData)
        {
            _symbol = cardData.cardSymbol;
            _type = cardData.cardType;
            _value = cardData.value;

            this.valueText.SetText(GetCardText());
            this.iconImage.sprite = cardData.iconSprite;
            this.cardImage.sprite = cardData.imageSprite;
        }

        /// <summary>
        /// Computes and returns the card text
        /// </summary>
        private string GetCardText()
        {
            switch (_type)
            {
                case CardType.jack:
                    return "J";
                case CardType.king:
                    return "K";
                case CardType.queen:
                    return "Q";
                default:
                    if (_value != 1) 
                        return _value.ToString();
                    else 
                        return "A";
            }
        }
    }
}
