
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Global Resource Tick")]
    [field: SerializeField] public float globalTickTime { get; private set; }
    [SerializeField, Header("Inclusive")] private int minStartAmount = 150;
    [SerializeField, Header("Not Inclusive")] private int maxStartAmount = 351;

    [Header("Set Starting Resources"), ReadOnly] public BuildCost[] startingResources { get; private set; } = new BuildCost[0];

    [Header("Current Resources"), ReadOnly] public BuildCost[] currentResources { get; private set; } = new BuildCost[0];

    [Header("Max Resources (Capacity)"), ReadOnly] public BuildCost[] maxResources { get; private set; } = new BuildCost[0];

    [Header("Generation Rates"), ReadOnly] public BuildCost[] currentGenerationRates { get; private set; } = new BuildCost[0];

    [Header("Resource Building Counts"), ReadOnly] public int[] resourceBuildingCounts { get; private set; } = new int[0];


    public System.Action OnResourcesChanged;
    public System.Action GlobalResourceTick;

    private void Start()
    {
        InitializeStartingResources();
        _ = StartGlobalTick();
    }

    private async Awaitable StartGlobalTick()
    {
        while (gameObject.activeInHierarchy)
        {
            await Awaitable.WaitForSecondsAsync(globalTickTime, destroyCancellationToken);

            if (this is PlayerResourceManager playerResourceManager && GameplayRegistry.GetResource(Side.Player).Count > 0)
                AudioManager.PlayOneShot(GameAudioType.ResourceTick);

            GlobalResourceTick?.Invoke();
        }
    }

    #region  Helper Functions
    private void InitializeStartingResources()
    {
        var enumValues = ScenarioDataTypes._resourceEnumValues;
        int resourceCount = enumValues.Length;

        startingResources = new BuildCost[resourceCount];
        currentResources = new BuildCost[resourceCount];
        maxResources = new BuildCost[resourceCount];
        currentGenerationRates = new BuildCost[resourceCount];
        resourceBuildingCounts = new int[resourceCount];

        int randomAmount = Random.Range(minStartAmount, maxStartAmount);

        for (int i = 0; i < resourceCount; i++)
        {
            var type = enumValues[i];

            randomAmount = Mathf.CeilToInt(randomAmount / 10f) * 10;

            startingResources[i].resourceType = type;
            startingResources[i].resourceAmount = randomAmount;

            currentResources[i].resourceType = type;
            currentResources[i].resourceAmount = randomAmount;

            maxResources[i].resourceType = type;
            maxResources[i].resourceAmount = randomAmount;

            currentGenerationRates[i].resourceType = type;
            currentGenerationRates[i].resourceAmount = 0;
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

    // distribute resources evenly then redistribute overflow from capped slots into uncapped slots
    public void BalanceResources()
    {
        int totalAmount = 0;
        for (int i = 0; i < currentResources.Length; i++)
            totalAmount += currentResources[i].resourceAmount;

        int[] assigned = new int[currentResources.Length];
        bool[] capped = new bool[currentResources.Length];

        int slots = currentResources.Length;
        int remaining = 0;

        for (int i = 0; i < currentResources.Length; i++)
        {
            int share = totalAmount / slots + (i < totalAmount % slots ? 1 : 0);
            int clamped = Mathf.Min(share, maxResources[i].resourceAmount);

            assigned[i] = clamped;
            capped[i] = clamped < share;
            remaining += share - clamped;
        }

        // redistribute leftover into uncapped slots
        for (int i = 0; i < currentResources.Length && remaining > 0; i++)
        {
            if (!capped[i])
            {
                int space = maxResources[i].resourceAmount - assigned[i];
                int toAdd = Mathf.Min(space, remaining);
                assigned[i] += toAdd;
                remaining -= toAdd;
            }
        }

        for (int i = 0; i < currentResources.Length; i++)
            currentResources[i].resourceAmount = assigned[i];

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

    [Button("DEBUG: Hack Resources")]
    public void HackResources()
    {
        for (int i = 0; i < currentResources.Length; i++)
        {
            currentResources[i].resourceAmount = 999;
            maxResources[i].resourceAmount = 999;
        }

        ClampResources();
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

    [Button("DEBUG: Consume Resource")]
    public void RemoveResources(ScenarioResourceType resourceType, int amount)
    {
        AddResources(resourceType, -amount);
    }
    #endregion
}
