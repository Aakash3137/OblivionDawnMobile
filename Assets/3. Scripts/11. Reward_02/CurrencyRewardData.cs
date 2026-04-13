using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Currency Reward")]
public class CurrencyRewardData : RewardData02
{
    public override void Grant(int amount)
    {
        RewardManager.Instance.GrantRewards(
            new List<RewardInstance>
            {
                new RewardInstance
                {
                    rewardData = this,
                    amount = amount
                }
            }
        );
    }
}