// using UnityEngine;
// using UnityEngine.EventSystems; // needed for IsPointerOverGameObject

// public class TileClickManager : MonoBehaviour
// {
//     public TileUIPanel tileUIPanel; // assign in Inspector
//     private Camera mainCamera;

//     void Start()
//     {
//         mainCamera = Camera.main;
//         if (mainCamera == null)
//             Debug.LogError("MainCamera not found! Make sure your camera has the 'MainCamera' tag.");

//         if (tileUIPanel != null)
//             tileUIPanel.Close(); // hide at start
//     }

//     void Update()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             // Skip raycast if pointer is over UI
//             if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
//             {
//                 Debug.Log("Click ignored: pointer is over UI");
//                 return;
//             }

//             Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out RaycastHit hit))
//             {
//                 Debug.Log("Raycast hit: " + hit.collider.name);

//                 Tile tile = hit.collider.GetComponent<Tile>();
//                 if (tile == null)
//                     tile = hit.collider.GetComponentInParent<Tile>();

//                 if (tile != null)
//                 {
//                     Debug.Log("Found Tile script on: " + tile.name + " | isOpen = " + tile.isOpen);

//                     if (tile.isOpen)
//                         tileUIPanel.Open(tile);
//                     else
//                         tileUIPanel.Close();
//                 }
//             }
//         }
//     }
// }



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
