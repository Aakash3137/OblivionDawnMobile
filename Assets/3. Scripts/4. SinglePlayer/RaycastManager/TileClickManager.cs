using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickManager : MonoBehaviour
{
    public BuildPanelManager buildPanel; // assign in Inspector
    private Camera mainCamera;
    [SerializeField] private LayerMask tileLayerMask;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("MainCamera not found! Make sure your camera has the 'MainCamera' tag.");
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    HandleBuildPanel();
                }
            }
        }
        // EDITOR / DESKTOP (mouse)
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                HandleBuildPanel();
            }
        }
    }
    void HandleBuildPanel()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, tileLayerMask))
        {
            Tile tile = hit.collider.GetComponentInParent<Tile>();

            if (tile == null)
            {
                buildPanel.CloseBuildPanel();
                return;
            }

            if (tile.isOpen && tile.ownerSide == Side.Player)
                buildPanel.OpenBuildPanel(tile);
            else
                buildPanel.CloseBuildPanel();
        }
        else
        {
            buildPanel.CloseBuildPanel();
        }
    }
}
