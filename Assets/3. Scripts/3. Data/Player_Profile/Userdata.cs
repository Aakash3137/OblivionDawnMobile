using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;


// [SerializeField] private PlayerLevelData playerLevelData;
// [SerializeField] private Userdata userData;

// public void LevelUp()
// {
//     userData.Level++;

//     var bundle = playerLevelData.GetRewardForLevel(userData.Level);

//     if (bundle != null)
//     {
//         RewardManager.Instance.ClaimReward(bundle);
//     }
// }

[CreateAssetMenu(fileName = "Userdata", menuName = "Userdata", order = 0)]
public class Userdata : ScriptableObject
{
    [Header("Profile Information")]
    public string UserName = "";
    public string Birthday = "";
    public Sprite ProfilePicture;
    public Sprite defaultProfilePicture;
    public bool GuestUser = false;

    [Header("Game Data")]
    [SerializeField] private int coins = 0;
    [SerializeField] private int diamonds = 0;
    [SerializeField] private int mapShards = 0;
    [SerializeField, ReadOnly] private int[] fragments = new int[4];

    public Action<FactionName> OnFragmentsChanged;
    public Action OnDiamondsChanged;
    public Action OnMapShardsChanged;

    public int Level = 0;
    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
            UpdateCurrencyUI();
        }
    }

    public int Diamonds
    {
        get => diamonds;
        set
        {
            diamonds = value;
            UpdateCurrencyUI();
            OnDiamondsChanged?.Invoke();
        }
    }

    public int MapShards
    {
        get => mapShards;
        set
        {
            mapShards = value;
            UpdateCurrencyUI();
            OnMapShardsChanged?.Invoke();
        }
    }

    [Header("Daily Rewards")]
    public int CurrentDay = 1;
    public bool[] DayRewards = new bool[7];

    [Header("Home Ui")]
    public TMP_Text CoinTxt;
    public TMP_Text DiamondTxt;

    void UpdateCurrencyUI()
    {
        if (CoinTxt != null)
            CoinTxt.text = Coins.ToString();

        if (DiamondTxt != null)
            DiamondTxt.text = Diamonds.ToString();
    }
    [Button]
    public void AddFragments(FactionName faction, int amount)
    {
        fragments[(int)faction] += amount;

        OnFragmentsChanged?.Invoke(faction);
    }
    public void ConsumeFragments(FactionName faction, int amount)
    {
        fragments[(int)faction] -= amount;

        OnFragmentsChanged?.Invoke(faction);
    }
    public int GetFragment(FactionName faction) => fragments[(int)faction];

    public void ResetData()
    {
        UserName = "";
        Birthday = "";
        ProfilePicture = null;
        GuestUser = false;

        Level = 0;
        Coins = 0;
        Diamonds = 0;
        CurrentDay = 1;
        fragments = new int[4];

    }

    public bool CheckDay()
    {
        bool CheckBool = false;
        foreach (bool day in DayRewards)
        {
            if (day)
            {
                CheckBool = true;
            }
            else
            {
                CheckBool = false;
                return false;
            }
        }
        return CheckBool;
    }

}

