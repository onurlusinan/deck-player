using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using DeckPlayer.Managers;

namespace DeckPlayer.CardSystem
{
    public enum CardSuit
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

    public enum CardTheme
    {
        white,
        magenta,
        green,
        cyan
    }

    public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Card Info:")]
        [SerializeField] private CardSuit _suit;
        [SerializeField] private CardType _type;
        [SerializeField] private int _value;

        [Header("Card-Display Configuration")]
        public TextMeshProUGUI valueText;
        public Image iconImage;
        public Image cardImage;
        public Image backgroundImage;

        public CardSuit GetSuit() => _suit;
        public CardType GetCardType() => _type;
        public int GetValue() => _value;

        // for drag & drop
        private RectTransform cardRect;
        private Canvas gameCanvas;

        public CardSlot currentSlot;
        public CardSlot targetCardSlot;

        private void Awake()
        {
            cardRect = GetComponent<RectTransform>();
            gameCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            targetCardSlot = currentSlot;
        }

        /// <summary>
        /// Fills the card UI based on the CardData
        /// </summary>
        public void InitCard(CardData cardData)
        {
            _suit = cardData.cardSuit;
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

        public void ChangeTheme(CardTheme theme)
        {
            switch(theme)
            {
                case CardTheme.white:
                    backgroundImage.DOColor(Color.white, 0.2f);
                    break;
                case CardTheme.magenta:
                    backgroundImage.DOColor(Color.magenta, 0.2f);
                    break;
                case CardTheme.green:
                    backgroundImage.DOColor(Color.green, 0.2f);
                    break;
                case CardTheme.cyan:
                    backgroundImage.DOColor(Color.cyan, 0.2f);
                    break;
            }
        }

        #region drag-drop
        public void OnDrag(PointerEventData eventData)
        {
            if (!GameManager.Instance.deckInput) 
                return;

            cardRect.anchoredPosition += eventData.delta / gameCanvas.scaleFactor;
            cardRect.DORotateQuaternion(Quaternion.identity, 0.2f);

            DeckManager.Instance.draggingCard = true;
            DeckManager.Instance.draggedCard = this;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!GameManager.Instance.deckInput) 
                return;

            cardRect.DOScale(1.3f, 0.2f);

            // in order to appear in the very front
            transform.SetParent(DeckManager.Instance.cardDeck);
            transform.SetAsLastSibling();

            currentSlot.currentCard = null;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DeckManager.Instance.draggingCard = false;

            cardRect.DOScale(1.0f, 0.2f);

            if (targetCardSlot == null)
                targetCardSlot = currentSlot;

            DeckManager.Instance.SetCardToSlot(this, targetCardSlot, 0.2f);
            DeckManager.Instance.draggedCard = null;
        }
        #endregion
    }
}
