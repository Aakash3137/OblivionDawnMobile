using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelectionManager : MonoBehaviour
{
    public static TileSelectionManager Instance;

    public NetworkTile selectedTile;
    private Camera playerCamera;

    public LayerMask tileMask;   // Leave uninitialized here !!!

    private void Awake()
    {
        Instance = this;

        tileMask = LayerMask.GetMask("HexTile");
    }

    public void Initialize(Camera cam)
    {
        playerCamera = cam;
    }

    public void Local_SelectTile(NetworkTile tile)
    {
        Debug.Log($"[TSM] Local_SelectTile called for: {tile.name}");
        
        if (selectedTile != null)
        {
            Debug.Log($"[TSM] Deselecting previous tile: {selectedTile.name}");
            selectedTile.SetLocalSelected(false);
        }

        selectedTile = tile;

        if (selectedTile != null)
        {
            Debug.Log($"[TSM] Setting new tile as selected: {selectedTile.name}");
            selectedTile.SetLocalSelected(true);
        }

        Debug.Log($"[TSM] Selection complete: {tile.name} (ID: {tile.Object.Id})");
    }

    public NetworkTile GetSelectedTile()
    {
        return selectedTile;
    }

    // Player presses BUILD button
    public void TryBuild(string buildingName)
    {
        if (selectedTile == null) return;

        selectedTile.RequestBuild(buildingName);
    }


    // ---- INPUT ----
    private void Update()
    {
        if (playerCamera == null) return;
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("GameScene")) return;

        if (Input.GetMouseButtonDown(0))
            ProcessInput(Input.mousePosition);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            ProcessInput(Input.GetTouch(0).position);
    }

    private void ProcessInput(Vector2 pos)
    {
        // Raycast to tiles only
        Ray ray = playerCamera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileMask))
        {
            Debug.Log($"[TSM] Raycast hit: {hit.collider.name} at {hit.point}");

            var tile = hit.collider.GetComponent<NetworkTile>();
            if (tile != null)
            {
                Debug.Log($"[TSM] NetworkTile found: {tile.name}, calling HandleClick");
                tile.HandleClick();
            }
            else
            {
                Debug.Log($"[TSM] No NetworkTile component on {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log($"[TSM] Raycast missed (tileMask: {tileMask.value})");
        }
    }
    
   
}