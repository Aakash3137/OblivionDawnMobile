using System.Collections;
using UnityEngine;

public class ResourceGen : MonoBehaviour
{
    [SerializeField] private ResourceGenerationStatsSO buildingResourceData;
    public bool autoProduce = true;
    private SideScenario buildingSide;
    private BuildingStats buildingStats;

    [Header(" EDITOR VIEW ONLY ")]
    [SerializeField] private int GeneratedResourceAmount;
    private string ResourceName;
    private int buildingCurrentLevel;
    private ScenarioResourceType resourceType;
    private int resourceAmountPerBatch;
    public float resourceTimeToProduce { get; private set; }
    private float resourceGenerationRate;
    private PlayerResourceManager prmInstance;


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

    private async Awaitable Start()
    {
        prmInstance = PlayerResourceManager.Instance;
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

        prmInstance.SetResourceGenerationRate(resourceType, resourceGenerationRate);

        while (autoProduce && buildingStats.currentHealth > 0)
        {
            yield return new WaitForSeconds(resourceTimeToProduce);
            prmInstance.AddResources(resourceType, resourceAmountPerBatch);
            GeneratedResourceAmount += resourceAmountPerBatch;
            // Debug.Log($"Generated {ResourceName}. Current amount: {GeneratedResourceAmount}");
        }
    }

    private void OnDestroy()
    {
        prmInstance.SetResourceGenerationRate(resourceType, -resourceGenerationRate);
    }
}
