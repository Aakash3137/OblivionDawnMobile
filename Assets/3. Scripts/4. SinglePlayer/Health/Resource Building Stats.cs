using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceBuildingStats : BuildingStats
{
    [ReadOnly] public ScenarioResourceType resourceType { get; private set; }
    public ResourceBuildingUpgradeData resourceBuildingData { get; private set; }

    [field: SerializeField, ReadOnly] public bool isProducing { get; private set; }
    // [field: SerializeField, ReadOnly] public int generatedResourceAmount;
    private PlayerResourceManager prmInstance;
    private EnemyResourceManager ermInstance;
    private WaitForSeconds waitTime;

    private BuildingSkeleton buildingSkeleton;

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
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing ResourceBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();

        isProducing = false;

    }

    private void StartProducing()
    {
        IncreaseGlobalCapacity();
        StartCoroutine(StartResourceGeneration());
    }

    /// <summary>
    /// This coroutine will start generating resources for the building at the specified interval.
    /// If the building is destroyed, the coroutine will end.
    /// If the building's resource generation rate changes, the coroutine will adjust to the new rate.
    /// </summary>
    private IEnumerator StartResourceGeneration()
    {
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

            if (canProduce)
            {
                Produce();
            }
        }

        if (wasProducing)
            DecreaseGenerationRate();
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
        DecreaseGlobalCapacity();
        DecreaseGenerationRate();

        base.Die();

        KillCounterManager.Instance.AddResourceBuildingDestroyedData(resourceType, side);
    }

    internal override void OnDestroy()
    {
        base.OnDestroy();
    }
    private void OnEnable()
    {
        if (buildingSkeleton == null)
            buildingSkeleton = GetComponent<BuildingSkeleton>();

        buildingSkeleton.onBuildingBuilt += StartProducing;
    }

    private void OnDisable()
    {
        if (buildingSkeleton != null)
            buildingSkeleton.onBuildingBuilt -= StartProducing;
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
