using UnityEngine;
using UnityEngine.UI;

using DeckPlayer.CardSystem;

/// <summary>
/// CardData objects hold the info of the sounds
/// </summary>
[CreateAssetMenu(fileName = "CardData", menuName = "ScriptableObjects/Cards/CardData")]
public class CardData : ScriptableObject
{
    public int value;
    public CardSuit cardSuit;
    public CardType cardType;
    public Sprite iconSprite;
    public Sprite imageSprite;

    public void Init(int value, CardSuit suit, CardType type)
    {
        this.value = value;
        this.cardSuit = suit;
        this.cardType = type;
    }
}
