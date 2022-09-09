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
    public CardType cardType;
    public Image icon;
    public Image image;
}
