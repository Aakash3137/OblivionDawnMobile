using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class RewardPanelScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Userdata userdata;
    [SerializeField] private Button closeBtn;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform[] dayItems;
    [SerializeField] private DayBlock[] dayBlocks;

    [Header("Timer UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private Image TimerFilledImage;
    [SerializeField] private Transform HourHand;
    [SerializeField] private Transform MinuteHand;
    [Header ("Effects")]
    [SerializeField] private RewardEffects _Gems;
    [SerializeField] private RewardEffects _Chest;

    [Header("Reward Data")]
    [SerializeField] private WeeklyRewardSO rewardData;

    private const string RewardIndexKey = "WeeklyRewardIndex";

    private int currentRewardIndex;

    private const string LastClaimKey = "LastClaimUTC";

    // ==============================
    // TEST SETTINGS
    // ==============================
    public static bool UseTestTime = true;   // TRUE = 1 hour test
    public static float TestHours = 0.01f;     // 0.01 hours = 36 seconds

    private DateTime nextUnlockTime;
    private Coroutine timerRoutine;

    // ==============================
    private void OnEnable()
    {
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(OnClickCloseBtn);

        ValidateStreak();
        RefreshUI();
        ScrollToCurrentDay();
        SetupTimer();
        LoadOrCreateRewardIndex();
        rewardData.SelectedWeek(userdata.CurrentDay);
    }

    private void OnDisable()
    {
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);
    }

    // ==============================
    // STREAK VALIDATION
    // ==============================
    private void ValidateStreak()
    {
        string savedDate = PlayerPrefs.GetString(LastClaimKey, "");
        if (userdata.CurrentDay > 7)
        {
            userdata.CurrentDay = 1;

            ResetRewards();
            ResetWeekRewardIndex();
        }

        if (string.IsNullOrEmpty(savedDate))
        {
            userdata.CurrentDay = 1;
            return;
        }

        DateTime lastClaimDate = DateTime.Parse(savedDate);
        DateTime now = DateTime.UtcNow;

        if (!UseTestTime)
        {
            int difference = (now.Date - lastClaimDate.Date).Days;

            if (difference == 1)
                userdata.CurrentDay++;
            else if (difference > 1)
            {
                userdata.CurrentDay = 1;
                ResetRewards();
            }
        }
        // In test mode → do NOT auto increase day
    }

    private void ResetRewards()
    {
        for (int i = 0; i < userdata.DayRewards.Length; i++)
            userdata.DayRewards[i] = false;
    }

    // ==============================
    // CLAIM LOGIC
    // ==============================
    public void OnClaim()
    {
        DayRewardBlock reward = GetTodayReward();

        userdata.Coins += reward.RewardAmount;
        userdata.Diamonds += reward.RewardAmount;

        switch (reward.RewardItemType)
        {
            case RewardItem.Gems:
                userdata.Diamonds = reward.RewardAmount;
                break;
            case RewardItem.giveChest:
                // Implement Chest reward logic here
                break;
            case RewardItem.UnitCards:
                // Implement Unit Card reward logic here
                break;
            case RewardItem.DefenseCards:
                // Implement Defense Card reward logic here
                break;
            case RewardItem.Fragments:
                userdata.Coins = reward.RewardAmount;
                break;
                default:
                Debug.LogWarning("Unhandled reward type: " + reward.RewardItemType);
                break;
        }


        int index = userdata.CurrentDay - 1;
        userdata.DayRewards[index] = true;

        PlayerPrefs.SetString(LastClaimKey, DateTime.UtcNow.ToString());
        PlayerPrefs.Save();

        RefreshUI();
        ScrollToCurrentDay();
        SetupTimer();
    }

    private bool IsRewardReady()
    {
        string savedDate = PlayerPrefs.GetString(LastClaimKey, "");

        if (string.IsNullOrEmpty(savedDate))
            return true;

        DateTime lastClaimDate = DateTime.Parse(savedDate);
        DateTime now = DateTime.UtcNow;

        if (UseTestTime)
            return (now - lastClaimDate).TotalHours >= TestHours;

        return (now.Date - lastClaimDate.Date).Days >= 1;
    }

    // ==============================
    // TIMER SYSTEM (Coroutine Based)
    // ==============================
    private void SetupTimer()
    {
        string savedDate = PlayerPrefs.GetString(LastClaimKey, "");

        if (string.IsNullOrEmpty(savedDate))
        {
            countdownText.text = "Reward Ready!";
            return;
        }

        DateTime lastClaimDate = DateTime.Parse(savedDate);

        if (UseTestTime)
            nextUnlockTime = lastClaimDate.AddHours(TestHours);
        else
            nextUnlockTime = lastClaimDate.Date.AddDays(1);

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            TimeSpan remaining = nextUnlockTime - DateTime.UtcNow;

            if (remaining.TotalSeconds <= 0)
            {
                countdownText.text = "Reward Ready!";
                RefreshUI();
                yield break;
            }

            countdownText.text =
                $"Next Reward In\n{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            
            SetClock(remaining);

            yield return new WaitForSeconds(1f);
        }
    }

    void SetClock(TimeSpan time)
    {
        float totalSeconds = (float)time.TotalSeconds;
        float daySeconds = UseTestTime ? TestHours * 3600f : 24 * 3600f;

        HourHand.gameObject.SetActive(!UseTestTime);
            MinuteHand.gameObject.SetActive(!UseTestTime);

        if(UseTestTime)
        {
            TimerFilledImage.fillAmount = totalSeconds / daySeconds;
        }
        else
        {
            float hourRotation =
            (time.Hours * 15f) +
            (time.Minutes * 0.25f) +
            (time.Seconds * (0.25f / 60f));

            float minuteRotation =
            (time.Minutes * 6f) +
            (time.Seconds * 0.1f);

            // Get current Z rotation
            float currentHourZ = HourHand.localEulerAngles.z;
            float currentMinuteZ = MinuteHand.localEulerAngles.z;

            // Add new rotation
            HourHand.localRotation = Quaternion.Euler(0, 0, currentHourZ - hourRotation);
            MinuteHand.localRotation = Quaternion.Euler(0, 0, currentMinuteZ - minuteRotation);
        }        
    }

    // ==============================
    // UI REFRESH
    // ==============================
    private void RefreshUI()
    {
        dayText.text = "DAY " + userdata.CurrentDay;

        for (int i = 0; i < dayBlocks.Length; i++)
        {
            dayBlocks[i].claimButton.onClick.RemoveAllListeners();

            if (userdata.DayRewards[i])
            {
                dayBlocks[i].SetClaimed();
            }
            else if (i == userdata.CurrentDay - 1)
            {
                if (userdata.CurrentDay == 1 && !PlayerPrefs.HasKey(LastClaimKey))
                {
                    dayBlocks[i].SetUnlocked();
                    dayBlocks[i].claimButton.onClick.AddListener(OnClaim);
                }
                else if (IsRewardReady())
                {
                    dayBlocks[i].SetUnlocked();
                    dayBlocks[i].claimButton.onClick.AddListener(OnClaim);
                }
                else
                {
                    dayBlocks[i].SetLocked();
                }
            }
            else
            {
                dayBlocks[i].SetLocked();
            }
        }
    }

    private void ScrollToCurrentDay()
    {
        Canvas.ForceUpdateCanvases();

        int index = userdata.CurrentDay - 1;

        float normalized =
            1f - (float)index / (dayItems.Length - 1);

        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalized);
        dayText.text = "Streak: " + (userdata.CurrentDay-1).ToString();
    }
    // ==============================

    // ==============================
    // Reward
    // ==============================
    private void LoadOrCreateRewardIndex()
    {
        if (PlayerPrefs.HasKey(RewardIndexKey))
        {
            currentRewardIndex = PlayerPrefs.GetInt(RewardIndexKey);
        }
        else
        {
            currentRewardIndex = UnityEngine.Random.Range(0, 4);

            PlayerPrefs.SetInt(RewardIndexKey, currentRewardIndex);
            PlayerPrefs.Save();
        }
    }

    private DayRewardBlock GetTodayReward()
    {
        return rewardData.GetReward(userdata.CurrentDay);
    }

    private void ResetWeekRewardIndex()
    {
        currentRewardIndex = UnityEngine.Random.Range(0, 4);

        PlayerPrefs.SetInt(RewardIndexKey, currentRewardIndex);
        PlayerPrefs.Save();
    }
    // ==============================

    private void OnClickCloseBtn()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
}