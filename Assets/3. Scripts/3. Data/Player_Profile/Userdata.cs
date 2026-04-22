using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

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

    [Header("Progression")]
    public PlayerLevelData levelData;
    [SerializeField] private int xp = 0;
    [SerializeField, ReadOnly] public int playerLevel = 0;

    [Header("Daily Rewards")]
    public int CurrentDay = 1;
    public bool[] DayRewards = new bool[7];

    [Header("Home Ui")]
    public TMP_Text CoinTxt;
    public TMP_Text DiamondTxt;

    public Action<FactionName> OnFragmentsChanged;
    public Action<int> OnDiamondsChanged;
    public Action<int> OnMapShardsChanged;
    public Action<int> OnPlayerLevelChanged;
    public Action<int> OnXPChanged;

    public int XP
    {
        get => xp;
        set
        {
            xp = value;
            OnXPChanged?.Invoke(xp);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

    public int PlayerLevel
    {
        get => playerLevel;
        set
        {
            playerLevel = value;
            OnPlayerLevelChanged?.Invoke(playerLevel);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

    public int Diamonds
    {
        get => diamonds;
        set
        {
            diamonds = value;
            OnDiamondsChanged?.Invoke(diamonds);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }

    public int MapShards
    {
        get => mapShards;
        set
        {
            mapShards = value;
            OnMapShardsChanged?.Invoke(mapShards);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
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

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
    public int GetFragment(FactionName faction) => fragments[(int)faction];

    public void ResetData()
    {
        UserName = "";
        Birthday = "";
        ProfilePicture = null;
        GuestUser = false;

        //Level = 0;
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

