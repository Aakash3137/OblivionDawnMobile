using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public bool IsUsed = false;
    public Material itemMaterial;    
}
