
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Set Starting Resources")]
    public BuildCost[] startingResources;

    [Header("Current Resources"), ReadOnly]
    public BuildCost[] currentResources;

    [Header("Max Resources (Capacity)"), ReadOnly]
    public BuildCost[] maxResources;

    [Header("Generation Rates"), ReadOnly]
    public BuildCost[] currentGenerationRates;

    public Action OnResourcesChanged;


    private void Start()
    {
        InitializeResources(startingResources);
    }

    public void InitializeResources(BuildCost[] resources)
    {
        currentResources = new BuildCost[resources.Length];
        maxResources = new BuildCost[resources.Length];
        currentGenerationRates = new BuildCost[resources.Length];

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
                    Debug.Log($"<color=#C6616B>Not enough {currentResources[i].resourceType}, Need {currentResources[i].resourceAmount - resources[i].resourceAmount} more {currentResources[i].resourceType}</color>");
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
        ClampResources();
    }

    public void IncreaseResourceGenerationRate(BuildCost[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            currentGenerationRates[i].resourceAmount += resources[i].resourceAmount;
        }
        ClampResources();
    }

    public void DecreaseResourceGenerationRate(ScenarioResourceType resourceType, float amount = 0)
    {
        currentGenerationRates[(int)resourceType].resourceAmount -= (int)amount;
        ClampResources();
    }

    public void DecreaseResourceGenerationRate(BuildCost[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            currentGenerationRates[i].resourceAmount -= resources[i].resourceAmount;
        }
        ClampResources();
    }
    #endregion

    public bool CanAddResource(ScenarioResourceType type, int amount)
    {
        return currentResources[(int)type].resourceAmount + amount <= maxResources[(int)type].resourceAmount;
    }
    void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(ScenarioResourceType));
        if (startingResources == null || startingResources.Length != enumValues.Length)
        {
            startingResources = new BuildCost[enumValues.Length];
        }

        for (int i = 0; i < startingResources.Length; i++)
        {
            startingResources[i].resourceType = (ScenarioResourceType)enumValues.GetValue(i);
        }
    }

    [Button]
    public void DecreaseAllResources(int amount)
    {
        for (int i = 0; i < currentResources.Length; i++)
        {
            currentResources[i].resourceAmount -= amount;
        }

        ClampResources();
    }
}
