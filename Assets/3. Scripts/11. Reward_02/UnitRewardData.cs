using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Unit Reward")]
public class UnitRewardData : RewardData02
{
    public string unitID;

    public override void Grant(int amount)
    {
        PlayerInventory.Instance.AddUnit(unitID, amount);
    }
}