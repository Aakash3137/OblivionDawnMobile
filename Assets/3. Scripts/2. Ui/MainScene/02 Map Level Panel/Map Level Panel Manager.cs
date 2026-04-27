using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelPanelManager : MonoBehaviour
{
    public static MapLevelPanelManager Instance { get; private set; }
    
    [SerializeField] private MapLevelBlock mapLevelBlockPrefab;
    [SerializeField] private ToggleGroup mapLevelBlockContainer;

    [SerializeField] private List<MapLevelDataSO> allMapLevelDataSO; 
 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
 
    private void Start()
    {
        allMapLevelDataSO.Sort((x, y) => x.level.CompareTo(y.level));
 
        for (int i = 0; i < allMapLevelDataSO.Count; i++)
        {
            var data         = allMapLevelDataSO[i];
            var mapLevelBlock = Instantiate(mapLevelBlockPrefab, mapLevelBlockContainer.transform);
 
            mapLevelBlock.Initialize(data.level, data, mapLevelBlockContainer);
        }
    }
 
    private void OnValidate()
    {
        if (allMapLevelDataSO != null)
            allMapLevelDataSO.Sort((x, y) => x.level.CompareTo(y.level));
    }
}
