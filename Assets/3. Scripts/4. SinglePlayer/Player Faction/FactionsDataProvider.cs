using UnityEngine;

public class FactionsDataProvider : MonoBehaviour
{
    [SerializeField] private AllFactionsData dataAsset;

    private void Awake()
    {
        // Store the asset globally so UnitSide / Spawner can access it
        GameData.AllFactionsData = dataAsset;
    }
}
