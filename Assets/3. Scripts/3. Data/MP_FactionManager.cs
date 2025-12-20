using UnityEngine;

public class MP_FactionManager : MonoBehaviour
{
    [Header("All Factions")]
    public MP_Faction[] allFactions;

    public static MP_Faction LocalFaction;

    void Start()
    {
        //LoadFaction();
    }

    void LoadFaction()
    {
        foreach (var faction in allFactions)
        {
            if (faction.factionName == GameData.SelectedFactionName)
            {
                LocalFaction = faction;
                Debug.Log("Loaded faction: " + faction.factionName);
                return;
            }
        }

        // if faction not found in this list, log an error
        Debug.LogError("Faction not found: " + GameData.SelectedFactionName);
    }
}