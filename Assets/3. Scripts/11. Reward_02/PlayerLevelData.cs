using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelRewardEntry
{
    public int level;
    public RewardBundle rewardBundle;
}

[CreateAssetMenu(menuName = "Progression/Player Level Data")]
public class PlayerLevelData : ScriptableObject
{
    public List<LevelRewardEntry> levelRewards;

    private Dictionary<int, RewardBundle> levelRewardMap;

    private void OnEnable()
    {
        levelRewardMap = new Dictionary<int, RewardBundle>();

        foreach (var entry in levelRewards)
        {
            if (!levelRewardMap.ContainsKey(entry.level))
                levelRewardMap.Add(entry.level, entry.rewardBundle);
        }
    }

    public RewardBundle GetRewardForLevel(int level)
    {
        if (levelRewardMap == null)
            OnEnable();

        if (levelRewardMap.TryGetValue(level, out var bundle))
            return bundle;

        Debug.LogWarning($"No reward bundle found for level {level}");
        return null;
    }
}