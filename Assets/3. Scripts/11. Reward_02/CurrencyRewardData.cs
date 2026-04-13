using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Currency Reward")]
public class CurrencyRewardData : RewardData02
{
    [Header("Default Icon (Gems, Shards)")]
    public Sprite defaultIcon;

    [Header("Fragment Icons (Per Faction)")]
    public List<FactionIconEntry> fragmentIcons;

    public override Sprite GetIcon(FactionName faction = default)
    {
        if (rewardType == RewardType02.Fragment)
        {
            foreach (var entry in fragmentIcons)
            {
                if (entry.faction == faction)
                    return entry.icon;
            }

            Debug.LogWarning($"No icon found for faction: {faction}");
            return null;
        }

        return defaultIcon;
    }


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

[System.Serializable]
public class FactionIconEntry
{
    public FactionName faction;
    public Sprite icon;
}