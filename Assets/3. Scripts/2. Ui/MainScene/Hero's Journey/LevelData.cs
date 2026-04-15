using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<XP_Required> xpRequirements;
    [SerializeField] private PlayerLevelData playerLevelData;
    [SerializeField] private Userdata _Data;

    public int _XP;

    [Serializable]
    public class XP_Required
    {
        public int level;
        public int requiredXP;

    }

    public int PlayerXP
    {
        get => _XP;
        set
        {
            _XP = value;
            SetLevel();
        }
    }

    public void SetLevel()
    {
        int currentLevel = 0;

        for (int i = 0; i < xpRequirements.Count; i++)
        {
            if (xpRequirements[i].requiredXP <= PlayerXP)
                currentLevel++;
        }

        _Data.Level = currentLevel;
    }

    public void SetXP(int amount)
    {
        PlayerXP += amount;
    }

    public void GenrateLevel(LevelBox prefab, Transform parent)
    {
        for (int i = 0; i < xpRequirements.Count; i++)
        {
            int level = i + 1;

            LevelBox box = Instantiate(prefab, parent);
            box.LevelNoTxt.text = level.ToString();
            box.gameObject.name = "Level_" + level;

            bool isLocked = level > _Data.Level;
            box.LockImage.gameObject.SetActive(isLocked);

            var entry = playerLevelData.GetEntry(level);
            if (entry == null || entry.rewardBundle == null)
                continue;

            foreach (var reward in entry.rewardBundle.rewards)
            {
                RewardBox rBox = box.SetReward(reward, box.transform);

                bool isClaimed = entry.isClaimed;

                rBox.ClaimButton.interactable = !isLocked && !isClaimed;

                int capturedLevel = level; // fix closure issue

                rBox.ClaimButton.onClick.AddListener(() =>
                {
                    OnClaimReward(capturedLevel, entry, rBox);
                });
            }
        }
    }

    private void OnClaimReward(int level, LevelRewardEntry entry, RewardBox box)
    {
        if (entry.isClaimed)
            return;

        Debug.Log($"Claiming reward for level {level}");

        // Trigger reward flow (UI + grant handled inside)
        RewardManager.Instance.ClaimReward(entry.rewardBundle);

        // Mark claimed AFTER triggering
        entry.isClaimed = true;
        box.ClaimButton.interactable = false;
    }
}