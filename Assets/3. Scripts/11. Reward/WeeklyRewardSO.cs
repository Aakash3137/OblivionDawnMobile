using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeeklyReward", menuName = "Data/Weekly Reward")]
public class WeeklyRewardSO : ScriptableObject
{
    private const string LastClaimKey = "LastClaimUTC";

    [Header("Reward Data")]
    public List<DayRewardBlock> days = new List<DayRewardBlock>();
    public List<WeekData> weeks = new List<WeekData>();

    [Header("Timer Settings")]
    public bool UseTestTime = true;

    [Tooltip("Test time in hours (0.01 = 36 seconds)")]
    public float TestHours = 0.1f;

    [Header("Runtime")]
    public bool RewardReady;

    // ======================
    // SELECT WEEK
    // ======================

    public void SelectedWeek(int currentDay)
    {
        if (currentDay != 1 && days.Count > 0)
            return;

        if (weeks.Count == 0)
        {
            Debug.LogError("No week data configured!");
            return;
        }

        int weekIndex = UnityEngine.Random.Range(0, weeks.Count);

        days.Clear();
        days.AddRange(weeks[weekIndex].days);
    }

    // ======================
    // GET REWARD
    // ======================

    public DayRewardBlock GetReward(int day)
    {
        if (day <= 0 || day > days.Count)
            return null;

        return days[day - 1];
    }

    // ======================
    // TIMER
    // ======================

    public DateTime GetNextUnlockTime()
    {
        string saved = PlayerPrefs.GetString(LastClaimKey, "");

        if (string.IsNullOrEmpty(saved))
            return DateTime.MaxValue;

        DateTime lastClaim = DateTime.Parse(saved);

        if (UseTestTime)
            return lastClaim.AddHours(TestHours);

        return lastClaim.AddDays(1);
    }

    public TimeSpan GetRemainingTime()
    {
        DateTime next = GetNextUnlockTime();

        if (next == DateTime.MaxValue)
            return TimeSpan.Zero;

        return next - DateTime.UtcNow;
    }

    public void UpdateRewardState()
    {
        string saved = PlayerPrefs.GetString(LastClaimKey, "");

        if (string.IsNullOrEmpty(saved))
        {
            RewardReady = true;
            return;
        }

        RewardReady = GetRemainingTime().TotalSeconds <= 0;
    }

    public void SaveClaimTime()
    {
        PlayerPrefs.SetString(LastClaimKey, DateTime.UtcNow.ToString());
        PlayerPrefs.Save();

        RewardReady = false;
    }

    // ======================
    // DAY PROGRESSION
    // ======================

    public void UpdateDay(Userdata userdata)
    {
        if (!RewardReady)
            return;

        userdata.CurrentDay++;

        if (userdata.CurrentDay > 7)
        {
            ResetWeek(userdata);
            return;
        }

        RewardReady = false;
    }   

    public void ResetWeek(Userdata userdata)
    {
        Debug.Log("Resetting Weekly Rewards");

        userdata.CurrentDay = 1;

        for (int i = 0; i < userdata.DayRewards.Length; i++)
        {
            userdata.DayRewards[i] = false;
        }

        PlayerPrefs.DeleteKey(LastClaimKey);

        SelectedWeek(userdata.CurrentDay);

        RewardReady = true;
    }


    public void ResetDay(Userdata userdata)
    {
        if(userdata.CurrentDay >= 7 || userdata.CheckDay())
            ResetWeek(userdata);
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

public enum RewardItem
{
    Gems,
    Fragments,
    UnitCards,
    DefenseCards,
    MapShards
}