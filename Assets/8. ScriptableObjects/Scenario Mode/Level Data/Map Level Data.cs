using UnityEngine;

[CreateAssetMenu(fileName = "Map Level Data", menuName = "Level Data/Map Level Data")]
public class MapLevelData : ScriptableObject
{
    public GameObject environmentPrefab;
    public Material tileMaterial;
    public TileModificationData[] tileModificationData;
}