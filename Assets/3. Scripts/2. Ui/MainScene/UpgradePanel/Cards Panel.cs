using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] private Transform cardsContainer;

    [field: SerializeField, ReadOnly] public List<CardUpgradeData> unlockedCards { get; private set; }
    public bool initializedUnits => unlockedCards.Count > 0;
    [field: SerializeField, ReadOnly] public List<CardUpgradeData> lockedCards { get; private set; }
    public bool initializedBuildings => lockedCards.Count > 0;


    public void AddCards(CardUpgradeData card, BuildingDataSO dataSO)
    {
        card = Instantiate(card, cardsContainer);
        if (dataSO.buildingIdentity.isLocked)
            lockedCards.Add(card);
        else
            unlockedCards.Add(card);
        card.buildingUpgradeData = dataSO;
    }

    public void AddCards(CardUpgradeData card, UnitProduceStatsSO dataSO)
    {
        card = Instantiate(card, cardsContainer);
        if (dataSO.unitIdentity.isLocked)
            lockedCards.Add(card);
        else
            unlockedCards.Add(card);
        card.unitUpgradeData = dataSO;
    }
}
