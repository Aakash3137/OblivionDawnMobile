
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Global Resource Tick")]
    [field: SerializeField] public float globalTickTime { get; private set; }

    [Header("Set Starting Resources")]
    public BuildCost[] startingResources;

    [Header("Current Resources"), ReadOnly]
    public BuildCost[] currentResources;

    [Header("Max Resources (Capacity)"), ReadOnly]
    public BuildCost[] maxResources;

    [Header("Generation Rates"), ReadOnly]
    public BuildCost[] currentGenerationRates;

    [Header("Resource Building Counts"), ReadOnly]
    public int[] resourceBuildingCounts;

    public Action OnResourcesChanged;
    public Action GlobalResourceTick;


    private void Start()
    {
        InitializeResources(startingResources);

        _ = StartGlobalTick();
    }

    private async Awaitable StartGlobalTick()
    {
        while (gameObject.activeInHierarchy)
        {
            await Awaitable.WaitForSecondsAsync(globalTickTime, destroyCancellationToken);
            GlobalResourceTick?.Invoke();
        }
    }

    #region  Helper Functions
    public void InitializeResources(BuildCost[] resources)
    {
        currentResources = new BuildCost[resources.Length];
        maxResources = new BuildCost[resources.Length];
        currentGenerationRates = new BuildCost[resources.Length];
        resourceBuildingCounts = new int[resources.Length];

        for (int i = 0; i < resources.Length; i++)
        {
            currentResources[i].resourceType = resources[i].resourceType;
            maxResources[i].resourceType = resources[i].resourceType;
            currentGenerationRates[i].resourceType = resources[i].resourceType;

            maxResources[i].resourceAmount = resources[i].resourceAmount;
            currentResources[i].resourceAmount = resources[i].resourceAmount;
        }

        ClampResources();
    }

    public void SetResources(BuildCost[] resources)
    {
        for (int i = 0; i < currentResources.Length; i++)
        {
            currentResources[i].resourceAmount = resources[i].resourceAmount;
            maxResources[i].resourceAmount = resources[i].resourceAmount;
        }

        ClampResources();
    }

    public void IncreaseResourcesCap(ScenarioResourceType resourceType, int amount = 0)
    {
        maxResources[(int)resourceType].resourceAmount += amount;
        ClampResources();
    }

    public void DecreaseResourcesCap(ScenarioResourceType resourceType, int amount = 0)
    {
        maxResources[(int)resourceType].resourceAmount -= amount;
        ClampResources();
    }

    public void AddResources(ScenarioResourceType resourceType, int amount = 0)
    {
        currentResources[(int)resourceType].resourceAmount += amount;
        ClampResources();
    }
    public void SpendResources(BuildCost[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            currentResources[i].resourceAmount -= resources[i].resourceAmount;
        }

        ClampResources();
    }

    public bool HasResources(BuildCost[] resources, bool debug = false)
    {
        bool hasAllResources = true;

        for (int i = 0; i < currentResources.Length; i++)
        {
            if (currentResources[i].resourceAmount - resources[i].resourceAmount < 0)
            {
                if (debug)
                    Debug.Log($"Not enough {currentResources[i].resourceType}, have {currentResources[i].resourceAmount}, need {resources[i].resourceAmount}");

                hasAllResources = false;
            }
        }

        return hasAllResources;
    }

    private void ClampResources()
    {
        for (int i = 0; i < currentResources.Length; i++)
        {
            currentResources[i].resourceAmount = Mathf.Clamp(currentResources[i].resourceAmount, 0, maxResources[i].resourceAmount);
        }

        OnResourcesChanged?.Invoke();
    }

    #region Resource Generation
    public void IncreaseResourceGenerationRate(ScenarioResourceType resourceType, float amount = 0)
    {
        currentGenerationRates[(int)resourceType].resourceAmount += (int)amount;
        OnResourcesChanged?.Invoke();
    }

    public void IncreaseResourceGenerationRate(BuildCost[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            currentGenerationRates[i].resourceAmount += resources[i].resourceAmount;
        }

        OnResourcesChanged?.Invoke();
    }

    public void DecreaseResourceGenerationRate(ScenarioResourceType resourceType, float amount = 0)
    {
        currentGenerationRates[(int)resourceType].resourceAmount -= (int)amount;
        OnResourcesChanged?.Invoke();
    }

    public void DecreaseResourceGenerationRate(BuildCost[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            currentGenerationRates[i].resourceAmount -= resources[i].resourceAmount;
        }

        OnResourcesChanged?.Invoke();
    }
    #endregion

    public bool CanAddResource(ScenarioResourceType type, int amount)
    {
        return currentResources[(int)type].resourceAmount + amount <= maxResources[(int)type].resourceAmount;
    }

    public void IncrementResourceBuildingCount(ScenarioResourceType resourceType)
    {
        resourceBuildingCounts[(int)resourceType]++;
        OnResourcesChanged?.Invoke();
    }

    public void DecrementResourceBuildingCount(ScenarioResourceType resourceType)
    {
        resourceBuildingCounts[(int)resourceType]--;
        OnResourcesChanged?.Invoke();
    }

    void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

        startingResources = BuildCostUtils.ResizePreservingData(startingResources, enumValues.Length);

        for (int i = 0; i < startingResources.Length; i++)
            startingResources[i].resourceType = (ScenarioResourceType)enumValues.GetValue(i);
    }

    [Button("DEBUG: Add Resources")]
    public void AddResources(int amount)
    {
        for (int i = 0; i < currentResources.Length; i++)
        {
            currentResources[i].resourceAmount += amount;
        }

        ClampResources();
    }
    #endregion
}
