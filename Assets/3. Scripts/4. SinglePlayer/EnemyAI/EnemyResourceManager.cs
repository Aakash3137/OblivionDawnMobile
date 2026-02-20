using Sirenix.OdinInspector;

public class EnemyResourceManager : ResourceManager
{
    public static EnemyResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #region  ContextMenu
    [Button]
    public void ResetResources()
    {
        startingResources[0].resourceAmount = 100;
        startingResources[1].resourceAmount = 100;
        startingResources[2].resourceAmount = 100;
        startingResources[3].resourceAmount = 100;
    }

    [Button]
    public void HackResources()
    {
        BuildCost[] resources = new BuildCost[4];
        startingResources[0].resourceAmount = 999;
        startingResources[1].resourceAmount = 999;
        startingResources[2].resourceAmount = 999;
        startingResources[3].resourceAmount = 999;
        resources[0].resourceAmount = 999;
        resources[1].resourceAmount = 999;
        resources[2].resourceAmount = 999;
        resources[3].resourceAmount = 999;

        SetResources(resources);
    }
    #endregion
}
