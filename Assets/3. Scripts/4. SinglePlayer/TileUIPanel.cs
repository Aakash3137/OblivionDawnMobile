using UnityEngine;
using TMPro;

public class TileUIPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text tileInfoText;
    public CanvasGroup canvasGroup;

    [Header("Building Prefabs (indexed)")]
    public GameObject[] buildingPrefabs; // assign in Inspector

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

        // Spawn at tile center (adjust Y or parent as needed)
        Vector3 spawnPos = currentTile.transform.position;
        Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity);

        // Mark tile occupied
        currentTile.isOpen = false;

        // Optional: hide PlusIcon if present
        Transform cubeChild = currentTile.transform.Find("Cube");
        if (cubeChild != null)
        {
            Transform plusIcon = cubeChild.Find("Plus_Icon");
            if (plusIcon != null) plusIcon.gameObject.SetActive(false);
        }

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

    // Optional: hook your close button to this for clarity
    public void OnCloseButtonClicked() => Close();
}
