using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WeeklyReward", menuName = "Data/Weekly Reward")]
public class WeeklyRewardSO : ScriptableObject
{
    private const string LastClaimKey = "LastClaimUTC";

    [Header("Reward Data")]
    public List<DayRewardBlock> days = new List<DayRewardBlock>();
    public List<WeekData> weeks = new List<WeekData>();

    [Header("Timer Settings")]
    public bool UseTestTime = false;

    [Tooltip("Test time in minutes")]
    public float TestMinutes = 1f;

    [ShowInInspector] public bool RewardReady;

    // ==========================
    // SELECT RANDOM WEEK
    // ==========================

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
 
    // ==========================
    // GET REWARD
    // ==========================

    public DayRewardBlock GetReward(int day)
    {
        if (day <= 0 || day > days.Count)
            return null;

        return days[day - 1];
    }

    // ==========================
    // SAVE CLAIM TIME
    // ==========================
    public void SaveClaimTime()
    {
        PlayerPrefs.SetString(LastClaimKey, DateTime.UtcNow.ToString());
        PlayerPrefs.Save();
    }

    // ==========================
    // NEXT DAY
    // ==========================
    public void NextDay(Userdata data)
    {
        data.CurrentDay++;

        // if reached after day 7
        if (data.CurrentDay > 7)
        {
            data.CurrentDay = 8; // special state (week completed)
        }
    }

    // ==========================
    // RESET WEEK
    // ==========================
    public void ResetWeek(Userdata data)
    {
        data.CurrentDay = 1;

        for (int i = 0; i < data.DayRewards.Length; i++)
        {
            data.DayRewards[i] = false;
        }

        // DO NOT delete timer
        // PlayerPrefs.DeleteKey(LastClaimKey); ❌ remove this

        SelectedWeek(1);
    }

    // ==========================
    // TIMER CALCULATION
    // ==========================
    public DateTime GetNextUnlockTime()
    {
        string savedDate = PlayerPrefs.GetString(LastClaimKey, "");

        if (string.IsNullOrEmpty(savedDate))
        {
            RewardReady = true;
            return DateTime.UtcNow;
        }

        DateTime lastClaim = DateTime.Parse(savedDate);

        if (UseTestTime)
            return lastClaim.AddMinutes(TestMinutes);

        return lastClaim.Date.AddDays(1);
    }

    public TimeSpan GetRemainingTime()
    {
        DateTime nextTime = GetNextUnlockTime();

        TimeSpan remaining = nextTime - DateTime.UtcNow;

        if (remaining.TotalSeconds <= 0)
        {
            RewardReady = true;
            return TimeSpan.Zero;
        }

        RewardReady = false;

        return remaining;
    }

    // ==========================
    // UPDATE STATE
    // ==========================

    public void UpdateRewardState(Userdata data)
    {
        TimeSpan remaining = GetRemainingTime();

        if (remaining.TotalSeconds <= 0)
        {
            RewardReady = true;

            // if week completed → reset after timer
            if (data.CurrentDay > 7)
            {
                ResetWeek(data);
            }
        }
        else
        {
            RewardReady = false;
        }
    }
}

[Serializable]
public class DayRewardBlock
{
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