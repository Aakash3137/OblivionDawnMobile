using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public Side ownerSide { get; private set; }
    [field: SerializeField, ReadOnly] public bool isOpen { get; private set; } = false;
    [field: SerializeField, ReadOnly] public bool hasBuilding { get; private set; }
    [field: SerializeField, ReadOnly] public BuildingStats currentBuilding { get; private set; }
    [field: SerializeField, ReadOnly] public List<Tile> proxyTiles = new List<Tile>();

    [Space(15)]
    [SerializeField, ReadOnly] private Vector2Int coord;
    [SerializeField, ReadOnly] private MeshRenderer meshObject;
    [SerializeField, ReadOnly] private GameObject openTileVisual;

    [Space(5)]
    [Header("Borders Right Left Up Down (East West North South) ")]
    [SerializeField] GameObject[] borders;
    [Space(5)]
    [Header("Materials")]
    public Material playerMaterial;
    public Material enemyMaterial;
    public Material neutralMaterial;
    public Material neutralAllyMaterial;
    public Material neutralEnemyMaterial;

    private Side previousSide = Side.Neutral;

    private CubeGridManager cgmInstance => CubeGridManager.Instance;

    private void Start()
    {
        RefreshBorders();
    }

#if UNITY_EDITOR
    public void InitializeSide(Side side)
    {
        // Only for editor to assign side to apply serialization of field is required
        ChangeSide(side);
        ApplyMaterials();
    }
#endif
    public void Initialize(Vector2Int coord)
    {
        this.coord = coord;
    }

    public void ChangeSide(Side side)
    {
        ownerSide = side;

        if (!Application.isPlaying) return;

        RefreshBorders();

        foreach (var neighbor in cgmInstance.GetCardinalTiles(coord))
        {
            neighbor?.RefreshBorders();
        }
    }

    private void ApplyMaterials()
    {
        var material = ownerSide switch
        {
            Side.Player => playerMaterial,
            Side.Enemy => enemyMaterial,
            Side.Neutral => neutralMaterial,
            Side.NeutralAlly => neutralAllyMaterial,
            Side.NeutralEnemy => neutralEnemyMaterial,
            _ => playerMaterial
        };

        if (meshObject != null)
        {
            meshObject.sharedMaterial = material;
        }
    }

    public void OpenStatusHandler(bool flag)
    {
        isOpen = flag;

        if (openTileVisual == null)
            return;

        if (ownerSide != Side.Player)
        {
            openTileVisual.SetActive(false);
            return;
        }

        var allNeighbors = cgmInstance.GetAllTiles(coord);

        foreach (var neighbor in allNeighbors)
        {
            if (neighbor == null)
                continue;

            if (neighbor.currentBuilding != null && neighbor.currentBuilding.side != Side.Player)
            {
                openTileVisual.SetActive(false);
                return;
            }
        }

        openTileVisual.SetActive(flag);
    }

    public void Occupy(Side side)
    {
        if (ownerSide != Side.Player && ownerSide != Side.Enemy)
            return;

        // Change previous side only when occupying
        previousSide = ownerSide;
        ChangeSide(side);
        OpenStatusHandler(isOpen);
        cgmInstance.TileOccupied(side, this);
    }

    public void RefreshBorders()
    {
        var neighbors = cgmInstance.GetCardinalTiles(coord);

        for (int i = 0; i < borders.Length; i++)
        {
            borders[i].SetActive(ShouldShowBorder(neighbors[i]));
        }
    }

    private bool ShouldShowBorder(Tile neighbor)
    {
        if (ownerSide != Side.Player)
            return false;

        bool neighborIsDifferentSide = neighbor == null || neighbor.ownerSide != Side.Player;
        return neighborIsDifferentSide;
    }

    public void SetCurrentBuilding(BuildingStats building)
    {
        currentBuilding = building;
        hasBuilding = true;
    }
    public void ClearCurrentBuilding()
    {
        currentBuilding = null;
        hasBuilding = false;
    }

    private void OnValidate()
    {
        if (meshObject == null)
            meshObject = transform.GetChild(0).GetComponent<MeshRenderer>();

        if (openTileVisual == null)
            openTileVisual = transform.GetChild(1).gameObject;
    }
}