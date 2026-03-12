using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<LevelInfo> levels;
    [SerializeField] private Sprite LevelDetails;
    public int _XP;
    public int PlayerXP
    {
        get => _XP;
        set
        {
            _XP = value; 
            SetLevel();
        }
    }
    [SerializeField] private Userdata _Data;

    public void SetLevel()
    {
        int temp =0;
        foreach (var level in levels)
        {
            if(level.XP_Required <= PlayerXP)
            {
                level.IsLocked = false;
                temp += 1;
            }
            else
            {
                level.IsLocked = true;
            }
        }

        _Data.Level = temp;
    }

    public void SetXP(int Amount)
    {
        PlayerXP += Amount;
    }

    public void GenrateLevel(LevelBox _Prefab, Transform _Parent)
    {
        int temp = PlayerXP;
        for(int i = 0; i < levels.Count; i++)
        {
            LevelBox _Box = Instantiate(_Prefab, _Parent);
            _Box.LevelNoTxt.text = (i + 1).ToString();
            _Box.gameObject.name = "Level_" + (i + 1).ToString();

            if(levels[i].IsLocked)
            {
                _Box.LockImage.gameObject.SetActive(true);
            }
            else
            {
                _Box.LockImage.gameObject.SetActive(false);
            }

            foreach (var item in levels[i].Rewards)
            {
                RewardBox R_Box = _Box.SetReward(item, _Box.transform);
                R_Box.ClaimButton.interactable = !levels[i].IsLocked;
                item._Box = R_Box;
                R_Box.ClaimButton.onClick.AddListener(() => 
                {
                    OnClaimReward(item);
                });
            }
        }
    }

    private void OnClaimReward(RewardData reward)
    {
        if(reward.RewardStatus)
            return;

        reward.RewardStatus = true;
        reward._Box.ClaimButton.interactable = false;
        if(reward.RewardType == RewardItem.Gems)
        {
            _Data.Diamonds += reward.RewardAmount;
        }
        else if(reward.RewardType == RewardItem.Fragments)
        {
            _Data.Coins += reward.RewardAmount;
        }
    }
}
 
[Serializable]
public class LevelInfo
{
    public bool IsLocked;
    public int XP_Required;
    public List<RewardData> Rewards;
}

[Serializable]
public class RewardData
{
    public int RewardAmount;
    public Sprite RewardIcon;
    public RewardItem RewardType;
    public bool RewardStatus;
    public RewardBox _Box;
}

