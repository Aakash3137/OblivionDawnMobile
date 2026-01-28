using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    public ScenarioResourceType resourceType { get; private set; }
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }

    public bool autoProduce { get; private set; }
    private PlayerResourceManager prmInstance;
    private EnemyResourceManager ermInstance;
    private WaitForSeconds waitTime;
    [ReadOnly]
    public int GeneratedResourceAmount;

    internal override void Start()
    {
        identity = buildingStats.buildingIdentity;

        autoProduce = true;

        prmInstance = PlayerResourceManager.Instance;
        ermInstance = EnemyResourceManager.Instance;

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
        if (side == Side.Player)
        {
            prmInstance.SetResourceGenerationRate(
                resourceType,
                resourceBuildingData.resourceGenerationRate
            );
        }
        else if (side == Side.Enemy)
        {
            ermInstance.SetResourceGenerationRate(
                resourceType,
                resourceBuildingData.resourceGenerationRate
            );
        }
        else
        {
            yield break;
        }
        while (autoProduce)//&& currentHealth > 0)
        {
            yield return delay;
            if (side == Side.Player)
            {
                prmInstance.AddResources(resourceType, amount);
            }
            else if (side == Side.Enemy)
            {
                ermInstance.AddResources(resourceType, amount);
            }
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
        if (side == Side.Player)
        {
            prmInstance.SetResourceGenerationRate
            (
                resourceType, -resourceBuildingData.resourceGenerationRate
            );
        }
        else if (side == Side.Enemy)
        {
            ermInstance.SetResourceGenerationRate
            (
                resourceType, -resourceBuildingData.resourceGenerationRate
            );
        }
    }

    public float GetGenerationTime()
    {
        return resourceBuildingData.resourceTimeToProduce;
    }
}
