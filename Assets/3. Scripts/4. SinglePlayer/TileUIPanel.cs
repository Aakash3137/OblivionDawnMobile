// using UnityEngine;
// using TMPro;

// public class TileUIPanel : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public TMP_Text tileInfoText;
//     public CanvasGroup canvasGroup;

//     [Header("Building Prefabs (indexed)")]
//     public GameObject[] buildingPrefabs; // assign in Inspector

//     private Tile currentTile;

//     public void Open(Tile tile)
//     {
//         currentTile = tile;

//         if (tileInfoText != null)
//             tileInfoText.text = $"Tile: {tile.name}\nOwner: {tile.ownerSide}";

//         ShowPanel(true);
//     }

//     public void Close()
//     {
//         currentTile = null;
//         ShowPanel(false);
//     }

//     public void PlaceBuilding(int prefabIndex)
//     {
//         if (currentTile == null) return;
//         if (prefabIndex < 0 || prefabIndex >= buildingPrefabs.Length) return;

//         // Prevent double placement
//         if (currentTile.hasBuilding) return;

//         // Spawn at tile center (adjust Y if needed)
//         Vector3 spawnPos = currentTile.transform.position;
//         spawnPos.y += 2f; // adjust depending on prefab pivot

//         Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

//         // Mark tile as having a building
//         currentTile.SetBuildingPlaced();

//         Close();
//     }

//     private void ShowPanel(bool show)
//     {
//         if (canvasGroup != null)
//         {
//             canvasGroup.alpha = show ? 1f : 0f;
//             canvasGroup.interactable = show;
//             canvasGroup.blocksRaycasts = show;
//         }
//         else
//         {
//             gameObject.SetActive(show);
//         }
//     }
//     public void OnCloseButtonClicked() => Close();
// }




















using UnityEngine;
using TMPro;

public class TileUIPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text tileInfoText;
    public CanvasGroup canvasGroup;

    [Header("Building Prefabs (placeholders with UnitSide)")]
    public GameObject[] buildingPrefabs; // assign in Inspector (empty prefabs with UnitSide attached)

    private Tile currentTile;

    public void Open(Tile tile)
    {
        currentTile = tile;

        if (tileInfoText != null)
            tileInfoText.text = $"Tile: {tile.name}\nOwner: {tile.ownerSide}";

        ShowPanel(true);
    }

    public void Close()
    {
        currentTile = null;
        ShowPanel(false);
    }

    public void PlaceBuilding(int prefabIndex)
    {
        if (currentTile == null) return;
        if (prefabIndex < 0 || prefabIndex >= buildingPrefabs.Length) return;
        if (currentTile.hasBuilding) return;

        Vector3 spawnPos = currentTile.transform.position;
        spawnPos.y += 2f;

        // Instantiate the placeholder prefab
        var placeholder = Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

        // Inject UnitSide info
        var unitSide = placeholder.GetComponent<UnitSide>();
        if (unitSide != null)
        {
            unitSide.side = currentTile.ownerSide;

            // Force UnitSide to inject the correct prefab immediately
            unitSide.Refresh();
        }
        else
        {
            Debug.LogError("[TileUIPanel] Building prefab must have UnitSide attached!");
        }

        currentTile.SetBuildingPlaced();
        Close();
    }

    private void ShowPanel(bool show)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = show ? 1f : 0f;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }
        else
        {
            gameObject.SetActive(show);
        }
    }

    public void OnCloseButtonClicked() => Close();
}
