using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    [ReadOnly] public ScenarioResourceType resourceType { get; private set; }
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }
    [field: SerializeField, ReadOnly] public bool isProducingResources { get; private set; }
    private WaitForSeconds waitTime;
    private float productionTickSyncTime;
    private float generationTime;

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        if (buildingStatsSO is ResourceBuildingDataSO resourceBuildingSO)
        {
            resourceType = resourceBuildingSO.resourceType;
            resourceBuildingData = resourceBuildingSO.resourceBuildingUpgradeData[identity.spawnLevel];

            basicStats = resourceBuildingData.buildingBasicStats;
            buildCost = resourceBuildingSO.buildingBuildCost;
            // waitTime = new WaitForSeconds(resourceBuildingData.resourceTimeToProduce);

            buildTime = resourceBuildingData.buildingBuildTime;
            // buildingWaitTime = new WaitForSeconds(buildTime);
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing ResourceBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();

        generationTime = resourceBuildingData.resourceAmountPerBatch; // rmInstance.globalTickTime;
    }

    internal override async Awaitable InitializeOnBuilt()
    {
        isProducingResources = false;
        await base.InitializeOnBuilt();

        IncreaseGlobalCapacity();
        productionTickSyncTime = Time.time;
        rmInstance.GlobalResourceTick += StartProducing;
    }

    private void StartProducing()
    {
        bool canProduce = CanProduce();

        if (canProduce && !isProducingResources)
        {
            IncreaseGenerationRate();
            isProducingResources = true;
        }
        else if (!canProduce && isProducingResources)
        {
            DecreaseGenerationRate();
            isProducingResources = false;
        }

        if (Time.time - productionTickSyncTime >= rmInstance.globalTickTime)
            Produce();

        if (!canProduce && isProducingResources)
        {
            DecreaseGenerationRate();
            isProducingResources = false;
        }
    }

    // private IEnumerator StartResourceGeneration()
    // {
    //     isProducing = false;
    //     yield return buildingWaitTime;

    //     IncreaseGlobalCapacity();
    //     bool wasProducing = false;

    //     while (currentHealth > 0)
    //     {
    //         bool canProduce = CanProduce();

    //         if (canProduce && !wasProducing)
    //         {
    //             IncreaseGenerationRate();
    //             isProducing = true;
    //             wasProducing = true;
    //         }

    //         if (!canProduce && wasProducing)
    //         {
    //             DecreaseGenerationRate();
    //             isProducing = false;
    //             wasProducing = false;
    //             yield return new WaitUntil(CanProduce);
    //             continue;
    //         }

    //         yield return waitTime;

    //         // if (canProduce)            
    //         Produce();
    //     }
    // }


    private void Produce()
    {
        rmInstance.AddResources(resourceType, resourceBuildingData.resourceAmountPerBatch);

    }

    private bool CanProduce()
    {
        return rmInstance.CanAddResource(resourceType, resourceBuildingData.resourceAmountPerBatch);
    }

    private void IncreaseGenerationRate()
    {
        rmInstance.IncreaseResourceGenerationRate(resourceType, generationTime);
    }

    private void DecreaseGenerationRate()
    {
        rmInstance.DecreaseResourceGenerationRate(resourceType, generationTime);
    }

    private void IncreaseGlobalCapacity()
    {
        rmInstance.IncreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
    }

    private void DecreaseGlobalCapacity()
    {
        rmInstance.DecreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
    }

    internal override void Die()
    {
        base.Die();

        if (hasBuilt)
            DecreaseGlobalCapacity();
        if (hasBuilt && isProducingResources)
            DecreaseGenerationRate();

        rmInstance.GlobalResourceTick -= StartProducing;

        KillCounterManager.Instance.AddResourceBuildingDestroyedData(resourceType, side);
    }

    public float GetGenerationTime()
    {
        return rmInstance.globalTickTime;
    }

    public ResourceBuildingDataSO GetBuildingSO()
    {
        return buildingStatsSO as ResourceBuildingDataSO;
    }
}
