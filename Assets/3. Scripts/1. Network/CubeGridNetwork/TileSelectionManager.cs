using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelectionManager : MonoBehaviour
{
    public static TileSelectionManager Instance;

    public NetworkTile selectedTile;
    private Camera playerCamera;

    public LayerMask tileMask;

    private void Awake()
    {
        Instance = this;
        tileMask = LayerMask.GetMask("CubeTile");
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

    public void TrySelectTile(NetworkTile tile)
    {
        if (tile == null)
            return;

        if (tile.IsOccupied)
        {
            Debug.Log($"[TSM] Tile {tile.name} is occupied.");
            return;
        }

        if (tile.CurrentVisualOwner != NetworkSide.Player)
        {
            Debug.Log($"[TSM] Tile {tile.name} visual owner is {tile.CurrentVisualOwner}. Selection denied.");
            return;
        }

        if (selectedTile == tile)
        {
            selectedTile.SetLocalSelected(false);
            selectedTile = null;
            return;
        }

        Local_SelectTile(tile);
    }
    
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
        // -----------------------------------------------------------
        // BLOCK TILE CLICKS IF TOUCH IS OVER UI WITH UIRaycastBlocker
        // -----------------------------------------------------------
        if (IsPointerOverBlockingUI(pos))
        {
            Debug.Log("[TSM] Touch is over UI that blocks raycast");
            return;
        }

        // Raycast to tiles only
        Ray ray = playerCamera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileMask))
        {
            Debug.Log($"[TSM] Raycast hit: {hit.collider.name} at {hit.point}");

            var tile = hit.collider.GetComponent<NetworkTile>();
            if (tile != null)
            {
                Debug.Log($"[TSM] NetworkTile found: {tile.name}, attempting selection");
                // NOTE: use TrySelectTile which handles occupied/ownership/toggle logic
                TrySelectTile(tile);
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

    // -----------------------------------------------------------
    // RETURN TRUE if finger/mouse is over a UI element that has
    // the "UIRaycastBlocker" component
    // -----------------------------------------------------------
    private bool IsPointerOverBlockingUI(Vector2 pos)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = pos;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<UIRaycastBlocker>() != null)
                return true;
        }

        return false;
    }


}
