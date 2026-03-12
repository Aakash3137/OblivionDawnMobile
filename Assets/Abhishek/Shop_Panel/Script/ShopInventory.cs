using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ShopInventory", menuName = "Shop/Shop Inventory")]
public class ShopInventory : ScriptableObject
{
    public string currentSelectedArrow = "Arrow";
    public string currentSelectedBow = "Bow";
    public string currentSelectedSkin = "Skin";

    public int totalBalance = 0;
    public List<ShopItem> Allitems = new List<ShopItem>();

    public GameObject ArrowPrefab;

    public void SetArrowItem(MeshRenderer character)
    {
        foreach (ShopItem _item in Allitems)
        {
            if (currentSelectedArrow == _item.itemName)
            {
                character.material = _item.itemMaterial;
                
                break;
            }
        }
    }

    public void SetBowItem(MeshRenderer Character)
    {
        foreach (ShopItem _item in Allitems)
        {
            if (currentSelectedBow == _item.itemName)
            {
                Character.material = _item.itemMaterial;
                break;
            }
        }
    }
    
    public void SetSkinItem(MeshRenderer Character)
    {
        foreach (ShopItem _item in Allitems)
        {
            if (currentSelectedSkin == _item.itemName)
            {
                Character.material = _item.itemMaterial;
                break;
            }
        }
    }
}