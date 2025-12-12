using UnityEngine;

public enum BuildingSide
{
    Blue,
    Red
}

public class NetworkBuildingSide : MonoBehaviour
{
    public BuildingSide side;   // choose in Inspector

    /*public NetworkMainBuilding mainBuilding;  // drag the manager here

    // Inspector button
    [ContextMenu("Apply Side To Main Building")]
    void ApplySide()
    {
        if (mainBuilding != null)
            mainBuilding.SelectMainBuilding(side);
        else
            Debug.LogWarning("MainBuilding reference missing!");
    }*/
}