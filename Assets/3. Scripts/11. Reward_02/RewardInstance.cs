using System;
using Sirenix.OdinInspector;

[Serializable]
public class RewardInstance
{
    public RewardData02 rewardData;
    public int amount;

    [ShowIf(nameof(ShowFaction))]
    [LabelText("Faction (Only for Fragments)")]
    public FactionName faction;

    private bool ShowFaction()
    {
        return rewardData != null && rewardData.rewardType == RewardType02.Fragment;
    }
}