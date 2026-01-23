using System.Collections;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    [Space(30)]
    public ScenarioResourceType resourceType;
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }

    public bool autoProduce = true;
    private bool isProducing;
    private PlayerResourceManager prmInstance;
    private WaitForSeconds generateTime;
    [SerializeField] private int GeneratedResourceAmount;

    private ResourceProgress resourceProgress;

    internal override void Start()
    {
        prmInstance = PlayerResourceManager.Instance;
        resourceProgress = GetComponentInChildren<ResourceProgress>();

        if (buildingStats is ResourceBuildingDataSO resourceBuildingSO)
        {
            resourceType = resourceBuildingSO.resourceType;
            resourceBuildingData = resourceBuildingSO.resourceBuildingUpgradeData[level];
            buildCost = resourceBuildingSO.buildingBuildCost;
            generateTime = new WaitForSeconds(resourceBuildingData.resourceTimeToProduce);
            basicStats = resourceBuildingData.buildingBasicStats;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing ResourceBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();

        StartCoroutine(StartResourceGeneration(resourceBuildingData.resourceAmountPerBatch, generateTime));
    }

    private IEnumerator StartResourceGeneration(int amount, WaitForSeconds delay)
    {
        if (side != Side.Player)
            yield break;

        isProducing = true;

        prmInstance.SetResourceGenerationRate(resourceType, resourceBuildingData.resourceGenerationRate);

        while (autoProduce && currentHealth > 0)
        {
            //resourceProgress.UIResourceProgress();
            yield return delay;
            prmInstance.AddResources(resourceType, amount);
            GeneratedResourceAmount += amount;
            // Debug.Log($"Generated {ResourceName}. Current amount: {GeneratedResourceAmount}");
        }
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();
        isProducing = false;
        prmInstance.SetResourceGenerationRate(resourceType, -resourceBuildingData.resourceGenerationRate);

        KillCounterManager.Instance.AddResourceBuildingDestroyedData(resourceType, side);
    }
}
