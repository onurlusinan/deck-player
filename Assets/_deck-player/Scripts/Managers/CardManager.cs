using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;

public class CardManager : MonoBehaviour
{
    public List<Card> cards; // 11 cards
    private CardDataCollection collection; // 52 card datas

    private void Awake()
    {
        collection = Resources.Load<CardDataCollection>("CardData/CardDataCollection");
    }

    private void Start()
    {
        
    }

    private void CreateCard(CardData cardData)
    {
        
    }
}
