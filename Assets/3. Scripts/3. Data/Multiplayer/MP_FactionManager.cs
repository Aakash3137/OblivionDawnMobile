using UnityEngine;

public class MP_FactionManager : MonoBehaviour
{
    [Header("All Factions")]
    public MP_Faction[] allFactions;

    public static MP_Faction LocalFaction;
    
    private static MP_FactionManager instance;
    public static MP_FactionManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        /*else
        {
            Destroy(gameObject);
        }*/
    }

    void Start()
    {
        LoadFaction();
    }

    public void LoadFaction()
    {
        if (string.IsNullOrEmpty(GameData.SelectedFactionName))
        {
            Debug.LogWarning("[MP_FactionManager] No faction name set in GameData");
            return;
        }
        
        foreach (var faction in allFactions)
        {
            if (faction.factionName == GameData.SelectedFactionName)
            {
                LocalFaction = faction;
                GameData.SelectedMPFaction = faction;
                Debug.Log($"[MP_FactionManager] Loaded faction: {faction.factionName}");
                return;
            }
        }

        Debug.LogError($"[MP_FactionManager] Faction not found: {GameData.SelectedFactionName}");
    }
    
    public MP_Faction GetFactionByName(string factionName)
    {
        foreach (var faction in allFactions)
        {
            if (faction.factionName == factionName)
                return faction;
        }
        return null;
    }
}