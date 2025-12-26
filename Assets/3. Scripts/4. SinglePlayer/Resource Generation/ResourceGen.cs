using System.Collections;
using UnityEngine;

public class ResourceGen : MonoBehaviour
{
    [SerializeField] private ResourceGenerationStatsSO buildingResourceData;

    [Header("Editor View purpose only")]
    public string ResourceName;
    private int buildingCurrentLevel;
    [SerializeField] private ScenarioResourceType resourceType;
    private int resourceAmountPerBatch;
    private float resourceTimeToProduce;
    private float resourceGenerationRate;
    //Generation Data
    [SerializeField] private int lifetimeResourcesGenerated;
    [SerializeField] private int currentResourceAmount;

    protected virtual void Awake()
    {
        //set values
        lifetimeResourcesGenerated = 0;
        buildingCurrentLevel = 0;
        currentResourceAmount = 0;

        //Registering resource type and resource name        
        resourceType = buildingResourceData.resourceType;
        ResourceName = buildingResourceData.resourceName;

        //Set resource generation values
        resourceAmountPerBatch = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceAmountPerBatch;
        resourceTimeToProduce = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceTimeToProduce;
        resourceGenerationRate = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceGenerationRate;
    }

    void Start()
    {
        GenerateResource();
    }

    public void UpgradeBuilding()
    {
        //Upgrade building and updating resource generation values
        buildingCurrentLevel++;
        resourceAmountPerBatch = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceAmountPerBatch;
        resourceTimeToProduce = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceTimeToProduce;
        resourceGenerationRate = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceGenerationRate;
    }

    public void GenerateResource()
    {
        StartCoroutine(StartResourceGeneration());
    }

    public void UseResource(int amount)
    {
        currentResourceAmount -= amount;
    }

    public int GetCurrentResourceAmount()
    {
        return currentResourceAmount;
    }

    public int GetLifetimeResourcesGenerated()
    {
        return lifetimeResourcesGenerated;
    }

    private IEnumerator StartResourceGeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(resourceTimeToProduce);
            currentResourceAmount += resourceAmountPerBatch;
            lifetimeResourcesGenerated += resourceAmountPerBatch;

        }
    }
}
