using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyTileClickManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private EnemyBuildPanel enemyBuildPanel;
    [SerializeField] private LayerMask tileLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
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
                    return;
                }

                if (tile.ownerSide == Side.Enemy)
                    enemyBuildPanel.OpenBuildPanel(tile);
                else
                    enemyBuildPanel.CloseBuildPanel();
            }
            else
            {
                enemyBuildPanel.CloseBuildPanel();
            }
        }
    }
}
