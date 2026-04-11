using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

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
            reward.rewardData.Grant(reward.amount);
        }
    }
}