using UnityEngine;

[CreateAssetMenu(fileName = "Map Level Data", menuName = "Level Data/Map Level Data")]
public class MapLevelDataSO : ScriptableObject
{
    [field: SerializeField] public int level { get; private set; }
    public GameObject environmentPrefab;
    public Material tileMaterial;
    public TileModificationData[] tileModificationData;
}