using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuldingCardsPanel : MonoBehaviour
{
    [SerializeField] private Transform unlockedCardsContainer;
    [field: SerializeField, ReadOnly] public List<CardUpgrade> unlockedCards { get; private set; }

    [SerializeField] private Transform lockedCardsContainer;
    [field: SerializeField, ReadOnly] public List<CardUpgrade> lockedCards { get; private set; }


    public void AddCards(CardUpgrade card, BuildingDataSO dataSO)
    {
        lockedCards.Add(card);
        card.transform.parent = lockedCardsContainer;
    }
    public void AddCards(CardUpgrade card, UnitProduceStatsSO dataSO)
    {
        lockedCards.Add(card);
        card.transform.parent = lockedCardsContainer;
    }

}
