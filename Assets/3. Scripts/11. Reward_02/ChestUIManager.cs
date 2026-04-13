using System.Collections;
using UnityEngine;

public class ChestUIManager : MonoBehaviour
{
    public static ChestUIManager Instance;

    private bool nextPressed = false;

    [Header("UI Panels")]
    public GameObject summaryPanel;
    public GameObject chestPanel;
    public GameObject rewardsPanel;

    [Header("Systems")]
    public ChestIdleAnimator idleAnimator;
    public RadialFlashController radialFlash;

    [Header("Reward Display")]
    public RewardDisplayUI rewardDisplay;
    public float maxWait = 3f; // Max time to wait for player tap before auto-advancing

    [Header("Chest Reference")]
    public Transform chestTransform;

    [Header("Summary UI")]
    public RewardSummaryUI summaryUI;

    private RewardBundle currentBundle;
    private bool isProcessing = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // When flash reaches full white → show rewards
        radialFlash.OnFlashPeak += OnFlashPeak;
    }

    // Called by RewardManager
    public void OpenChest(RewardBundle bundle)
    {
        currentBundle = bundle;

        chestPanel.SetActive(true);
        rewardsPanel.SetActive(false);

        isProcessing = false;
    }

    // Button click
    public void OnChestClicked()
    {
        if (isProcessing) return;
        isProcessing = true;

        // Stop idle animation
        if (idleAnimator != null)
            idleAnimator.StopIdle();

        Debug.Log("Chest clicked! Starting flash...");

        // Directly trigger radial flash
        radialFlash.PlayFlash(chestTransform.position);

    }

    // When flash fills screen (peak white)
    private void OnFlashPeak()
    {
        // Hide chest UI
        //chestPanel.SetActive(false);

        // Show rewards UI
        rewardsPanel.SetActive(true);

        // Start reward reveal
        StartCoroutine(ShowRewardsSequentially());
    }

    private IEnumerator ShowRewardsSequentially()
    {
        foreach (var reward in currentBundle.rewards)
        {
            nextPressed = false;

            rewardDisplay.Show(reward);

            float timer = 0f;
            maxWait = 3f;

            // Wait for tap OR timeout
            while (timer < maxWait && !nextPressed)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // Grant rewards AFTER showing
        RewardManager.Instance.GrantRewards(currentBundle.rewards);

        yield return new WaitForSeconds(0.5f);

        ShowSummary();

        // CloseRewardsPanel();
    }

    private void ShowSummary()
    {
        rewardsPanel.SetActive(false);

        summaryPanel.SetActive(true);
        summaryUI.ShowSummary(currentBundle);
    }

    public void OnNextPressed()
    {
        nextPressed = true;
    }

    public void OnSummaryOkPressed()
    {
        summaryPanel.SetActive(false);
        CloseRewardsPanel();
    }

    private void CloseRewardsPanel()
    {
        chestPanel.SetActive(false);
    }

}