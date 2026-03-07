using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<LevelInfo> levels;
    
    [SerializeField] private int PlayerXP;

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
            _Box.LevelNoTxt.text = i.ToString();
            _Box.gameObject.name = "Level_" + i.ToString();
            if(levels[i].XP_Required < temp)
            {
                temp -= levels[i].XP_Required;
                _Box.FillImage.fillAmount = 1;
            }
            else
            {
                _Box.FillImage.fillAmount = 0;
            }

            foreach (var item in levels[i].Rewards)
            {
                RewardBox R_Box = _Box.SetReward(item, _Box.transform);
            }
                
        }
    }
}

[SerializeField]
public class LevelInfo
{
    public bool IsLocked;
    public int XP_Required;
    public List<RewardData> Rewards;
}

[SerializeField]
public class RewardData
{
    public int RewardAmount;
    public Sprite RewardIcon;
    public RewardItem RewardType;
    public bool RewardStatus;
}

