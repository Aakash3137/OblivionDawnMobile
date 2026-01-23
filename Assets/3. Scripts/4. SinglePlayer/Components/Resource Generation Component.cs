using System.Collections;
using UnityEngine;

public class ResourceGenerationComponent : MonoBehaviour
{
    public bool autoProduce = true;

    [Header(" EDITOR VIEW ONLY ")]
    public int GeneratedResourceAmount;
    private float resourceGenerationRate;
    public float resourceTimeToProduce;
    private PlayerResourceManager prmInstance;
    private WaitForSeconds resourceTimeToProduceDelay;
    private IdentityComponent identityComponent;
    private BasicStatsComponent basicStatsComponent;

    // Initialize in Resource Building
    public void Initialize(ResourceBuildingDataSO generationData, int spawnLevel)
    {
        GeneratedResourceAmount = 0;

        //Registering resource type and resource name        
        var resourceType = generationData.resourceType;

        //Set resource generation values
        int resourceAmountPerBatch = generationData.resourceBuildingUpgradeData[spawnLevel].resourceAmountPerBatch;
        resourceTimeToProduce = generationData.resourceBuildingUpgradeData[spawnLevel].resourceTimeToProduce;
        resourceGenerationRate = generationData.resourceBuildingUpgradeData[spawnLevel].resourceGenerationRate;

        resourceTimeToProduceDelay = new WaitForSeconds(resourceTimeToProduce);
        prmInstance = PlayerResourceManager.Instance;

        basicStatsComponent = GetComponent<BasicStatsComponent>();
        identityComponent = GetComponent<IdentityComponent>();

        StartCoroutine(StartResourceGeneration(resourceType, resourceAmountPerBatch, resourceTimeToProduceDelay));
    }

    private IEnumerator StartResourceGeneration(ScenarioResourceType resourceType, int amount, WaitForSeconds time)
    {
        while (autoProduce && basicStatsComponent.currentHealth > 0)
        {
            yield return time;
            prmInstance.AddResources(resourceType, amount);
            GeneratedResourceAmount += amount;
            // Debug.Log($"Generated {ResourceName}. Current amount: {GeneratedResourceAmount}");
        }
    }

    public void ResourceGenerationRateHandler(ScenarioResourceType resourceType, int multiplier)
    {
        switch (identityComponent.side)
        {
            case Side.Player:
                prmInstance.SetResourceGenerationRate(resourceType, resourceGenerationRate * multiplier);
                break;
            case Side.Enemy:

                break;
        }
    }
}

