using UnityEngine;

[CreateAssetMenu(fileName = "Resource Building Stats", menuName = "Scenario Stats/Resource Building Stats")]
public class ResourceGenerationStatsSO : ScriptableObject
{
    public string resourceName;
    public ScenarioResourceType resourceType;
    public ResourceGenerationData[] resourceGenerationData;

    private void OnValidate()
    {
        if (resourceGenerationData == null) return;
        for (int i = 0; i < resourceGenerationData.Length; i++)
        {
            resourceGenerationData[i].resourceGenerationRate = resourceGenerationData[i].resourceAmountPerBatch / resourceGenerationData[i].resourceTimeToProduce;
            resourceGenerationData[i].level = i;
        }
    }
}

[System.Serializable]
public class ResourceGenerationData
{
    public int level;
    public int resourceAmountPerBatch;
    public float resourceTimeToProduce;
    public float resourceGenerationRate;
}