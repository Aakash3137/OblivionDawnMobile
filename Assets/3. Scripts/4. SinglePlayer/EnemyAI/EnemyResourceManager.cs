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
        startingResources[0].resourceCost = 100;
        startingResources[1].resourceCost = 100;
        startingResources[2].resourceCost = 100;
        startingResources[3].resourceCost = 100;
    }

    [Button]
    public void HackResources()
    {
        BuildCost[] resources = new BuildCost[4];
        startingResources[0].resourceCost = 999;
        startingResources[1].resourceCost = 999;
        startingResources[2].resourceCost = 999;
        startingResources[3].resourceCost = 999;
        resources[0].resourceCost = 999;
        resources[1].resourceCost = 999;
        resources[2].resourceCost = 999;
        resources[3].resourceCost = 999;

        SetResources(resources);
    }
    #endregion
}
