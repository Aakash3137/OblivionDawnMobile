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
    public int Level = 0;
    public int Coins = 0;
    public int Diamonds = 0;

    [Header("Daily Rewards")]
    public int CurrentDay = 1;
    public bool[] DayRewards = new bool[7];

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

