using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingStats : Stats
{
    [field: Header("Assign Building Stats")]
    [field: SerializeField]
    public BuildingDataSO buildingStatsSO { get; private set; }
    public ScenarioBuildingType buildingType { get; private set; }
    public float buildTime { get; protected set; }
    private GameObject buildingPool;
    [field: SerializeField, ReadOnly]
    public Tile currentTile { get; private set; }


    internal override void Initialize()
    {
        if (buildingStatsSO == null)
        {
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");
        }

        buildingType = buildingStatsSO.buildingType;
        visuals = buildingStatsSO.buildingVisuals;

        currentTile = GetComponentInParent<Tile>();
        currentTile.SetOccupant(this);

        side = currentTile.ownerSide;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {buildingStatsSO.name} ScriptableObject</color>");
        }

        SetParent();

        base.Initialize();
    }

    private void SetParent()
    {
        switch (buildingType)
        {
            case ScenarioBuildingType.MainBuilding:
                buildingPool = GameObject.FindWithTag("MainPool");
                if (buildingPool == null)
                    Debug.Log("<color=green>No GameObject with tag 'MainPool' found in scene!</color>");
                break;
            case ScenarioBuildingType.DefenseBuilding:
                buildingPool = GameObject.FindWithTag("DefensePool");
                if (buildingPool == null)
                    Debug.Log("<color=green>No GameObject with tag 'DefensePool' found in scene!</color>");
                break;
            case ScenarioBuildingType.OffenseBuilding:
                buildingPool = GameObject.FindWithTag("OffensePool");
                if (buildingPool == null)
                    Debug.Log("<color=green>No GameObject with tag 'OffensePool' found in scene!</color>");
                break;
            case ScenarioBuildingType.ResourceBuilding:
                buildingPool = GameObject.FindWithTag("ResourcePool");
                if (buildingPool == null)
                    Debug.Log("<color=green>No GameObject with tag 'ResourcePool' found in scene!</color>");
                break;
        }

        transform.parent = buildingPool?.transform;
    }

    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddBuildingDestroyedData(buildingType, side);
    }

    internal virtual void OnDestroy()
    {
        if (currentTile != null)
            currentTile.ClearOccupant();
    }
}
