
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Set Starting Resources")]
    public BuildCost[] startingResources;
    public int currentFood { get; protected set; }
    public int currentGold { get; protected set; }
    public int currentMetal { get; protected set; }
    public int CurrentPower { get; protected set; }

    public int maxFood { get; protected set; }
    public int maxGold { get; protected set; }
    public int maxMetal { get; protected set; }
    public int maxPower { get; protected set; }

    [field: Header("EDITOR VIEW ONLY")]
    [field: SerializeField, ReadOnly]
    public float currentFoodGenerationRate { get; protected set; }
    [field: SerializeField, ReadOnly]
    public float currentGoldGenerationRate { get; protected set; }
    [field: SerializeField, ReadOnly]
    public float currentMetalGenerationRate { get; protected set; }
    [field: SerializeField, ReadOnly]
    public float currentPowerGenerationRate { get; protected set; }

    public Action OnResourcesChanged;

    private void Start()
    {
        SetResources(startingResources);
    }

    public void SetResources(BuildCost[] resources)
    {
        currentFood = resources[0].resourceAmount;
        currentGold = resources[1].resourceAmount;
        currentMetal = resources[2].resourceAmount;
        CurrentPower = resources[3].resourceAmount;

        maxFood = currentFood;
        maxGold = currentGold;
        maxMetal = currentMetal;
        maxPower = CurrentPower;

        // Invoke the event to notify listeners
        OnResourcesChanged?.Invoke();
    }

    public void IncreaseResourcesCap(ScenarioResourceType resourceType, int amount = 0)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                maxFood += amount;
                break;
            case ScenarioResourceType.Gold:
                maxGold += amount;
                break;
            case ScenarioResourceType.Metal:
                maxMetal += amount;
                break;
            case ScenarioResourceType.Power:
                maxPower += amount;
                break;
        }
    }
    public void DecreaseResourcesCap(ScenarioResourceType resourceType, int amount = 0)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                maxFood -= amount;
                break;
            case ScenarioResourceType.Gold:
                maxGold -= amount;
                break;
            case ScenarioResourceType.Metal:
                maxMetal -= amount;
                break;
            case ScenarioResourceType.Power:
                maxPower -= amount;
                break;
        }

        ClampResources();
    }


    public void AddResources(ScenarioResourceType resourceType, int amount = 0)
    {
        // Don't over produce
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                currentFood += amount;
                break;
            case ScenarioResourceType.Gold:
                currentGold += amount;
                break;
            case ScenarioResourceType.Metal:
                currentMetal += amount;
                break;
            case ScenarioResourceType.Power:
                CurrentPower += amount;
                break;
        }

        // Invoke the event to notify listeners
        ClampResources();
    }

    public void SpendResources(BuildCost[] resources)
    {
        currentFood -= resources[0].resourceAmount;
        currentGold -= resources[1].resourceAmount;
        currentMetal -= resources[2].resourceAmount;
        CurrentPower -= resources[3].resourceAmount;

        // Invoke the event to notify listeners
        ClampResources();
    }

    public bool HasResources(BuildCost[] resources, bool debug = false)
    {
        int food = resources[0].resourceAmount;
        int gold = resources[1].resourceAmount;
        int metal = resources[2].resourceAmount;
        int power = resources[3].resourceAmount;

        if (debug)
        {
            if (currentFood - food < 0)
                Debug.Log($"<color=#C6616B>Not enough food, Need {food - currentFood} more food</color>");
            if (currentGold - gold < 0)
                Debug.Log($"<color=#D4B608>Not enough gold, Need {gold - currentGold} more gold</color>");
            if (currentMetal - metal < 0)
                Debug.Log($"<color=#ADB4BD>Not enough metal, Need {metal - currentMetal} more metal</color>");
            if (CurrentPower - power < 0)
                Debug.Log($"<color=#4AAEDB>Not enough power, Need {power - CurrentPower} more power</color>");
        }

        return currentFood >= food && currentGold >= gold && currentMetal >= metal && CurrentPower >= power;
    }
    private void ClampResources()
    {
        currentFood = Mathf.Clamp(currentFood, 0, maxFood);
        currentGold = Mathf.Clamp(currentGold, 0, maxGold);
        currentMetal = Mathf.Clamp(currentMetal, 0, maxMetal);
        CurrentPower = Mathf.Clamp(CurrentPower, 0, maxPower);

        OnResourcesChanged?.Invoke();
    }

    #region Resource Generation
    public void IncreaseResourceGenerationRate(ScenarioResourceType resourceType, float amount = 0)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                currentFoodGenerationRate += amount;
                break;
            case ScenarioResourceType.Gold:
                currentGoldGenerationRate += amount;
                break;
            case ScenarioResourceType.Metal:
                currentMetalGenerationRate += amount;
                break;
            case ScenarioResourceType.Power:
                currentPowerGenerationRate += amount;
                break;
        }

        // This event is handling both resources changed and resource generation rate changed
        OnResourcesChanged?.Invoke();
    }

    public void IncreaseResourceGenerationRate(BuildCost[] resources)
    {
        currentFoodGenerationRate += resources[0].resourceAmount;
        currentGoldGenerationRate += resources[1].resourceAmount;
        currentMetalGenerationRate += resources[2].resourceAmount;
        currentPowerGenerationRate += resources[3].resourceAmount;

        // This event is handling both resources changed and resource generation rate changed
        OnResourcesChanged?.Invoke();
    }

    public void DecreaseResourceGenerationRate(ScenarioResourceType resourceType, float amount = 0)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                currentFoodGenerationRate -= amount;
                break;
            case ScenarioResourceType.Gold:
                currentGoldGenerationRate -= amount;
                break;
            case ScenarioResourceType.Metal:
                currentMetalGenerationRate -= amount;
                break;
            case ScenarioResourceType.Power:
                currentPowerGenerationRate -= amount;
                break;
        }

        // This event is handling both resources changed and resource generation rate changed
        OnResourcesChanged?.Invoke();
    }

    public void DecreaseResourceGenerationRate(BuildCost[] resources)
    {
        currentFoodGenerationRate -= resources[0].resourceAmount;
        currentGoldGenerationRate -= resources[1].resourceAmount;
        currentMetalGenerationRate -= resources[2].resourceAmount;
        currentPowerGenerationRate -= resources[3].resourceAmount;

        // This event is handling both resources changed and resource generation rate changed
        OnResourcesChanged?.Invoke();
    }
    #endregion

    public bool CanAddResource(ScenarioResourceType type, int amount)
    {
        switch (type)
        {
            case ScenarioResourceType.Food:
                return currentFood + amount <= maxFood;
            case ScenarioResourceType.Gold:
                return currentGold + amount <= maxGold;
            case ScenarioResourceType.Metal:
                return currentMetal + amount <= maxMetal;
            case ScenarioResourceType.Power:
                return CurrentPower + amount <= maxPower;
        }
        return false;
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
}
