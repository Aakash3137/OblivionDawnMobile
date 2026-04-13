using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    [ReadOnly] public ScenarioResourceType resourceType { get; private set; }
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }
    [field: SerializeField, ReadOnly] public bool isProducingResources { get; private set; }
    public override TileEffectType compatibleTileEffectType => TileEffectType.ResourceTile;
    private float productionTickSyncTime;
    private int generationAmount;
    private float generationRate;

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

        generationAmount = resourceBuildingData.resourceAmountPerBatch;
        generationRate = generationAmount;
    }

    internal override async Awaitable InitializeOnBuilt()
    {
        isProducingResources = false;
        await base.InitializeOnBuilt();

        IncreaseGlobalCapacity();
        rmInstance.IncrementResourceBuildingCount(resourceType);
        rmInstance.GlobalResourceTick += StartProducing;
    }

    private void StartProducing()
    {
        if (!isProducingResources)
            productionTickSyncTime = Time.time;

        bool canProduce = CanProduce();
        bool syncComplete = Time.time - productionTickSyncTime >= rmInstance.globalTickTime;

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

        if (syncComplete)
            Produce();

        // calling CanProduce method instead of cached value to refresh it
        if (!CanProduce() && isProducingResources)
        {
            DecreaseGenerationRate();
            isProducingResources = false;
        }
    }

    private void Produce()
    {
        rmInstance.AddResources(resourceType, generationAmount);
    }

    private bool CanProduce()
    {
        return rmInstance.CanAddResource(resourceType, resourceBuildingData.resourceAmountPerBatch);
    }

    private void IncreaseGenerationRate()
    {
        rmInstance.IncreaseResourceGenerationRate(resourceType, generationRate);
    }

    private void DecreaseGenerationRate()
    {
        rmInstance.DecreaseResourceGenerationRate(resourceType, generationRate);
    }

    private void IncreaseGlobalCapacity()
    {
        rmInstance.IncreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
    }

    private void DecreaseGlobalCapacity()
    {
        rmInstance.DecreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
    }
    public void BuffResourceProduction(float buffStrength)
    {
        generationAmount = generationAmount * (int)buffStrength;
        generationRate = generationAmount;
    }

    internal override void Die()
    {
        base.Die();

        if (hasBuilt)
        {
            DecreaseGlobalCapacity();
            rmInstance.DecrementResourceBuildingCount(resourceType);
        }
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
