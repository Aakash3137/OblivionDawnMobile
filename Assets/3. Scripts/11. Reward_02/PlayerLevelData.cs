using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelRewardEntry
{
    public int level;
    public RewardBundle rewardBundle;

    // NEW: track claim state
    public bool isClaimed;
}

[CreateAssetMenu(menuName = "Progression/Player Level Data")]
public class PlayerLevelData : ScriptableObject
{
    public List<LevelRewardEntry> levelRewards;

    private Dictionary<int, LevelRewardEntry> levelRewardMap;

    private void OnEnable()
    {
        levelRewardMap = new Dictionary<int, LevelRewardEntry>();

        foreach (var entry in levelRewards)
        {
            if (!levelRewardMap.ContainsKey(entry.level))
                levelRewardMap.Add(entry.level, entry);
        }
    }

    public LevelRewardEntry GetEntry(int level)
    {
        if (levelRewardMap == null)
            OnEnable();

        if (levelRewardMap.TryGetValue(level, out var entry))
            return entry;

        Debug.LogWarning($"No reward entry found for level {level}");
        return null;
    }

    public RewardBundle GetRewardForLevel(int level)
    {
        return GetEntry(level)?.rewardBundle;
    }

    public bool IsClaimed(int level)
    {
        return GetEntry(level)?.isClaimed ?? true;
    }

    public void MarkClaimed(int level)
    {
        var entry = GetEntry(level);
        if (entry != null)
            entry.isClaimed = true;
    }
}