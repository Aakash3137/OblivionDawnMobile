using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class RewardPanelScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Userdata userdata;
    [SerializeField] private Button closeBtn;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform[] dayItems;
    [SerializeField] private List<DayBlock> dayBlocks = new List<DayBlock>();
    [SerializeField] private TMP_Text RewardPointTxt;
    [SerializeField] private TMP_Text MessageTxt;

    private string lastClaimKey = "LastClaimDate";

    private void OnEnable()
    {
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(OnClickCloseBtn);

        SetClainButtonListeners();
        CheckMissedDay();
        RefreshUI();
        ScrollToCurrentDay();
    }

    // ===============================
    // CHECK IF PLAYER MISSED DAY
    // ===============================
    private void CheckMissedDay()
    {
        string lastClaim = PlayerPrefs.GetString(lastClaimKey, "");

        if (!string.IsNullOrEmpty(lastClaim))
        {
            DateTime lastDate = DateTime.Parse(lastClaim);
            DateTime today = DateTime.UtcNow.Date;

            double diff = (today - lastDate).TotalDays;

            if (diff > 1)
            {
                // Reset streak
                userdata.CurrentDay = 1;
                ResetAllRewards();
            }
        }
    }

    private void ResetAllRewards()
    {
        for (int i = 0; i < userdata.DayRewards.Length; i++)
        {
            userdata.DayRewards[i] = false;
        }
    }

    // ===============================
    // CLAIM LOGIC
    // ===============================
    void SetClainButtonListeners()
    {
        foreach(DayBlock block in dayBlocks)
        {
            block.claimButton.onClick.RemoveAllListeners();
            block.claimButton.onClick.AddListener(() => OnClaim());
        }
    }

    public void OnClaim()
    {
        if (userdata.CurrentDay > userdata.DayRewards.Length)
            return;

        // Already claimed today?
        string lastClaim = PlayerPrefs.GetString(lastClaimKey, "");
        if (!string.IsNullOrEmpty(lastClaim))
        {
            DateTime lastDate = DateTime.Parse(lastClaim);
            if ((DateTime.UtcNow.Date - lastDate.Date).TotalDays < 1)
            {
                Debug.Log("Already claimed today");
                return;
            }
        }

        int index = userdata.CurrentDay - 1;

        userdata.DayRewards[index] = true;
        PlayerPrefs.SetString(lastClaimKey, DateTime.UtcNow.Date.ToString());
        PlayerPrefs.Save();

        userdata.CurrentDay++;

        RefreshUI();
        ScrollToCurrentDay();
    }

    // ===============================
    // UPDATE ALL DAY BLOCK UI
    // =============================== 
    private void RefreshUI()
    {
        for (int i = 0; i < dayBlocks.Count; i++)
        {
            if (userdata.DayRewards[i])
            {
                dayBlocks[i].SetClaimed();
            }
            else if (i == userdata.CurrentDay - 1)
            {
                dayBlocks[i].SetUnlocked();
            }
            else
            {
                dayBlocks[i].SetLocked();
            }
        }
    }

    // ===============================
    // AUTO SCROLL
    // =============================== 
    public void ScrollToCurrentDay()
    {
        if (userdata.CurrentDay <= 0 || userdata.CurrentDay > dayItems.Length)
            return;

        Canvas.ForceUpdateCanvases();

        RectTransform target = dayItems[userdata.CurrentDay - 1];

        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        float targetY = Mathf.Abs(target.anchoredPosition.y);

        float normalizedPosition = 1 - (targetY / (contentHeight - viewportHeight));

        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
    }

    // ===============================
    private void OnClickCloseBtn()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
    }
}