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
    
    public Vector2Int HexCoord;
    
    [Networked] public int OwnerInt { get; set; }   //Real owner (host controlled)
    
    private bool localSelected = false;

    private Material originalMaterial;
    private NetworkSideManager sideManager;

    private int _lastOwnerInt = -999;
    private bool _lastSelected = false;
    
    private void Awake()
    {
        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        originalMaterial = tileRenderer.material;
    }

    public override void Spawned()
    {
        // Auto-detect coordinate from world position
        HexCoord = NetworkHexGridManager.Instance.WorldToHex(transform.position);

        sideManager = FindAnyObjectByType<NetworkSideManager>();

        //Host sets the rel owner
        if (Object.HasStateAuthority)
        {
            OwnerInt = (int)initialOwnerSide;
        }

        _lastOwnerInt = OwnerInt;
        
        ApplyOwnerVisual();
        UpdateLocalSelectionVisual();
        
        // Register tile in hex grid manager
        NetworkHexGridManager.Instance.RegisterHex(HexCoord, gameObject);
    }

    public override void FixedUpdateNetwork()
    {
        if (_lastOwnerInt != OwnerInt)
        {
            _lastOwnerInt = OwnerInt;
            ApplyOwnerVisual();

            NetworkHexGridManager.Instance?.NotifyOwnerChanged();
        }
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
            Debug.Log($"[NetworkTile] Applying selection material to {name}");
            tileRenderer.material = selectionMaterial;
        }
        else
        {
            Debug.Log($"[NetworkTile] Applying owner visual to {name} (localSelected: {localSelected}, selectionMaterial: {selectionMaterial != null})");
          //  tileRenderer.material = originalMaterial;
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
        TileSelectionManager.Instance?.Local_SelectTile(this);
    }

    // --------------------------------------------------------------------
    // NETWORK BUILD REQUEST
    // --------------------------------------------------------------------
    public void RequestBuild(string buildingName)
    {
        Debug.Log($"[NetworkTile] RequestBuild called on {name} for {buildingName}. Object.HasInputAuthority = {Object?.HasInputAuthority.ToString() ?? "null"}; Object.Id = {(Object != null ? Object.Id.ToString() : "null")}");

        NetworkBuildingManager.Instance.RequestBuild(this, buildingName);
    }

 
    /*
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestBuild(string buildingName)
    {
        Debug.Log($"[NetworkTile] RPC_RequestBuild invoked on StateAuthority for tile {name} with buildingName: {buildingName}. LocalPlayer = {Runner.LocalPlayer}");
        NetworkBuildingManager.Instance?.SpawnBuildingOnTile(this, buildingName);
    }*/

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
}