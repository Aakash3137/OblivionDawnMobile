using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsWindow : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private DecManager _DecManager;
    [Header("UI References")]
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemTypeText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text FactionType;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button UpgradeButton;

    [Header("Attributes")]
    [SerializeField] private StatBlock AttributesPrefab;
    [SerializeField] private Transform AttributesParent;
    [SerializeField] List<Sprite> AllAttributesIcon;
    [SerializeField] private List<StatBlock> GeneratedAttributes;
    [SerializeField] private List<AttributeData> AttributeList;

    [Header("Level")]
    [SerializeField] private Slider LevelSlider;
    [SerializeField] private TMP_Text LevelNoText;
    [SerializeField] private int CurrentLevel;
    [SerializeField] private int MaxLevel = 10;

    [Header("Abilities")]
    [SerializeField] private GameObject AbilitiesPrefab;
    [SerializeField] private Transform AbilitiesParent;
    [SerializeField] private List<AbilityData> AbilityList;

    [Header("Game Data")]
    internal UnitProduceStatsSO unit;
    internal DefenseBuildingDataSO Defense;
    internal ResourceBuildingDataSO Resourse;


    private void OnEnable()
    {
        closeButton.onClick.AddListener(HideItemDetails);
        UpgradeButton.onClick.AddListener(OnClickUpgradeButton);
    }

    void GenerateAAttributesData()
    {
        if (AttributeList.Count > 0)
        {
            AttributeList.Clear();
        }
        int maxLevel = 20;
        int next = unit.unitUpgradeData.Length;

        if (next > maxLevel)
            return;

        var Last = unit.unitUpgradeData[next - 1];

        var cur = new UnitUpgradeData();
        cur.unitLevel = next;

        // float maxHealth = StatUpgrade.MaxHealth(Last.unitBasicStats.maxHealth, cur.unitLevel, maxLevel);
        // float armor = StatUpgrade.Armour(Last.unitBasicStats.armor, cur.unitLevel, maxLevel);
        // float damage = StatUpgrade.Damage(Last.unitAttackStats.damage, cur.unitLevel, maxLevel);
        // float fireRate = StatUpgrade.FireRate(Last.unitAttackStats.fireRate, cur.unitLevel, maxLevel);
        // float moveSpeed = StatUpgrade.MoveSpeed(Last.unitMobilityStats.moveSpeed, cur.unitLevel, maxLevel);
        // float attackRange = StatUpgrade.AttackRange(Last.unitRangeStats.attackRange, cur.unitLevel, maxLevel);

        // for (int i = 0; i < 6; i++)
        // {
        //     AttributeData _data = new AttributeData();
        //     _data.Icon = AllAttributesIcon[i];
        //     Debug.Log("Icon: " + _data.Icon);
        //     switch (i)
        //     {
        //         case 0:
        //             _data.AttributeName = "Max Health";
        //             Debug.Log("" + _data.AttributeName);
        //             _data.AttributeValue = Last.unitBasicStats.maxHealth;
        //             _data.AttributeIncreaseby = maxHealth - StatUpgrade.MaxHealth(Last.unitBasicStats.maxHealth, cur.unitLevel, maxLevel);
        //             break;

        //         case 1:
        //             _data.AttributeName = "Armor";
        //             Debug.Log("" + _data.AttributeName);
        //             _data.AttributeValue = Last.unitBasicStats.armor;
        //             _data.AttributeIncreaseby = armor - StatUpgrade.Armour(Last.unitBasicStats.armor, cur.unitLevel, maxLevel);
        //             break;

        //         case 2:
        //             _data.AttributeName = "Damage";
        //             Debug.Log("" + _data.AttributeName);
        //             _data.AttributeValue = Last.unitAttackStats.damage;
        //             _data.AttributeIncreaseby = damage - StatUpgrade.Damage(Last.unitAttackStats.damage, cur.unitLevel, maxLevel);
        //             break;

        //         case 3:
        //             _data.AttributeName = "Fire rate";
        //             Debug.Log("" + _data.AttributeName);
        //             _data.AttributeValue = Last.unitAttackStats.fireRate;
        //             _data.AttributeIncreaseby = fireRate - StatUpgrade.FireRate(Last.unitAttackStats.fireRate, cur.unitLevel, maxLevel);
        //             break;

        //         case 4:
        //             _data.AttributeName = "Move Speed";
        //             Debug.Log("" + _data.AttributeName);
        //             _data.AttributeValue = Last.unitMobilityStats.moveSpeed;
        //             _data.AttributeIncreaseby = moveSpeed - StatUpgrade.MoveSpeed(Last.unitMobilityStats.moveSpeed, cur.unitLevel, maxLevel);
        //             break;

        //         case 5:
        //             _data.AttributeName = "Range";
        //             Debug.Log("" + _data.AttributeName);
        //             _data.AttributeValue = Last.unitRangeStats.attackRange;
        //             _data.AttributeIncreaseby = attackRange - StatUpgrade.AttackRange(Last.unitRangeStats.attackRange, cur.unitLevel, maxLevel);
        //             break;
        //     }
        //     AttributeList.Add(_data);
        // }
    }

    internal void ShowItemDetails(string Name, string Details, Sprite itemIcon, string Type, FactionName _Faction)
    {
        GenerateAAttributesData();
        itemName.text = Name;

        itemIconImage.sprite = itemIcon;
        itemTypeText.text = Type;
        switch (_Faction)
        {
            case FactionName.Galvadore:
                FactionType.text = "Galvadore"; break;

            case FactionName.Medieval:
                FactionType.text = "Medieval"; break;

            case FactionName.Present:
                FactionType.text = "Present"; break;

            case FactionName.Futuristic:
                FactionType.text = "Futuristic"; break;
        }

        if (AbilityList.Count > 0)
        {
            for (int j = 0; j < AbilityList.Count; j++)
            {
                GameObject abilityObj = Instantiate(AbilitiesPrefab, AbilitiesParent);
                abilityObj.name = AbilityList[j].AbilityName;
                abilityObj.GetComponent<Image>().sprite = AbilityList[j].AbilityImage.sprite;
                if (AbilityList[j].UnlockStatus)
                {
                    abilityObj.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    abilityObj.GetComponent<Image>().color = Color.gray;
                }
            }
        }

        for (int i = 0; i < AttributeList.Count; i++)
        {
            GeneratedAttributes[i].icon.sprite = AttributeList[i].Icon;
            GeneratedAttributes[i].title.text = AttributeList[i].AttributeName;
            GeneratedAttributes[i].currentValueText.text = ((int)AttributeList[i].AttributeValue).ToString();
            // if (AttributeList[i].AttributeIncreaseby > 0)
            // {
            //     GeneratedAttributes[i].Increasable = true;
            //     GeneratedAttributes[i].nextValueText.text = ((int)AttributeList[i].AttributeIncreaseby).ToString() + " +";
            // }
            // else
            // {
            //     GeneratedAttributes[i].Increasable = false;
            //     GeneratedAttributes[i].nextValueText.text = "";
            // }
        }

        LevelNoText.text = CurrentLevel.ToString();
        LevelSlider.maxValue = MaxLevel;
        LevelSlider.minValue = CurrentLevel;
        gameObject.SetActive(true);
    }

    internal void OnClickUpgradeButton()
    {
        Debug.Log("Upgrade Button");
        // UnitProduceUpgrade _UpgradeData = new UnitProduceUpgrade();
        // _UpgradeData.UpgradeNext(unit, _DecManager);
    }

    private void HideItemDetails()
    {
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class AttributeData
{
    public string AttributeName;
    public float AttributeValue;
    public float AttributeIncreaseby;
    public Sprite Icon;
}

[System.Serializable]
public class AbilityData
{
    public bool UnlockStatus;
    public string AbilityName;
    public Image AbilityImage;
}
