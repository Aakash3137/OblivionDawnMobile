using Fusion;
using UnityEngine;
public enum NetworkSide : int { None = 0, Player = 1, Enemy = 2 }

[RequireComponent(typeof(NetworkObject))]
public class NetworkTile : NetworkBehaviour
{
    [Header("Ownership")]
    public NetworkSide initialOwnerSide;
    
    [Header("Debug – Visual owner seen by this client")]
    [SerializeField] private NetworkSide currentVisualOwner;
    public NetworkSide CurrentVisualOwner => currentVisualOwner;

    [Header("Visuals")]
    public Renderer tileRenderer;
    public Material selectionMaterial;
    
    public Vector2Int  Coord;
    
    [Networked] public int OwnerInt { get; set; }   //Real owner (host controlled)
    
    [Networked] public bool IsOccupied { get; set; }
    public bool isOpen = false;
    
    private bool localSelected = false;

    private Material originalMaterial;
    private NetworkSideManager sideManager;

    private int _lastOwnerInt = -999;
    private bool _lastSelected = false;
    
    public bool CanBeSelected()
    {
        if (IsOccupied)
            return false;

        return currentVisualOwner == NetworkSide.Player;
    }
    private void Awake()
    {
        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        originalMaterial = tileRenderer.material;
    }

    public override void Spawned()
    {
        // Auto-detect coordinate from world position
         Coord = NetworkCubeGridManager.Instance.WorldToGrid(transform.position);

        sideManager = FindAnyObjectByType<NetworkSideManager>();

        //Host sets the rel owner
        if (Object.HasStateAuthority)
        {
            OwnerInt = (int)initialOwnerSide;
            IsOccupied = false;  
        }

        _lastOwnerInt = OwnerInt;
        
        ApplyOwnerVisual();
        UpdateLocalSelectionVisual();
        
        // Register tile in   grid manager
        NetworkCubeGridManager.Instance.RegisterCube( Coord, gameObject);
        
        // Mark main building tiles as occupied
        if (Object.HasStateAuthority)
        {
            if (this == NetworkCubeGridManager.Instance.MainBuildingTile1 || 
                this == NetworkCubeGridManager.Instance.MainBuildingTile2)
            {
                IsOccupied = true;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_lastOwnerInt != OwnerInt)
        {
            _lastOwnerInt = OwnerInt;
            ApplyOwnerVisual();

            NetworkCubeGridManager.Instance?.NotifyOwnerChanged();
        }
    }

    public void OccupyTile()
    {
        if (Object.HasStateAuthority)
            IsOccupied = true;
    }

    public void FreeTile()
    {
        if (Object.HasStateAuthority)
            IsOccupied = false;
    }
    
    // shows selection local
    public void SetLocalSelected(bool selected)
    {
        Debug.Log($"[NetworkTile] SetLocalSelected({selected}) called on {name}");
        localSelected = selected;
        UpdateLocalSelectionVisual();
    }
    
    private void UpdateLocalSelectionVisual()
    {
        if (tileRenderer == null)
        {
            Debug.LogWarning($"[NetworkTile] UpdateLocalSelectionVisual: tileRenderer is null on {name}");
            return;
        }

        if (localSelected && selectionMaterial != null)
        {
            //Debug.Log($"[NetworkTile] Applying selection material to {name}");
            tileRenderer.material = selectionMaterial;
        }
        else
        {
           // Debug.Log($"[NetworkTile] Applying owner visual to {name} (localSelected: {localSelected}, selectionMaterial: {selectionMaterial != null})");
            ApplyOwnerVisual();
        }
    }
    
    // --------------------------------------------------------------------
    // HANDLE CLICK (LOCAL ONLY)
    // --------------------------------------------------------------------
    public void HandleClick()
    {
        Debug.Log($"[NetworkTile] HandleClick called on {name}");
        Debug.Log($"[NetworkTile] Calling TileSelectionManager.Local_SelectTile for {name}");
        TileSelectionManager.Instance?.TrySelectTile(this);
    }

    // --------------------------------------------------------------------
    // NETWORK BUILD REQUEST
    // --------------------------------------------------------------------
    public void RequestBuild(string buildingName)
    {
        Debug.Log($"[NetworkTile] RequestBuild called on {name} for {buildingName}. Object.HasInputAuthority = {Object?.HasInputAuthority.ToString() ?? "null"}; Object.Id = {(Object != null ? Object.Id.ToString() : "null")}");

        NetworkBuildingManager.Instance.RequestBuild(this, buildingName);
    }
    

    private void ApplyOwnerVisual()
    {
        if (sideManager == null || tileRenderer == null)
            return;

        NetworkSide realSide = (NetworkSide)OwnerInt;
        
        //Setting the visuals over both host and client
        // Find local player with active camera
        var allPlayers = FindObjectsOfType<NetworkPlayer>();
        NetworkPlayer localPlayer = null;
        
        foreach (var player in allPlayers)
        {
            if (player.MainCamera != null && player.MainCamera.gameObject.activeSelf)
            {
                localPlayer = player;
                break;
            }
        }
        
        if (localPlayer == null)
        {
            currentVisualOwner = realSide;
            sideManager.SetSide(tileRenderer, realSide);
            return;
        }

        float playerZ = localPlayer.transform.position.z;
        int spawnId = playerZ < 15f ? 0 : 1;
        
        if (spawnId == 0)  //real side
        {
            currentVisualOwner = realSide;
            sideManager.SetSide(tileRenderer, realSide);
        }
        else  //flipside
        {
            NetworkSide flipped = realSide;

            if (realSide == NetworkSide.Player)
                flipped = NetworkSide.Enemy;
            else if (realSide == NetworkSide.Enemy)
                flipped = NetworkSide.Player;

            currentVisualOwner = flipped;
            sideManager.SetSide(tileRenderer, flipped);
        }
    }
    
    public void SetBuildingPlaced()
    {
        IsOccupied = true;
        isOpen = false;

        // Hide PlusIcon if present
        Transform cubeChild = transform.Find("Cube");
        if (cubeChild != null)
        {
            Transform plusIcon = cubeChild.Find("Plus_Icon");
            if (plusIcon != null) plusIcon.gameObject.SetActive(false);
        }
    }
    
}