using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Map Level Data", menuName = "Level Data/Map Level Data")]
public class MapLevelDataSO : ScriptableObject
{
    [field: SerializeField] public int level { get; private set; }
    
    // ── Addressable references
    public AssetReferenceGameObject environmentPrefabRef;
    public AssetReferenceT<Texture2D> tileTextureRef;
 
    public TileModificationData[] tileModificationData;
}