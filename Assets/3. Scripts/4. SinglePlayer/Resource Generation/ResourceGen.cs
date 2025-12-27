using System.Collections;
using UnityEngine;

public class ResourceGen : MonoBehaviour
{
    [SerializeField] private ResourceGenerationStatsSO buildingResourceData;
    public bool autoProduce = true;
    private SideScenario buildingSide;   // building's faction marker
    private BuildingStats buildingStats;

    [Header("Editor View purpose only")]
    public string ResourceName;
    private int buildingCurrentLevel;
    [SerializeField] private ScenarioResourceType resourceType;
    private int resourceAmountPerBatch;
    private float resourceTimeToProduce;
    private float resourceGenerationRate;
    //Generation Data
    [SerializeField] private int GenerateResourceAmount;

    private void Awake()
    {
        //set values
        buildingCurrentLevel = 0;
        GenerateResourceAmount = 0;

        //Registering resource type and resource name        
        resourceType = buildingResourceData.resourceType;
        ResourceName = buildingResourceData.resourceName;

        //Set resource generation values
        resourceAmountPerBatch = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceAmountPerBatch;
        resourceTimeToProduce = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceTimeToProduce;
        resourceGenerationRate = buildingResourceData.resourceGenerationData[buildingCurrentLevel].resourceGenerationRate;

        buildingSide = GetComponent<SideScenario>();
        buildingStats = GetComponent<BuildingStats>();
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
        GenerateResourceAmount -= amount;
    }
    public float GetResourceGenerationRate()
    {
        return resourceGenerationRate;
    }
    private IEnumerator StartResourceGeneration()
    {
        while (autoProduce && buildingSide.side == Side.Player && buildingStats.currentHealth > 0)
        {
            Debug.Log($"Generating {resourceAmountPerBatch} {ResourceName}.... at {resourceGenerationRate} per second");
            yield return new WaitForSeconds(resourceTimeToProduce);
            GenerateResourceAmount += resourceAmountPerBatch;
            Debug.Log($"Generated {ResourceName}. Current amount: {GenerateResourceAmount}");
        }
    }
}
