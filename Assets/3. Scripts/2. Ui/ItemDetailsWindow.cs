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
    [SerializeField] private GameObject AttributesPrefab;
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


    private void OnEnable()
    {
        closeButton.onClick.AddListener(HideItemDetails);
    }

    internal void ShowItemDetails(string Name, string Details, Sprite itemIcon, string Type, FactionName _Faction)
    {
        itemName.text = Name;
        itemDescriptionText.text = Details;
        itemIconImage.sprite = itemIcon;
        itemTypeText.text = Type;
        FactionType.text = _Faction.ToString();
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
        gameObject.SetActive(true);
    }

    private void HideItemDetails()
    {
        gameObject.SetActive(false);
    }
}

[SerializeField]
public class AttributeData
{
    public string AttributeName;
    public float AttributeValue;
    public float AttributeIncreaseby;
    public Sprite Icon;
}

[SerializeField]
public class AbilityData
{
    public bool UnlockStatus;
    public string AbilityName;
}


