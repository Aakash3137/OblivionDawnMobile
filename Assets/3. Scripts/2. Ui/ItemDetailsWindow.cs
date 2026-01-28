using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsWindow : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemTypeText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text FactionType;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button UpgradeButton;

    [Header ("Attributes")]
    [SerializeField] private BlockScript AttributesPrefab;
    [SerializeField] private Transform AttributesParent;
    [SerializeField] private List<AttributeData> AttributeList;

    [Header ("Level")]
    [SerializeField] private Slider LevelSlider;
    [SerializeField] private TMP_Text LevelNoText;
    [SerializeField] private int CurrentLevel;
    [SerializeField] private int MaxLevel = 10;

    [Header ("Abilities")]
    [SerializeField] private GameObject AbilitiesPrefab;
    [SerializeField] private Transform AbilitiesParent;
    [SerializeField] private List<AbilityData> AbilityList;

    [Header ("Game Data")]
    internal UnitProduceStatsSO unit;

    

    private void OnEnable()
    {
        closeButton.onClick.AddListener(HideItemDetails);
    }

    internal void ShowItemDetails(string Name, string Details, Sprite itemIcon, string Type, FactionName _Faction)
    {
        itemName.text = Name;
        // itemDescriptionText.text = Details;
        itemIconImage.sprite = itemIcon;
        itemTypeText.text = Type;
        switch(_Faction)
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

        if(AbilityList.Count > 0)
        {
            for(int j = 0; j < AbilityList.Count; j++)
            {
                GameObject abilityObj = Instantiate(AbilitiesPrefab, AbilitiesParent);
                abilityObj.name = AbilityList[j].AbilityName;
                abilityObj.GetComponent<Image>().sprite = AbilityList[j].AbilityImage.sprite;
                if(AbilityList[j].UnlockStatus)
                {
                    abilityObj.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    abilityObj.GetComponent<Image>().color = Color.gray;
                }
            }

            
        }

        for(int i = 0; i < AttributeList.Count; i++)
            {
                BlockScript attributeObj = Instantiate(AttributesPrefab, AttributesParent);
                attributeObj.IconImage.sprite = AttributeList[i].Icon;
                attributeObj.TitleText.text = AttributeList[i].AttributeName;
                attributeObj.CurrentValueText.text = AttributeList[i].AttributeValue.ToString();
                if(AttributeList[i].AttributeIncreaseby > 0)
                {
                    attributeObj.Increasable = true;
                    attributeObj.IncreaseByText.text =  AttributeList[i].AttributeIncreaseby.ToString() + " +";
                }
                else
                {
                    attributeObj.Increasable = false;
                    attributeObj.IncreaseByText.text = "";
                }
                switch (i) {
                    case 0:
                        attributeObj.name = "Max Health";
                        
                        break;
                    default :
                        
                        break;
                }
            }

        LevelNoText.text =  CurrentLevel.ToString();
        LevelSlider.maxValue = MaxLevel;  
        LevelSlider.minValue = CurrentLevel;
        gameObject.SetActive(true);
    }

    internal void OnClickUpgradeButton()
    {
        UnitProduceUpgrade _UpgradeData = new UnitProduceUpgrade();
        _UpgradeData.UpgradeNext(unit);
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
