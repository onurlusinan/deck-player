using System.Collections.Generic;
using UnityEngine;

using DeckPlayer.CardSystem;

public class TestManager : MonoBehaviour
{
    public static TestManager Instance;

    [Header("Testing Config")]
    public bool isTesting = false;
    public List<CardData> testInputCardDatas;
    public List<CardData> oneTwoThreeExpectedResult;
    public List<CardData> tripleSevenExpectedResult;
    public List<CardData> smartSortExpectedResult;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (!isTesting)
            Destroy(gameObject);
    }
}
