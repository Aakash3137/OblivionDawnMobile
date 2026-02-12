using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] private Transform unlockedCardsContainer;
    [field: SerializeField, ReadOnly] public List<CardUpgrade> unlockedCards { get; private set; }
    public bool initializedUnits => unlockedCards.Count > 0;

    [SerializeField] private Transform lockedCardsContainer;
    [field: SerializeField, ReadOnly] public List<CardUpgrade> lockedCards { get; private set; }
    public bool initializedBuildings => lockedCards.Count > 0;


    public void AddCards(CardUpgrade card, BuildingDataSO dataSO)
    {
        card = Instantiate(card, lockedCardsContainer);
        lockedCards.Add(card);
        card.buildingUpgradeData = dataSO;
    }

    public void AddCards(CardUpgrade card, UnitProduceStatsSO dataSO)
    {
        card = Instantiate(card, lockedCardsContainer);
        lockedCards.Add(card);
        card.unitUpgradeData = dataSO;
    }
}
