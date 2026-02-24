using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    [ReadOnly] public ScenarioResourceType resourceType { get; private set; }
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }
    [field: SerializeField, ReadOnly] public bool isProducing { get; private set; }

    private PlayerResourceManager prmInstance;
    private EnemyResourceManager ermInstance;
    private WaitForSeconds waitTime;

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        prmInstance = PlayerResourceManager.Instance;
        ermInstance = EnemyResourceManager.Instance;

        if (buildingStatsSO is ResourceBuildingDataSO resourceBuildingSO)
        {
            resourceType = resourceBuildingSO.resourceType;
            resourceBuildingData = resourceBuildingSO.resourceBuildingUpgradeData[identity.spawnLevel];

            basicStats = resourceBuildingData.buildingBasicStats;
            buildCost = resourceBuildingSO.buildingBuildCost;
            waitTime = new WaitForSeconds(resourceBuildingData.resourceTimeToProduce);

            buildTime = resourceBuildingData.buildingBuildTime;
            buildingWaitTime = new WaitForSeconds(buildTime);
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing ResourceBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();


        StartProducing();
    }

    private void StartProducing()
    {
        StartCoroutine(StartResourceGeneration());
    }

    private IEnumerator StartResourceGeneration()
    {
        isProducing = false;
        yield return buildingWaitTime;

        IncreaseGlobalCapacity();
        bool wasProducing = false;

        while (currentHealth > 0)
        {
            bool canProduce = CanProduce();

            if (canProduce && !wasProducing)
            {
                IncreaseGenerationRate();
                isProducing = true;
                wasProducing = true;
            }

            if (!canProduce && wasProducing)
            {
                DecreaseGenerationRate();
                isProducing = false;
                wasProducing = false;
                yield return new WaitUntil(CanProduce);
                continue;
            }

            yield return waitTime;

            // if (canProduce)            
            Produce();
        }
    }


    private void Produce()
    {
        switch (side)
        {
            case Side.Player:
                prmInstance.AddResources(resourceType, resourceBuildingData.resourceAmountPerBatch);
                break;
            case Side.Enemy:
                ermInstance.AddResources(resourceType, resourceBuildingData.resourceAmountPerBatch);
                break;
        }
    }

    private bool CanProduce()
    {
        switch (side)
        {
            case Side.Player:
                return prmInstance.CanAddResource(resourceType, resourceBuildingData.resourceAmountPerBatch);
            case Side.Enemy:
                return ermInstance.CanAddResource(resourceType, resourceBuildingData.resourceAmountPerBatch);
        }
        return false;
    }

    private void IncreaseGenerationRate()
    {
        switch (side)
        {
            case Side.Player:
                prmInstance.IncreaseResourceGenerationRate(resourceType, resourceBuildingData.resourceGenerationRate);
                break;
            case Side.Enemy:
                ermInstance.IncreaseResourceGenerationRate(resourceType, resourceBuildingData.resourceGenerationRate);
                break;
        }
    }

    private void DecreaseGenerationRate()
    {
        isProducing = false;
        if (side == Side.Player)
            prmInstance.DecreaseResourceGenerationRate(resourceType, resourceBuildingData.resourceGenerationRate);
        else if (side == Side.Enemy)
            ermInstance.DecreaseResourceGenerationRate(resourceType, resourceBuildingData.resourceGenerationRate);
    }

    private void IncreaseGlobalCapacity()
    {
        switch (side)
        {
            case Side.Player:
                prmInstance.IncreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
                break;
            case Side.Enemy:
                ermInstance.IncreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
                break;
        }
    }

    private void DecreaseGlobalCapacity()
    {
        switch (side)
        {
            case Side.Player:
                prmInstance.DecreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
                break;
            case Side.Enemy:
                ermInstance.DecreaseResourcesCap(resourceType, resourceBuildingData.resourceAmountCapacity);
                break;
        }
    }

    internal override void Die()
    {
        base.Die();

        DecreaseGlobalCapacity();
        if (isProducing)
            DecreaseGenerationRate();
        KillCounterManager.Instance.AddResourceBuildingDestroyedData(resourceType, side);
    }

    public float GetGenerationTime()
    {
        return resourceBuildingData.resourceTimeToProduce;
    }

    public ResourceBuildingDataSO GetBuildingSO()
    {
        return buildingStatsSO as ResourceBuildingDataSO;
    }
}
