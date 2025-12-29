using System.Collections;
using UnityEngine;

public class ResourceGen : MonoBehaviour
{
    [SerializeField] private ResourceGenerationStatsSO buildingResourceData;
    public bool autoProduce = true;
    private SideScenario buildingSide;   // building's faction marker
    private BuildingStats buildingStats;

    [Header(" EDITOR VIEW ONLY ")]
    [SerializeField] private int GeneratedResourceAmount; //Generation Data
    private string ResourceName;
    private int buildingCurrentLevel;
    private ScenarioResourceType resourceType;
    private int resourceAmountPerBatch;
    private float resourceTimeToProduce;
    private float resourceGenerationRate;

    private void Awake()
    {
        //set values
        buildingCurrentLevel = 0;
        GeneratedResourceAmount = 0;

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
    private IEnumerator StartResourceGeneration()
    {
        if (buildingSide.side != Side.Player)
            yield break;

        PlayerResourceManager.Instance.SetResourceGenerationRate(resourceType, resourceGenerationRate);

        while (autoProduce && buildingStats.currentHealth > 0)
        {
            yield return new WaitForSeconds(resourceTimeToProduce);
            PlayerResourceManager.Instance.AddResources(resourceType, resourceAmountPerBatch);
            GeneratedResourceAmount += resourceAmountPerBatch;
            // Debug.Log($"Generated {ResourceName}. Current amount: {GeneratedResourceAmount}");
        }
    }
    void OnDisable()
    {
        PlayerResourceManager.Instance.SetResourceGenerationRate(resourceType, -resourceGenerationRate);
    }
}
