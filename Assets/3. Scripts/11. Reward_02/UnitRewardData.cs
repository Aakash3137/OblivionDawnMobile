using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Unit Reward")]
public class UnitRewardData : RewardData02
{
    public ItemData itemData;

    public override Sprite GetIcon(FactionName faction = default)
    {
        return itemData != null ? itemData.icon : null;
    }


    public void UnlockUnit()
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData is NULL");
            return;
        }

        // 🔥 Detect type automatically
        if (itemData.ItemSo != null)
        {
            itemData.ItemSo.cardDetails.cardState = CardState.Unlocked;
            Debug.Log($"Unlocked ATTACK Unit: {itemData.itemName}");
        }
        else if (itemData.DefenseSo != null)
        {
            itemData.DefenseSo.cardDetails.cardState = CardState.Unlocked;
            Debug.Log($"Unlocked DEFENSE Unit: {itemData.itemName}");
        }
        else if (itemData.BuildingSO != null)
        {
            itemData.BuildingSO.cardDetails.cardState = CardState.Unlocked;
            Debug.Log($"Unlocked BUILDING: {itemData.itemName}");
        }
        else
        {
            Debug.LogError($"No valid SO assigned in ItemData: {itemData.itemName}");
        }
    }

    public override void Grant(int amount)
    {
        //
    }
}