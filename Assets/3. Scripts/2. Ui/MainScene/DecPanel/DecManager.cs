using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] internal TMP_Text diamondtext;
    [SerializeField] internal Userdata _Profile;
    [SerializeField] private TMP_Text Coins;
    
    private void OnEnable()
    {
        diamondtext.text = _Profile.Diamonds.ToString();
        Coins.text = _Profile.Coins.ToString();
        Debug.Log("DecManager OnEnable Called");
        if (deckList.Count > 0)
            SelectDeck(deckList[0]);
        
        BuildAllCardsInventory();
    }

    public void OnClickButton(GameObject clickedButton)
    {
        foreach (var deck in deckList)
        {
            deck.InActiveObj.SetActive(true);
            deck._Checked.SetActive(false);
        }

        DecSelector selected = deckList.Find(d => d.InActiveObj == clickedButton);

        if (selected == null)
            return;

        SelectDeck(selected);
    }

    private void SelectDeck(DecSelector selected)
    {
        GameDebug.Log($"Selected Deck: {selected._FactionName}");
        selected.InActiveObj.SetActive(false);
        selected._Checked.SetActive(true);

        if (selected.FactionSprite != null)
            selectedFactionIcon.sprite = selected.FactionSprite;

        StartCoroutine(BuildEquippedDeck(selected));
    }

    private IEnumerator BuildEquippedDeck(DecSelector selected)
    {
        Debug.Log("Building Equipped Deck...");
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

            GameDebug.Log($"Added Equipped Item: {card.unitIdentity.name} to Inventory from Deck: {selected._FactionName}");
        }
    }

    private void BuildAllCardsInventory()
    {
        inventoryManager.ClearUnequipped();

        foreach (DecSelector deck in deckList)
        {
            foreach (UnitProduceStatsSO card in deck.Cards)
            {
                GameDebug.Log($"Added Unequipped Item: {card.unitIdentity.name} to Inventory from Deck: {deck._FactionName}");
                
                inventoryManager.AddUnequippedItem(card.unitIdentity.name, card.unitType.ToString(), _Canvas,
                "UnitLevel: " + (card.unitIdentity.spawnLevel + 1)
                    + "\nHealth: " + card.unitUpgradeData[0].unitBasicStats.maxHealth
                    + "\nArmor: " + card.unitUpgradeData[0].unitBasicStats.armor
                    + "\nAttack Range: " + card.unitUpgradeData[0].unitRangeStats.attackRange, deck._FactionName, false
                    );
                
                GameDebug.Log($"Added Unequipped Item: {card.unitIdentity.name} to Inventory from Deck: {deck._FactionName}");
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
