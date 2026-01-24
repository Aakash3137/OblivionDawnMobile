using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecManager : MonoBehaviour
{
    [Header("Deck Configuration")]
    [SerializeField] private List<DecSelector> deckList = new();

    [Header("Inventory Reference")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("UI")]
    [SerializeField] private Image selectedFactionIcon;
    [SerializeField] private Canvas _Canvas;

    // --------------------------------------------------
    // UNITY EVENTS
    // --------------------------------------------------

    private void OnEnable()
    {
        if (deckList.Count > 0)
            SelectDeck(deckList[0]);

        BuildAllCardsInventory();
    }

    // --------------------------------------------------
    // DECK SELECTION
    // --------------------------------------------------

    public void OnClickButton(GameObject clickedButton)
    {
        foreach (var deck in deckList)
        {
            deck.InActiveObj.SetActive(true);
            deck._Checked.SetActive(false);
        }

        DecSelector selected =
            deckList.Find(d => d.InActiveObj == clickedButton);

        if (selected == null)
            return;

        SelectDeck(selected);
    }

    private void SelectDeck(DecSelector selected)
    {
        selected.InActiveObj.SetActive(false);
        selected._Checked.SetActive(true);

        if (selected.FactionSprite != null)
            selectedFactionIcon.sprite = selected.FactionSprite;

        StartCoroutine(BuildEquippedDeck(selected));
    }

    // --------------------------------------------------
    // EQUIPPED (CUSTOM DECK)
    // --------------------------------------------------

    private IEnumerator BuildEquippedDeck(DecSelector selected)
    {
        yield return null;

        inventoryManager.ClearEquipped();

        foreach (UnitProduceStatsSO card in selected.Cards)
        {
            inventoryManager.AddEquippedItem(card.unitIdentity.name, card.unitType.ToString(), _Canvas,
            "UnitLevel: " + (card.unitIdentity.spawnLevel + 1)
                + "\nHealth: " + card.unitUpgradeData[0].unitBasicStats.maxHealth
                + "\nArmor: " + card.unitUpgradeData[0].unitBasicStats.armor
                + "\nAttack Range: " + card.unitUpgradeData[0].unitRangeStats.attackRange, selected._FactionName, true
                );
        }
    }

    // --------------------------------------------------
    // UNEQUIPPED (ALL CARDS)
    // --------------------------------------------------

    private void BuildAllCardsInventory()
    {
        inventoryManager.ClearUnequipped();

        foreach (DecSelector deck in deckList)
        {
            foreach (UnitProduceStatsSO card in deck.Cards)
            {
                inventoryManager.AddUnequippedItem(card.unitIdentity.name, card.unitType.ToString(), _Canvas,
                "UnitLevel: " + (card.unitIdentity.spawnLevel + 1)
                    + "\nHealth: " + card.unitUpgradeData[0].unitBasicStats.maxHealth
                    + "\nArmor: " + card.unitUpgradeData[0].unitBasicStats.armor
                    + "\nAttack Range: " + card.unitUpgradeData[0].unitRangeStats.attackRange, deck._FactionName, false
                    );
            }
        }
    }
}

[System.Serializable]
public class DecSelector
{
    public FactionName _FactionName;
    public GameObject _Checked;
    public GameObject InActiveObj;
    public Sprite FactionSprite;
    public List<UnitProduceStatsSO> Cards = new();
}
