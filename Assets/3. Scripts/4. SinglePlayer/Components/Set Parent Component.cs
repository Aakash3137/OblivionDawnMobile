using UnityEngine;

public class SetParentComponent : MonoBehaviour
{
    private MilitaryUnit myUnit;
    private BuildingBlueprint myBuilding;
    private GameObject parentObject;

    public void Initialize()
    {
        myUnit = TryGetComponent<MilitaryUnit>(out var unit) ? unit : null;
        myBuilding = TryGetComponent<BuildingBlueprint>(out var building) ? building : null;

        if (myUnit != null)
        {
            parentObject = GameObject.FindWithTag("UnitPool");
            if (parentObject == null)
                Debug.Log("<color=red>No GameObject with tag 'UnitPool' found in scene!!!!</color>");
        }

        if (myBuilding != null)
        {
            switch (myBuilding.buildingType)
            {
                case ScenarioBuildingType.MainBuilding:
                    parentObject = GameObject.FindWithTag("MainPool");
                    if (parentObject == null)
                        Debug.Log("<color=red>No GameObject with tag 'MainPool' found in scene!!!!</color>");
                    break;
                case ScenarioBuildingType.DefenseBuilding:
                    parentObject = GameObject.FindWithTag("DefensePool");
                    if (parentObject == null)
                        Debug.Log("<color=red>No GameObject with tag 'DefensePool' found in scene!!!!</color>");
                    break;
                case ScenarioBuildingType.OffenseBuilding:
                    parentObject = GameObject.FindWithTag("OffensePool");
                    if (parentObject == null)
                        Debug.Log("<color=red>No GameObject with tag 'OffensePool' found in scene!!!!</color>");
                    break;
                case ScenarioBuildingType.ResourceBuilding:
                    parentObject = GameObject.FindWithTag("ResourcePool");
                    if (parentObject == null)
                        Debug.Log("<color=red>No GameObject with tag 'ResourcePool' found in scene!!!!</color>");
                    break;
            }
        }

        transform.parent = parentObject?.transform;
    }
}
