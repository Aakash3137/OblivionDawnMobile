using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class RewardPanelScript : MonoBehaviour
{
    public static RewardPanelScript Instance { get; private set; }
    private CanvasGroup canvasGroup;

    [Header("References")]
    [SerializeField] private Userdata userdata;
    [SerializeField] private WeeklyRewardSO rewardData;

    [SerializeField] private Button closeButton;
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

    // =============================
    // PANEL OPEN
    // =============================
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HidePanel();
    }

    // previously OnEnable  
    public void OpenRewardPanel()
    {
        userdata.CurrentDay = Mathf.Clamp(userdata.CurrentDay, 1, 7);

        rewardData.SelectedWeek(userdata.CurrentDay);
        rewardData.UpdateRewardState(userdata);
        SetupTimer();
        ScrollToCurrentDay();

        ShowPanel();
    }

    void Start()
    {
        closeButton.onClick.AddListener(OnClickClosePanel);
        rewardClaimBtn.onClick.AddListener(OnClaim);

        rewardPopup.SetActive(false);

        StartCoroutine(CreateDayBlocks());
    }

    // =============================
    // CREATE UI
    // =============================

    IEnumerator CreateDayBlocks()
    {
        dayBlocks.Clear();
        dayItems.Clear();

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

        RefreshUI();
        ScrollToCurrentDay();

    }

    // =============================
    // CLAIM REWARD
    // =============================

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

        if (index >= 0 && index < userdata.DayRewards.Length)
            userdata.DayRewards[index] = true;

        rewardData.SaveClaimTime();
        rewardData.NextDay(userdata);

        rewardPopup.SetActive(false);

        RefreshUI();
        StartCoroutine(ScrollAfterLayout());
        SetupTimer();
    }

    // =============================
    // TIMER
    // =============================

    void SetupTimer()
    {
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (true)
        {
            rewardData.UpdateRewardState(userdata);

            TimeSpan remaining = rewardData.GetRemainingTime();

            if (remaining.TotalSeconds <= 0)
            {
                countdownText.text = "REWARD READY!";
                rewardData.RewardReady = true;
            }
            else
            {
                rewardData.RewardReady = false;

                countdownText.text =
                    $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }

            RefreshUI();

            yield return new WaitForSeconds(1);
        }
    }

    // =============================
    // UI REFRESH
    // =============================

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

    // =============================
    // SCROLL
    // =============================

    void ScrollToCurrentDay()
    {
        if (dayItems.Count == 0)
            return;

        int index = userdata.CurrentDay - 1;

        if (index < 0 || index >= dayItems.Count)
            return;

        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;
        RectTransform target = dayItems[index];

        float viewportCenter = viewport.rect.height * 0.5f;
        float targetPos = Mathf.Abs(target.anchoredPosition.y);

        float newY = targetPos - viewportCenter;

        newY = Mathf.Clamp(newY, 0, content.rect.height - viewport.rect.height);

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
    }

    IEnumerator ScrollAfterLayout()
    {
        yield return null; // wait 1 frame for layout
        Canvas.ForceUpdateCanvases();

        ScrollToCurrentDay();
    }

    // =============================
    // UI BUTTONS
    // =============================

    private void OnClickClosePanel()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }

    void OpenPopup()
    {
        rewardPopup.SetActive(true);
    }
    public void ShowPanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void OnDestroy()
    {
        rewardClaimBtn.onClick.RemoveListener(OnClaim);
        closeButton.onClick.RemoveListener(OnClickClosePanel);
    }
}