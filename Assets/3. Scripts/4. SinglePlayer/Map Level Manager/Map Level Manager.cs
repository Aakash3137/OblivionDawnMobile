using System.Collections.Generic;
using UnityEngine;

public class MapLevelManager : MonoBehaviour
{
    public static MapLevelManager Instance { get; private set; }

    [SerializeField] private List<MapLevelDataSO> allMapLevelDataSO;
    [SerializeField] private GameObject ENVIRONMENT_CONTAINER;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        allMapLevelDataSO.Sort((x, y) => x.level.CompareTo(y.level));
    }

    private void Start()
    {
        var levelData = allMapLevelDataSO[GameData.mapLevel - 1];

        if (levelData.environmentPrefab != null)
            Instantiate(levelData.environmentPrefab, ENVIRONMENT_CONTAINER.transform);

        CubeGridManager.Instance.Initialize(levelData);
    }

    private void OnValidate()
    {
        allMapLevelDataSO.Sort((x, y) => x.level.CompareTo(y.level));
    }
}
