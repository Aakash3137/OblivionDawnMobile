using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Currency Reward")]
public class CurrencyRewardData : RewardData02
{
    public override void Grant(int amount)
    {
        switch (rewardType)
        {
            case RewardType02.Gem:
                PlayerInventory.Instance.AddGems(amount);
                break;

            case RewardType02.Fragment:
                PlayerInventory.Instance.AddFragments(amount);
                break;

            case RewardType02.MapShard:
                PlayerInventory.Instance.AddMapShards(amount);
                break;
        }
    }
}