using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeeklyReward", menuName = "Data/Weekly Reward")]
public class WeeklyRewardSO : ScriptableObject
{
    public List<DayRewardBlock> days = new List<DayRewardBlock>();
    public List<WeekData> weeks= new List<WeekData>();

    internal void SelectedWeek(int CurrentDay)
    {
        if(CurrentDay != 1 && days.Count > 0)
        {
            return;
        }
        
        int weekIndex = UnityEngine.Random.Range(0, weeks.Count);
        if (weekIndex < 0 || weekIndex >= weeks.Count)
        {
            Debug.LogError("Invalid week index selected: " + weekIndex);
            return;
        }

        // Clear existing day blocks and populate with the selected week's data
        days.Clear();
        days.AddRange(weeks[weekIndex].days);
    }

    public DayRewardBlock GetReward(int day)
    {
        if (day <= 0 || day > days.Count)
            return null;

        return days[day - 1];
    }
}

[Serializable]
public class DayRewardBlock
{
    public int day;
    public int RewardAmount;
    public RewardItem RewardItemType;
}

[Serializable]
public class WeekData
{
    public List<DayRewardBlock> days = new List<DayRewardBlock>();
}

[Serializable]
public enum RewardItem
{
    Gems,
    Fragments,
    UnitCards,
    DefenseCards,
    giveChest
}

