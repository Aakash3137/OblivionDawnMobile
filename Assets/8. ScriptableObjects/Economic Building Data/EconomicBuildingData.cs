using UnityEngine;

[CreateAssetMenu(fileName = "EconomicBuildingData", menuName = "Resource Data/Economic Building Data")]
public class EconomicBuildingData : ScriptableObject
{
    public string resourceName;
    public ResourceType resourceType;
    public FactionName faction;
    public int resourceAmountPerBatch;
    public float resourceTimeToProduce;

}
