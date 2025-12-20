using UnityEngine;

public class PrefabStoreProvider : MonoBehaviour
{
    [SerializeField] private FactionPrefabStore storeAsset;

    private void Awake()
    {
        GameData.PrefabStore = storeAsset;
    }
}
