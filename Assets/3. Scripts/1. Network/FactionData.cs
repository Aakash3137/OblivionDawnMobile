/*[CreateAssetMenu(menuName = "Assets/Faction")]
public class FactionData : ScriptableObject
{
    public FactionType factionType;

    [System.Serializable]
    public class BuildingEntry
    {
        public BuildType buildType;
        public GameObject prefab;
        public BuildingStats stats;
    }

    public BuildingEntry[] buildings;

    public BuildingEntry Get(BuildType type)
    {
        foreach (var b in buildings)
            if (b.buildType == type)
                return b;

        return null;
    }
}*/