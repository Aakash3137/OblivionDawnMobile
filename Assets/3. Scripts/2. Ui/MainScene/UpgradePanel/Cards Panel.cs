using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] private Transform cardsContainer;

    [field: SerializeField, ReadOnly, Space(20)] public List<UpgradeCard> unlockedCards { get; private set; }
    [field: SerializeField, ReadOnly] public List<UpgradeCard> lockedCards { get; private set; }
    [field: SerializeField, ReadOnly] public List<UpgradeCard> purchasedCards { get; private set; }


    public void AddCard(UpgradeCard card, BuildingDataSO dataSO)
    {
        card = Instantiate(card, cardsContainer);
        card.buildingUpgradeData = dataSO;

        ManageCardList(card, dataSO);
    }
    // Using polymorphism
    public void AddCard(UpgradeCard card, UnitProduceStatsSO dataSO)
    {
        card = Instantiate(card, cardsContainer);
        card.unitUpgradeData = dataSO;

        ManageCardList(card, dataSO);
    }

    public void ManageCardList(UpgradeCard card, UnitProduceStatsSO dataSO)
    {
        if (dataSO.cardDetails.isUnlocked)
        {
            unlockedCards.Add(card);

            if (dataSO.cardDetails.purchased)
                purchasedCards.Add(card);
        }
        else
        {
            lockedCards.Add(card);
            dataSO.cardDetails.purchased = false;
        }
    }
    // Using polymorphism
    public void ManageCardList(UpgradeCard card, BuildingDataSO dataSO)
    {
        if (dataSO.cardDetails.isUnlocked)
        {
            unlockedCards.Add(card);

            if (dataSO.cardDetails.purchased)
                purchasedCards.Add(card);
        }
        else
        {
            lockedCards.Add(card);
            dataSO.cardDetails.purchased = false;
        }
    }
}
