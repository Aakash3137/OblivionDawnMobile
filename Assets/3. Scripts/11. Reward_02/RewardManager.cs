using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [Header("Core Data")]
    public Userdata userData;   // Assign in inspector

    private void Awake()
    {
        Instance = this;
    }

    public void ClaimReward(RewardBundle bundle)
    {
        Debug.Log("Claiming reward bundle with " + bundle.rewards.Count + " rewards.");
        ChestUIManager.Instance.OpenChest(bundle);
    }

    public void GrantRewards(List<RewardInstance> rewards)
    {
        foreach (var reward in rewards)
        {
            GrantSingleReward(reward);
        }
    }

    private void GrantSingleReward(RewardInstance reward)
    {
        switch (reward.rewardData.rewardType)
        {
            case RewardType02.Gem:
                userData.Diamonds += reward.amount;
                break;

            case RewardType02.Fragment:
                // Assuming reward has faction info
                userData.AddFragments(reward.faction, reward.amount);
                break;

            case RewardType02.MapShard:
                userData.MapShards += reward.amount;
                break;

            case RewardType02.Unit:
                GrantUnitReward(reward);
                break;

            default:
                Debug.LogWarning("Unhandled reward type: " + reward.rewardData.rewardType);
                break;
        }
    }

    private void GrantUnitReward(RewardInstance reward)
    {
        if (reward.rewardData is UnitRewardData unitReward)
        {
            unitReward.UnlockUnit();

            Debug.Log($"Unlocked Unit: {unitReward.itemData.itemName}");
        }
        else
        {
            Debug.LogError("RewardData is not UnitRewardData!");
        }
    }

}