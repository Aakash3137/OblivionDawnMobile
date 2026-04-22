using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelPanelManager : MonoBehaviour
{
    public static MapLevelPanelManager Instance { get; private set; }
    [SerializeField] private MapLevelBlock mapLevelBlockPrefab;
    [SerializeField] private ToggleGroup mapLevelBlockContainer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < GameData.totalLevels; i++)
        {
            var mapLevelBlock = Instantiate(mapLevelBlockPrefab, mapLevelBlockContainer.transform);
            mapLevelBlock.Initialize(i + 1, mapLevelBlockContainer);
        }
    }
}
