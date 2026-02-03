
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

    void Start()
    {
        SetResources(startingResources);
    }

    public void SetResources(BuildCost[] resources)
    {
        currentFood = resources[0].resourceCost;
        currentGold = resources[1].resourceCost;
        currentMetal = resources[2].resourceCost;
        CurrentPower = resources[3].resourceCost;

        // Invoke the event to notify listeners
        OnResourcesChanged.Invoke();
    }

    public void AddResources(ScenarioResourceType resourceType, int amount = 0)
    {
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
        OnResourcesChanged.Invoke();
    }

    public void SpendResources(BuildCost[] resources)
    {
        currentFood -= resources[0].resourceCost;
        currentGold -= resources[1].resourceCost;
        currentMetal -= resources[2].resourceCost;
        CurrentPower -= resources[3].resourceCost;

        // Invoke the event to notify listeners
        OnResourcesChanged.Invoke();
    }

    public void SetResourceGenerationRate(ScenarioResourceType resourceType, float amount = 0)
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

        OnResourcesChanged?.Invoke();
    }
    public bool HasResources(BuildCost[] resources, bool debug = false)
    {
        int food = resources[0].resourceCost;
        int gold = resources[1].resourceCost;
        int metal = resources[2].resourceCost;
        int power = resources[3].resourceCost;

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
