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

    [field: SerializeField, ReadOnly, Space(15)] public Vector2Int coord { get; private set; }
    [SerializeField, ReadOnly] private MeshRenderer meshObject;
    [SerializeField, ReadOnly] private GameObject openTileVisual;

    [Space(5)]
    [Header("Borders Right Left Up Down (East West North South) ")]
    [SerializeField] GameObject[] borders;
    [Space(5)]

    private Side previousSide = Side.Neutral;
    private CubeGridManager cgmInstance => CubeGridManager.Instance;

    [ReadOnly] public List<TileEffect> tileEffects = new();
    [ReadOnly] public TileEffectType tileEffectType;
    [ReadOnly] public GameObject tileEffectPrefab;

    private void Awake()
    {
        if (meshObject == null)
            meshObject = transform.GetChild(0).GetComponent<MeshRenderer>();

        if (openTileVisual == null)
            openTileVisual = transform.GetChild(1).gameObject;
    }

    public void Initialize(Side side, Vector2Int coord, Texture2D tileTexture = null)
    {
        ownerSide = side;
        if (meshObject != null && tileTexture != null)
        {
            meshObject.sharedMaterial.mainTexture = tileTexture;
        }
        this.coord = coord;
    }

    public void ChangeSide(Side side)
    {
        ownerSide = side;

        RefreshBorders();

        foreach (var neighbor in cgmInstance.GetCardinalTiles(coord))
        {
            neighbor?.RefreshBorders();
        }
    }

    public void InitializeTileBuffs()
    {
        for (int i = 0; i < tileEffects.Count; i++)
        {
            tileEffects[i].ApplyEffect(this);
        }
    }

    public void OverrideMaterial(Material material)
    {
        meshObject.sharedMaterial = material;
    }

    public void OpenStatusHandler(bool flag)
    {
        if (hasBuilding)
            isOpen = true;
        else
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

        if (ownerSide == side)
            return;

        ChangeSide(side);
        cgmInstance.TileOccupied(side, this);
        // Change previous side only when occupying
        previousSide = ownerSide;
        OpenStatusHandler(isOpen);
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
        if (tileEffectPrefab != null)
            tileEffectPrefab.SetActive(true);
    }

    private void OnValidate()
    {
        if (meshObject == null)
            meshObject = transform.GetChild(0).GetComponent<MeshRenderer>();

        if (openTileVisual == null)
            openTileVisual = transform.GetChild(1).gameObject;
    }
    private void OnDestroy()
    {
        if (cgmInstance != null)
            cgmInstance.onTilesGenerated -= RefreshBorders;
    }
}