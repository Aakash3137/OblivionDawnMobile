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
    public HashSet<ScriptableObject> scriptablesInDeck = new();

    public void AddCard(UpgradeCard cardPrefab, ScriptableObject dataSO)
    {
        var card = Instantiate(cardPrefab, cardsContainer);
        card.upgradeDataSO = dataSO;
        card.myPanel = this;
        card.InitializeCard();
        ManageCardList(card, dataSO);
    }

    public void ManageCardList(UpgradeCard card, ScriptableObject dataSO)
    {
        CardState cardState;

        if (dataSO is UnitProduceStatsSO unitProduceStatsSO)
            cardState = unitProduceStatsSO.cardDetails.cardState;
        else if (dataSO is BuildingDataSO buildingDataSO)
            cardState = buildingDataSO.cardDetails.cardState;
        else
        {
            Debug.LogError("[CardsPanel] Imposter Scriptable Object not Unit SO or Building SO");
            return;
        }

        switch (cardState)
        {
            case CardState.Unlocked:
                unlockedCards.Add(card);
                break;
            case CardState.Locked:
                lockedCards.Add(card);
                break;
            case CardState.Purchased:
                purchasedCards.Add(card);
                break;
        }

        allCards.Add(card);
    }
}