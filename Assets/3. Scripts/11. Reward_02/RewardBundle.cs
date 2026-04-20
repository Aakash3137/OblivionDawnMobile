using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Reward Bundle")]
public class RewardBundle : ScriptableObject
{
    public List<RewardInstance> rewards;
}