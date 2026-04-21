using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelPanelManager : MonoBehaviour
{
    public static MapLevelPanelManager Instance { get; private set; }
    [SerializeField] private List<MapLevelDataSO> mapLevelDataSO;
    [SerializeField] private MapLevelBlock mapLevelBlockPrefab;
    [SerializeField] private ToggleGroup mapLevelBlockContainer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mapLevelDataSO.Sort((x, y) => x.level.CompareTo(y.level));
    }

    private void Start()
    {
        for (int i = 0; i < mapLevelDataSO.Count; i++)
        {
            var mapLevelBlock = Instantiate(mapLevelBlockPrefab, mapLevelBlockContainer.transform);
            mapLevelBlock.Initialize(i + 1, mapLevelBlockContainer, mapLevelDataSO[i]);
        }
    }

    private void OnValidate()
    {
        mapLevelDataSO.Sort((x, y) => x.level.CompareTo(y.level));
    }

}
