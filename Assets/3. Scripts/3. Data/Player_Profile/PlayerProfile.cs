using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerName;
    public int Rank;
    
    // Empty constructor for serialization
    public PlayerProfile() { }
    
    // Method to initialize with random data
    public void InitializeWithRandomData()
    {
        PlayerName = RandomNameGenerator.GetRandomName();
        Rank = Random.Range(1, 10); // Rank 1-9
    }
    
    public void LoadFromSaveData(string saveData)
    {
        if (!string.IsNullOrEmpty(saveData))
        {
            JsonUtility.FromJsonOverwrite(saveData, this);
        }
        else
        {
            InitializeWithRandomData();
        }
    }

    public void SaveToDisk()
    {
        Debug.Log($"[PlayerProfile] Saving PlayerName: {PlayerName}");
        PlayerPrefs.SetString("PLAYER_PROFILE", JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public static PlayerProfile LoadFromDisk()
    {
        string savedData = PlayerPrefs.GetString("PLAYER_PROFILE", "");
        Debug.Log($"[PlayerProfile] Loaded data: {savedData}");
        var profile = new PlayerProfile();
        if (!string.IsNullOrEmpty(savedData))
        {
            profile.LoadFromSaveData(savedData);
        }
        else
        {
            profile.InitializeWithRandomData();
            profile.SaveToDisk();
        }
        Debug.Log($"[PlayerProfile] Loaded PlayerName: {profile.PlayerName}");
        return profile;
    }

}