using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] private Transform cardsContainer;

    [field: SerializeField, ReadOnly, Space(20)] public List<UpgradeCard> unlockedCards { get; private set; }
    [field: SerializeField, ReadOnly] public List<UpgradeCard> lockedCards { get; private set; }
    [field: SerializeField, ReadOnly] public List<UpgradeCard> purchasedCards { get; private set; }
    [field: SerializeField, ReadOnly] public List<UpgradeCard> allCards { get; private set; }


    public void AddCard(UpgradeCard cardPrefab, ScriptableObject dataSO)
    {
        var card = Instantiate(cardPrefab, cardsContainer);
        card.upgradeDataSO = dataSO;
        card.myPanel = this;
        card.RefreshCard();
        ManageCardList(card, dataSO);
    }

    public void ManageCardList(UpgradeCard card, ScriptableObject dataSO)
    {
        CardDetails cardDetails = null;

        if (dataSO is UnitProduceStatsSO unitProduceStatsSO)
            cardDetails = unitProduceStatsSO.cardDetails;
        else if (dataSO is BuildingDataSO buildingDataSO)
            cardDetails = buildingDataSO.cardDetails;

        if (cardDetails == null)
        {
            Debug.LogError("[CardsPanel] CardDetails is null");
            return;
        }

        if (cardDetails.isUnlocked)
        {
            unlockedCards.Add(card);

            if (cardDetails.purchased)
                purchasedCards.Add(card);
        }
        else
        {
            lockedCards.Add(card);
            cardDetails.purchased = false;
        }

        allCards.Add(card);
    }
}
