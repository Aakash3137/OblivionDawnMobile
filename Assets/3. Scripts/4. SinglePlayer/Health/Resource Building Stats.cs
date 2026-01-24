using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    public ScenarioResourceType resourceType { get; private set; }
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }

    public bool autoProduce { get; private set; }
    private PlayerResourceManager prmInstance;
    private WaitForSeconds waitTime;
    [ReadOnly]
    public int GeneratedResourceAmount;

    internal override void Start()
    {
        identity = buildingStats.buildingIdentity;

        autoProduce = true;

        prmInstance = PlayerResourceManager.Instance;

        if (buildingStats is ResourceBuildingDataSO resourceBuildingSO)
        {
            resourceType = resourceBuildingSO.resourceType;
            resourceBuildingData = resourceBuildingSO.resourceBuildingUpgradeData[identity.spawnLevel];

            basicStats = resourceBuildingData.buildingBasicStats;

            buildCost = resourceBuildingSO.buildingBuildCost;
            waitTime = new WaitForSeconds(resourceBuildingData.resourceTimeToProduce);
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing ResourceBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();

        StartCoroutine(StartResourceGeneration(resourceBuildingData.resourceAmountPerBatch, waitTime));
    }

    private IEnumerator StartResourceGeneration(int amount, WaitForSeconds delay)
    {
        if (side != Side.Player)
            yield break;

        prmInstance.SetResourceGenerationRate(resourceType, resourceBuildingData.resourceGenerationRate);

        while (autoProduce)//&& currentHealth > 0)
        {
            yield return delay;
            prmInstance.AddResources(resourceType, amount);
            GeneratedResourceAmount += amount;
            // Debug.Log($"Generated {ResourceName}. Current amount: {GeneratedResourceAmount}");
        }
    }

    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddResourceBuildingDestroyedData(resourceType, side);
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();
        prmInstance.SetResourceGenerationRate(resourceType, -resourceBuildingData.resourceGenerationRate);
    }

    public float GetGenerationTime()
    {
        return resourceBuildingData.resourceTimeToProduce;
    }
}
