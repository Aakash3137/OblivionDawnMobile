using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class RewardPanelScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Userdata userdata;
    [SerializeField] private WeeklyRewardSO rewardData;

    [SerializeField] private Button closeBtn;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text dayText;

    [Header("Reward UI")]
    [SerializeField] private DayBlock rewardItemPrefab;
    [SerializeField] private Transform rewardItemParent;

    [Header("Popup")]
    [SerializeField] private GameObject rewardPopup;
    [SerializeField] private Button rewardClaimBtn;

    private List<DayBlock> dayBlocks = new();
    private List<RectTransform> dayItems = new();

    private Coroutine timerRoutine;

    // ======================

    void OnEnable()
    {
        rewardData.SelectedWeek(userdata.CurrentDay);
        rewardData.UpdateRewardState();
        ScrollToCurrentDay();
    }

    void Start()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);

        rewardClaimBtn.onClick.AddListener(OnClaim);

        rewardPopup.SetActive(false);

        StartCoroutine(CreateDayBlocks());
    }

    IEnumerator CreateDayBlocks()
    {
        for (int i = 0; i < 7; i++)
        {
            DayBlock block = Instantiate(rewardItemPrefab, rewardItemParent);

            block.name = "Day_" + (i + 1);
            block.SetDayTxt(i);

            dayBlocks.Add(block);
            dayItems.Add(block.GetComponent<RectTransform>());

            if (i == 6)
                block.Connector.SetActive(false);
        }

        yield return null;

        rewardData.UpdateRewardState();

        RefreshUI();
        ScrollToCurrentDay();

        SetupTimer();
    }

    // ======================
    // CLAIM
    // ======================

    void OnClaim()
    {
        if (!rewardData.RewardReady)
            return;


        DayRewardBlock reward = rewardData.GetReward(userdata.CurrentDay);

        if (reward == null)
            return;

        switch (reward.RewardItemType)
        {
            case RewardItem.Gems:
                userdata.Diamonds += reward.RewardAmount;
                break;

            case RewardItem.Fragments:
                userdata.Coins += reward.RewardAmount;
                break;
        }

        int index = userdata.CurrentDay - 1;

        userdata.DayRewards[index] = true;
        

        rewardData.SaveClaimTime();
        rewardData.UpdateDay(userdata);

        rewardPopup.SetActive(false);

        RefreshUI();
        ScrollToCurrentDay();
        SetupTimer();
    }

    // ======================
    // TIMER
    // ======================

    void SetupTimer()
    {
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        while (true)
        {
            rewardData.UpdateRewardState();

            TimeSpan remaining = rewardData.GetRemainingTime();

            if (remaining.TotalSeconds <= 0)
            {
                countdownText.text = "REWARD READY!";
                // userdata.CurrentDay += 1;
                RefreshUI();
                yield break;
            }

            countdownText.text =
                $"Next Reward In => {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";

            yield return new WaitForSeconds(1);
        }
    }

    // ======================
    // UI
    // ======================

    void RefreshUI()
    {
        dayText.text = "DAY " + userdata.CurrentDay;

        for (int i = 0; i < dayBlocks.Count; i++)
        {
            dayBlocks[i].claimButton.onClick.RemoveAllListeners();

            if (userdata.DayRewards[i])
            {
                dayBlocks[i].SetClaimed();
            }
            else if (i == userdata.CurrentDay - 1)
            {
                if (rewardData.RewardReady)
                {
                    dayBlocks[i].SetUnlocked();
                    dayBlocks[i].claimButton.onClick.AddListener(OpenPopup);
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

    void ScrollToCurrentDay()
    {
        Canvas.ForceUpdateCanvases();

        int index = userdata.CurrentDay - 1;
        Debug.Log("Index: " + index + " Day: " + userdata.CurrentDay + " Day Item["+index+"]: "+ dayItems[index]);
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;
        RectTransform item = dayItems[index];

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        float itemPos = Mathf.Abs(item.anchoredPosition.y);

        float target = (itemPos - viewportHeight * 0.5f) / (contentHeight - viewportHeight);
        float normalized = 1f - Mathf.Clamp01(target);

        StartCoroutine(SmoothScroll(normalized));
    }

    IEnumerator SmoothScroll(float target)
    {
        float start = scrollRect.verticalNormalizedPosition;
        float time = 0;

        while (time < 0.35f)
        {
            time += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, target, time / 0.35f);

            yield return null;
        }

        scrollRect.verticalNormalizedPosition = target;

        PlayRewardHighlight();
    }

    void PlayRewardHighlight()
    {
        int index = userdata.CurrentDay - 1;

        Transform reward = dayBlocks[index].transform;

        StartCoroutine(BounceAnimation(reward));
    }

    IEnumerator BounceAnimation(Transform target)
    {
        Vector3 startScale = Vector3.one;
        Vector3 bigScale = Vector3.one * 1.15f;

        float time = 0;

        while (time < 0.2f)
        {
            time += Time.deltaTime;
            target.localScale = Vector3.Lerp(startScale, bigScale, time / 0.2f);
            yield return null;
        }

        time = 0;

        while (time < 0.2f)
        {
            time += Time.deltaTime;
            target.localScale = Vector3.Lerp(bigScale, startScale, time / 0.2f);
            yield return null;
        }
    }

    // ======================
    // UI BUTTONS
    // ======================

    void OnClickCloseBtn()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }

    void OpenPopup()
    {
        rewardPopup.SetActive(true);
    }
    // ======================
    // Reset
    // ======================

    void ResetDay()
    {
        if(userdata.CurrentDay >= 7 && userdata.CheckDay())
            rewardData.ResetWeek(userdata);
    }

}