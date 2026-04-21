using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBox : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] internal TMP_Text LevelNoTxt;
    [SerializeField] internal Image LockImage;
    [SerializeField] internal Button ClaimButton;

    private int level;
    private bool isLocked;
    private LevelRewardEntry rewardEntry;

    public void Init(int levelNumber, bool locked, LevelRewardEntry entry)
    {
        level = levelNumber;
        isLocked = locked;
        rewardEntry = entry;

        LevelNoTxt.text = level.ToString();
        LockImage.gameObject.SetActive(isLocked);

        bool isClaimed = entry != null && entry.isClaimed;

        ClaimButton.interactable = !isLocked && !isClaimed;

        ClaimButton.onClick.RemoveAllListeners();
        ClaimButton.onClick.AddListener(OnClaimClicked);
    }

    private void OnClaimClicked()
    {
        if (rewardEntry == null || rewardEntry.isClaimed)
            return;

        Debug.Log($"Claiming reward for level {level}");

        // Trigger reward system
        RewardManager.Instance.ClaimReward(rewardEntry.rewardBundle);

        // Mark claimed
        rewardEntry.isClaimed = true;
        ClaimButton.interactable = false;
    }
}