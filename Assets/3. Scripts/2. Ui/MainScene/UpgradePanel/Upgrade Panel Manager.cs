using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] private List<UnitProduceStatsSO> unitScriptables;
    [SerializeField] private List<BuildingDataSO> buildingScriptables;

    [SerializeField] private CardUpgrade buildingCard;
    [SerializeField] private CardUpgrade unitCard;


    public void CreateBuildingCards()
    {
        foreach (var item in buildingScriptables)
        {
            CardUpgrade card = Instantiate(buildingCard, transform);
            // card.GetComponent<BuildingCard>().Initialize(item);
        }
    }

}
