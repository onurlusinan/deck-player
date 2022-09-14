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

    public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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

        private void ClampToRect(RectTransform clampingParent)
        {
            Vector2 pos = cardRect.localPosition;

            Vector2 minPosition = clampingParent.rect.min - cardRect.rect.min;
            Vector2 maxPosition = clampingParent.rect.max - cardRect.rect.max;

            pos.x = Mathf.Clamp(cardRect.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(cardRect.localPosition.y, minPosition.y, maxPosition.y);

            cardRect.localPosition = pos;
        }
        #endregion
    }
}
