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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        }
    }
}
