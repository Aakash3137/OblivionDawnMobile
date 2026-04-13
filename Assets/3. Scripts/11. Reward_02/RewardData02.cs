using UnityEngine;

public abstract class RewardData02 : ScriptableObject
{
    public RewardType02 rewardType;
    public string rewardName;
    public Sprite icon;

    public virtual Sprite GetIcon(FactionName faction = default)
    {
        return null;
    }

    public abstract void Grant(int amount);
}