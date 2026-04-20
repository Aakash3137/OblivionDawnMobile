using UnityEngine;

public class MapLevelManager : MonoBehaviour
{
    public static MapLevelManager Instance { get; private set; }

    [SerializeField] private GameObject ENVIRONMENT_CONTAINER;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        var levelData = GameData.mapLevelData;

        if (levelData == null)
        {
            levelData = Resources.Load<MapLevelDataSO>("DefaultLevelData");
        }

        if (levelData.environmentPrefab != null)
            Instantiate(levelData.environmentPrefab, ENVIRONMENT_CONTAINER.transform);

        CubeGridManager.Instance.Initialize(levelData);
    }
}
