using UnityEngine;

/// <summary>
/// CardDataCollection holds the CardData objects
/// </summary>
[CreateAssetMenu(fileName = "CardDataCollection", menuName = "ScriptableObjects/Cards/CardDataCollection")]
public class CardDataCollection : ScriptableObject
{
    [SerializeField]
    private CardData[] cardDatas;

    public CardData[] GetCollection()
    {
        return this.cardDatas;
    }
}
