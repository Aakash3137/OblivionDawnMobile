using UnityEngine;


[CreateAssetMenu(fileName = "Userdata", menuName = "Userdata", order = 0)]
public class Userdata : ScriptableObject 
{
    public string UserName = "";
    public int Level = 0;
    public int Coins = 0;
    public int Diamonds = 0;
    
    public bool GuestUser = false;
}

