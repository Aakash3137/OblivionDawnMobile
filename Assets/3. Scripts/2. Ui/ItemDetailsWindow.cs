using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemTypeText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text FactionType;

    [SerializeField] private Button closeButton;

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
        gameObject.SetActive(true);
    }

    private void HideItemDetails()
    {
        gameObject.SetActive(false);
    }
}
