using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Unit Reward")]
public class UnitRewardData : RewardData02
{
    public string unitID;

    public override void Grant(int amount)
    {
        //ADD LOGIC OF UNLOCKING UNITS HERE
    }
}