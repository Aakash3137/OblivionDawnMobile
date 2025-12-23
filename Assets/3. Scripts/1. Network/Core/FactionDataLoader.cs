using UnityEngine;

/// <summary>
/// Ensures faction data is loaded and available in the game scene
/// </summary>
public class FactionDataLoader : MonoBehaviour
{
    [Header("Faction Manager Reference")]
    public MP_FactionManager factionManager;

    private void Awake()
    {
        if (factionManager == null)
            factionManager = FindObjectOfType<MP_FactionManager>();
            
        LoadFactionData();
    }

    private void LoadFactionData()
    {
        if (string.IsNullOrEmpty(GameData.SelectedFactionName))
        {
            Debug.LogWarning("[FactionDataLoader] No faction selected, using default");
            return;
        }

        if (GameData.SelectedMPFaction != null)
        {
            Debug.Log($"[FactionDataLoader] Faction already loaded: {GameData.SelectedMPFaction.factionName}");
            return;
        }

        if (factionManager == null || factionManager.allFactions == null)
        {
            Debug.LogError("[FactionDataLoader] FactionManager or factions not found!");
            return;
        }

        foreach (var faction in factionManager.allFactions)
        {
            if (faction.factionName == GameData.SelectedFactionName)
            {
                GameData.SelectedMPFaction = faction;
                Debug.Log($"[FactionDataLoader] Loaded faction: {faction.factionName}");
                return;
            }
        }

        Debug.LogError($"[FactionDataLoader] Faction not found: {GameData.SelectedFactionName}");
    }
}
