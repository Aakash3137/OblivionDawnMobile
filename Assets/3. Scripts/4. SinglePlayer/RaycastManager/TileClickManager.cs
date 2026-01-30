using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickManager : MonoBehaviour
{
    public BuildPanelManager buildPanel; // assign in Inspector
    private Camera mainCamera;
    [SerializeField] private LayerMask tileLayerMask;
    
    // Adding this functionality
    // If user drags → NEVER open build panel
    // If user taps and releases without dragging → open build panel 
    
    [SerializeField] private float dragThreshold = 15f; // pixels
    private Vector2 pointerDownPos;
    private bool dragDetected;

    
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
                if (touch.phase == TouchPhase.Began)
                {
                    pointerDownPos = touch.position;
                    dragDetected = false;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (Vector2.Distance(pointerDownPos, touch.position) > dragThreshold)
                        dragDetected = true;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (!dragDetected)
                        HandleBuildPanel();
                }
            }
        }
        // EDITOR / DESKTOP (mouse)
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointerDownPos = Input.mousePosition;
                dragDetected = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (Vector2.Distance(pointerDownPos, Input.mousePosition) > dragThreshold)
                    dragDetected = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!dragDetected)
                    HandleBuildPanel();
            }
        }
    }
    
    void HandleBuildPanel()
    {
        
        if (CameraPanning.IsDragging)
            return;
        
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
