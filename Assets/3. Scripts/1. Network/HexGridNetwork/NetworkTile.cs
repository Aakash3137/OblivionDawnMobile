using Fusion;
using UnityEngine;
public enum NetworkSide : int { None = 0, Player = 1, Enemy = 2 }

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(BoxCollider))]
public class NetworkTile : NetworkBehaviour
{
    [Header("Ownership")]
    public NetworkSide initialOwnerSide;
    
    [Header("Debug - Current Visual Owner")]
    [SerializeField] private NetworkSide currentVisualOwner;
    
    // Public property to check tile ownership from other scripts
    public NetworkSide CurrentVisualOwner => currentVisualOwner;
    public bool IsPlayerTile => currentVisualOwner == NetworkSide.Player;
    public bool IsEnemyTile => currentVisualOwner == NetworkSide.Enemy;
    public bool IsNeutralTile => currentVisualOwner == NetworkSide.None;

    [Header("Visuals")]
    public Renderer tileRenderer;
    
    [Header("Selection")]
    public Material selectionMaterial;  // ADD THIS: Assign a yellow material in Inspector
    public float selectionScale = 1.1f;

    [Networked] public int OwnerInt { get; set; }
    [Networked] public NetworkBool IsSelected { get; set; }

    private int _lastOwnerInt = -999;
    private bool _lastSelected = false;
    private NetworkSideManager _sideManager;
    private Material _originalMaterial;  // CHANGE: Store original material instead of color
    private Vector3 _originalScale;

    private void Awake()
    {
        // Add BoxCollider if missing
        if (GetComponent<BoxCollider>() == null)
            gameObject.AddComponent<BoxCollider>();
        
        // Set collider size
        GetComponent<BoxCollider>().size = new Vector3(1.5f, 0.1f, 1.5f);
        
        // Store original values
        _originalScale = transform.localScale;
        
        if (tileRenderer != null)
        {
            _originalMaterial = tileRenderer.material;  // Store original material
        }
    }

    public override void Spawned()
    {
        base.Spawned();

        _sideManager = FindAnyObjectByType<NetworkSideManager>();

        if (NetworkHexGridManager.Instance != null)
        {
            var coord = NetworkHexGridManager.Instance.WorldToHex(transform.position);
            NetworkHexGridManager.Instance.RegisterHex(coord, gameObject);
        }

        if (Object.HasStateAuthority)
        {
            OwnerInt = (int)initialOwnerSide;
        }

        _lastOwnerInt = OwnerInt;
        ApplyOwnerVisual();
        
        _lastSelected = IsSelected;
        UpdateSelectionVisual();
    }

    public override void FixedUpdateNetwork()
    {
        if (_lastOwnerInt != OwnerInt)
        {
            _lastOwnerInt = OwnerInt;
            ApplyOwnerVisual();
            NetworkHexGridManager.Instance?.NotifyOwnerChanged();
        }
        
        if (_lastSelected != IsSelected)
        {
            _lastSelected = IsSelected;
            UpdateSelectionVisual();
        }
    }

    private void ApplyOwnerVisual()
    {
        if (_sideManager == null || tileRenderer == null)
            return;

        NetworkSide tileEnum = (NetworkSide)OwnerInt;
        
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
            currentVisualOwner = tileEnum;
            _sideManager.SetSide(tileRenderer, tileEnum);
            return;
        }

        float playerZ = localPlayer.transform.position.z;
        int spawnId = playerZ < 15f ? 0 : 1;
        
        // SpawnId 0: Keep tiles as-is
        if (spawnId == 0)
        {
            currentVisualOwner = tileEnum;
            _sideManager.SetSide(tileRenderer, tileEnum);
        }
        // SpawnId 1: Swap Player/Enemy
        else
        {
            NetworkSide visualSide = tileEnum;
            if (tileEnum == NetworkSide.Player)
                visualSide = NetworkSide.Enemy;
            else if (tileEnum == NetworkSide.Enemy)
                visualSide = NetworkSide.Player;
            
            currentVisualOwner = visualSide;
            _sideManager.SetSide(tileRenderer, visualSide);
        }
    }
    
    private void UpdateSelectionVisual()
    {
        if (tileRenderer != null)
        {
            if (IsSelected)
            {
                // Apply selection visual - CHANGE THIS PART
                if (selectionMaterial != null)
                {
                    tileRenderer.material = selectionMaterial;  // Swap to yellow material
                }
                transform.localScale = _originalScale * selectionScale;
            }
            else
            {
                // Restore original scale
                transform.localScale = _originalScale;
                
                ApplyOwnerVisual();
            }
        }
    }

    // Click handler called from TileSelectionManager
    public void HandleClick()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!sceneName.Equals("GameScene", System.StringComparison.OrdinalIgnoreCase))
            return;
            
        if (Object.HasInputAuthority) 
        {
            Debug.Log($"[NetworkTile] Clicked by local player, toggling selection");
            RPC_RequestSetSelected(!IsSelected);
            
            // TODO: Card system integration
            // if (NetworkCardManager.Instance != null)
            // {
            //     NetworkCardManager.Instance.OnTileClicked(this);
            // }
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSetSelected(bool selected, RpcInfo info = default)
    {
        IsSelected = selected;
        
        // If selecting this tile, deselect others
        if (selected)
        {
            DeselectOtherTiles(Object.Id);
        }
    }
    
    // TODO: Implement when card system is ready
    // [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    // public void RPC_RequestSpawnCard(string cardName, RpcInfo info = default)
    // {
    //     if (NetworkCardManager.Instance == null) return;
    //     
    //     CardData card = NetworkCardManager.Instance.GetCardByName(cardName);
    //     if (card == null) return;
    //     
    //     NetworkCardManager.Instance.SpawnCardOnTile(this, card, info.Source, Runner);
    // }
    
    private void DeselectOtherTiles(NetworkId currentTileId)
    {
        var allTiles = FindObjectsOfType<NetworkTile>();
        foreach (var tile in allTiles)
        {
            if (tile.Object.Id != currentTileId && tile.IsSelected)
            {
                tile.IsSelected = false;
            }
        }
    }
    
    // Debug method
    public void DebugTileInfo()
    {
        string side = ((NetworkSide)OwnerInt).ToString();
      //  string visualSide = GetVisualSide().ToString();
        string selected = IsSelected ? "SELECTED" : "NOT SELECTED";
    //    Debug.Log($"[Tile Debug] ID: {Object.Id}, Network Owner: {side}, Visual: {visualSide}, Selected: {selected}");
    }
}