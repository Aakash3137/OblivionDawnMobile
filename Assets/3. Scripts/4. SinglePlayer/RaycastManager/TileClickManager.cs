using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickManager : MonoBehaviour
{
    public FloatingUIManager floatingUIManager; // assign in Inspector
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("MainCamera not found! Make sure your camera has the 'MainCamera' tag.");

        if (floatingUIManager != null)
            floatingUIManager.CloseUI(); // hide at start
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Block clicks on UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile == null)
                    tile = hit.collider.GetComponentInParent<Tile>();

                if (tile != null)
                {
                    if (tile.isOpen && tile.ownerSide == Side.Player)
                        floatingUIManager.ShowUI(tile);
                    else
                        floatingUIManager.CloseUI();
                }
            }
        }
    }
}
