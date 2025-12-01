using Fusion;
using UnityEngine;

public class TileSelectionManager : NetworkBehaviour
{
    public static TileSelectionManager Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;
    
    [Networked] private NetworkId SelectedTileId { get; set; }
    
    private Camera _playerCamera;
    private NetworkTile _selectedTile;
    private Material _originalMaterial;
    private bool _isInitialized = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void Initialize(Camera playerCamera)
    {
        _playerCamera = playerCamera;
        
        if (_playerCamera == null)
        {
            Debug.LogError("[TileSelectionManager] Player camera not assigned!");
            return;
        }
        
        _isInitialized = true;
        Debug.Log($"[TileSelectionManager] Initialized with camera: {playerCamera.name}");
    }
    
    public override void FixedUpdateNetwork()
    {
        // Only local player processes input
        if (!Object.HasInputAuthority) return;
        
        // Check if we're in GameScene before processing input
        if (!IsInGameScene()) return;
        
        // Check if initialized
        if (!_isInitialized || _playerCamera == null) return;
        
        // Mobile touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ProcessTouch(Input.GetTouch(0).position);
        }
        
        // Mouse input for testing (optional)
        if (Input.GetMouseButtonDown(0))
        {
            ProcessTouch(Input.mousePosition);
        }
    }
    
    private bool IsInGameScene()
    {
        // Check if we're in the GameScene
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return sceneName.Equals("GameScene", System.StringComparison.OrdinalIgnoreCase);
    }
    
    private void ProcessTouch(Vector2 screenPosition)
    {
        if (!_isInitialized || _playerCamera == null)
        {
            Debug.LogError("[TileSelectionManager] Not initialized!");
            return;
        }
        
        Ray ray = _playerCamera.ScreenPointToRay(screenPosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayerMask))
        {
            NetworkTile tile = hit.collider.GetComponent<NetworkTile>();
            if (tile != null)
            {
                Debug.Log($"[TileSelectionManager] Tile clicked: {tile.Object.Id}");
                tile.HandleClick();
            }
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestSelectTile(NetworkId tileId, RpcInfo info = default)
    {
        // Server validates and sets selection
        SelectedTileId = tileId;
        
        // Also inform all clients about selection
        RPC_OnTileSelected(tileId);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnTileSelected(NetworkId tileId, RpcInfo info = default)
    {
        // All clients update visual selection
        UpdateTileSelectionVisual(tileId);
    }
    
    private void UpdateTileSelectionVisual(NetworkId tileId)
    {
        // Deselect previous tile
        if (_selectedTile != null && _selectedTile.Object != null)
        {
            ResetTileVisual(_selectedTile);
        }
        
        // Check if we're clearing selection (tileId is default/empty)
        if (tileId == default)
        {
            _selectedTile = null;
            return;
        }
        
        // Find and select new tile
        var tileObject = Runner.FindObject(tileId);
        if (tileObject != null)
        {
            _selectedTile = tileObject.GetComponent<NetworkTile>();
            if (_selectedTile != null)
            {
                SetTileAsSelected(_selectedTile);
                Debug.Log($"[TileSelectionManager] Tile selected: {_selectedTile.Object.Id}");
                
                // Notify other systems
                OnTileSelected(_selectedTile);
            }
        }
    }
    
    private void SetTileAsSelected(NetworkTile tile)
    {
        if (tile.tileRenderer != null)
        {
            // Store original material if not already stored
            if (_originalMaterial == null)
            {
                _originalMaterial = tile.tileRenderer.material;
            }
            
            // Apply selection material
            if (selectedMaterial != null)
            {
                tile.tileRenderer.material = selectedMaterial;
            }
            
            // Add glowing effect or outline
            tile.transform.localScale *= 1.1f; // Simple visual feedback
        }
    }
    
    private void ResetTileVisual(NetworkTile tile)
    {
        if (tile.tileRenderer != null)
        {
            // Restore original material or use default
            if (_originalMaterial != null)
            {
                tile.tileRenderer.material = _originalMaterial;
            }
            else if (defaultMaterial != null)
            {
                tile.tileRenderer.material = defaultMaterial;
            }
            
            tile.transform.localScale = Vector3.one;
        }
    }
    
    private void OnTileSelected(NetworkTile tile)
    {
        // This can be used by CardSystem or other managers
        Debug.Log($"[TileSelectionManager] Tile {tile.Object.Id} selected at position: {tile.transform.position}");
        
        // Example: If you have a CardSystem, notify it
        // CardSystem.Instance?.OnTileSelected(tile);
    }
    
    // Public API for other systems
    public NetworkTile GetSelectedTile()
    {
        return _selectedTile;
    }
    
    public bool HasSelectedTile()
    {
        return _selectedTile != null;
    }
    
    public void ClearSelection()
    {
        if (_selectedTile != null)
        {
            ResetTileVisual(_selectedTile);
            _selectedTile = null;
            
            // Network sync - clear selection for all
            if (Object.HasStateAuthority)
            {
                SelectedTileId = default;
                RPC_OnTileSelected(default);
            }
        }
    }
    
    // Handle scene changes
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"[TileSelectionManager] Scene changed to: {scene.name}");
        
        // Clear selection when leaving GameScene
        if (!scene.name.Equals("GameScene", System.StringComparison.OrdinalIgnoreCase))
        {
            ClearSelection();
        }
    }
    
    public override void Spawned()
    {
        base.Spawned();
        
        // Subscribe to scene changes
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Unsubscribe from scene changes
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        base.Despawned(runner, hasState);
    }
}