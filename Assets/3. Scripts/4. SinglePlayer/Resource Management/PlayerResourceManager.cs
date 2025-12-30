using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerResourceManager : MonoBehaviour
{
    [Header("Set Starting Resources")]
    [SerializeField] private BuildingUpgradeCost[] startingResources;
    public static PlayerResourceManager Instance;
    public int currentFood { get; private set; }
    public int currentGold { get; private set; }
    public int currentMetal { get; private set; }
    public int CurrentPower { get; private set; }

    [field: Header("EDITOR VIEW ONLY")]
    [field: SerializeField]
    public float currentFoodGenerationRate { get; private set; }
    [field: SerializeField]
    public float currentGoldGenerationRate { get; private set; }
    [field: SerializeField]
    public float currentMetalGenerationRate { get; private set; }
    [field: SerializeField]
    public float currentPowerGenerationRate { get; private set; }

    [HideInInspector]
    public UnityEvent OnResourcesChanged = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetResources(startingResources);
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

    public void SpendResources(BuildingUpgradeCost[] resources)
    {
        currentFood -= resources[0].resourceCost;
        currentGold -= resources[1].resourceCost;
        currentMetal -= resources[2].resourceCost;
        CurrentPower -= resources[3].resourceCost;

        // Invoke the event to notify listeners
        OnResourcesChanged.Invoke();
    }
    public void SetResources(BuildingUpgradeCost[] resources)
    {
        currentFood = resources[0].resourceCost;
        currentGold = resources[1].resourceCost;
        currentMetal = resources[2].resourceCost;
        CurrentPower = resources[3].resourceCost;

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
    }
    public bool HasResources(BuildingUpgradeCost[] resources, bool debug = false)
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
            startingResources = new BuildingUpgradeCost[enumValues.Length];
        }

        for (int i = 0; i < startingResources.Length; i++)
        {
            startingResources[i].resourceType = (ScenarioResourceType)enumValues.GetValue(i);
        }
    }

    [ContextMenu("Hack Resources")]
    void HackResources()
    {
        BuildingUpgradeCost[] resources = new BuildingUpgradeCost[4];
        resources[0].resourceCost = 9999;
        resources[1].resourceCost = 9999;
        resources[2].resourceCost = 9999;
        resources[3].resourceCost = 9999;

        SetResources(resources);
    }

}
