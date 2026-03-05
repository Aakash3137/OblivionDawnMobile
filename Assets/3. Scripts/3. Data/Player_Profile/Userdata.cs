using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "Userdata", menuName = "Userdata", order = 0)]
public class Userdata : ScriptableObject 
{
    [Header ("Profile Information")]
    public string UserName = "";
    public string Birthday = "";
    public Sprite ProfilePicture;
    public Sprite defaultProfilePicture;
    public bool GuestUser = false;

    [Header ("Game Data")]
    [SerializeField] private int coins = 0;
    [SerializeField] private int diamonds = 0;
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

    public void ResetData()
    {
        UserName ="";
        Birthday = "";
        ProfilePicture = null;
        GuestUser = false;

        Level = 0;
        Coins = 0;
        Diamonds = 0;
        CurrentDay = 1;
        
    }
    
}

