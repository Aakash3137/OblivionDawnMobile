using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Fusion;

public class RepairButtonHandler : MonoBehaviour
{
    [Header("Target Positions")]
    [SerializeField] private Vector2 showPosition = new Vector2(0, 0.75f);
    [SerializeField] private Vector2 hidePosition = new Vector2(0, -1f);

    [Header("Scale")]
    public Vector3 showScale = Vector3.one;
    public Vector3 hideScale = Vector3.zero;

    [Header("Animation")]
    public float duration = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float CoolDownTimer = 15f;
    [Header ("UI")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private Button Repairbtn;

    [Header ("Data")]
    [SerializeField] internal Stats StatsData;
    [SerializeField] internal bool IsReady;


    [Header ("Wall Data")]
    private Tile currentTile;
    private float _wallYOffset = 1f;

    [SerializeField] private WallParent _wallPrefab, CurrentWall;
    private BuildingStats placedBuilding;
    private bool _mainWallPlaced = false;
    public BuildingPlacementHelper BuildingHelper;

    [Header ("Effects")]
    public GameObject GlowEffectPrefab;
    public Transform EffectPlace;
    void OnEnable()
    {
        PlayHide();
        IsReady = true;
    }

    void Start()
    {
        CheckIFEnemy();
        Repairbtn.onClick.RemoveAllListeners();
        Repairbtn.onClick.AddListener(OnClickRepairBtn);
    }

    private void OnMouseDown() 
    {
        if(!IsReady)
           return;

        // Debug.Log("On Click Repair Button");
        StatsData.HealthRepair();
        Repairbtn.gameObject.SetActive(false);
        StartCoroutine(CoolDownTimerStart());
    }

    public void PlayShow()
    {
        if(!CheckIFEnemy() || !IsReady)
            return;
        
        // Debug.Log("Play Show");
        Repairbtn.gameObject.SetActive(true);
       StartCoroutine(Animate(showPosition, showScale));
    }

    public void PlayHide()
    {
        StartCoroutine(HideRoutine());
    }

    bool CheckIFEnemy()
    {
        if(StatsData.side == Side.Enemy)
        {
            gameObject.SetActive(false);
            return false;
        }
        return true;
    }

    IEnumerator HideRoutine()
    {
        Repairbtn.gameObject.SetActive(false);
        yield return Animate(hidePosition, hideScale);
    }

    IEnumerator Animate(Vector2 targetPos, Vector3 targetScale)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = curve.Evaluate(time / duration);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        
        rect.anchoredPosition = targetPos;
        rect.localScale = targetScale;
    }

    [Button]
    public void OnClickRepairBtn()
    {
        if(!IsReady)
           return;

        Debug.Log("On Click Repair Button");
        StatsData.HealthRepair();
        Repairbtn.gameObject.SetActive(false);
        StartCoroutine(CoolDownTimerStart());
    }

    IEnumerator CoolDownTimerStart()
    {
        IsReady = false;
        EffectPlace.localPosition = new Vector3(0, -0.55f, 0);
        GameObject _Effect = Instantiate(GlowEffectPrefab, EffectPlace);
        _Effect.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(CoolDownTimer);
        // Destroy(_Effect, CoolDownTimer);
        IsReady = true;
    }

    #region Repair Wall
    private void PlaceWalls()
    {
        currentTile = BuildingHelper.currentTile;
        Vector3 _currentTileCords = currentTile.transform.position;
        var cgmInstance = CubeGridManager.Instance;
        Vector2Int currentGrid = cgmInstance.WorldToGrid(_currentTileCords);
 
        List<Vector2Int> adjacentTileCords = cgmInstance.GetCardinalNeighbors(currentGrid);

        Tile[] adjacentTiles = new Tile[4]; 
 
        adjacentTiles[0] = cgmInstance.GetTile(adjacentTileCords[0]);
        adjacentTiles[1] = cgmInstance.GetTile(adjacentTileCords[1]);
        adjacentTiles[2] = cgmInstance.GetTile(adjacentTileCords[2]);
        adjacentTiles[3] = cgmInstance.GetTile(adjacentTileCords[3]); 

        var spawnPos = new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z);
        
        if(CurrentWall!= null)
            Destroy(CurrentWall.gameObject);
        
        CurrentWall = Instantiate(_wallPrefab, spawnPos, Quaternion.identity, placedBuilding.transform);

        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null || adjacentTiles[i].ownerSide == Side.Enemy)
                continue;

            if (adjacentTiles[i].hasBuilding)
            {
                switch (i)
                {
                    case 0:
                        CurrentWall.DisableWall(0);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(1);
                        break;
                    case 1:
                        CurrentWall.DisableWall(1);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(0);
                        break;
                    case 2:
                        CurrentWall.DisableWall(2);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(3);
                        break;
                    case 3:
                        CurrentWall.DisableWall(3);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(2);
                        break;
                }
            }
        }
    }

    private void PlaceWallsOnMainBuilding()
    {
        if (_mainWallPlaced) return;

        Transform mainBuildingTile = GameManager.Instance.playerSpawnPoint;
        Transform mainBuilding = null;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("MainBuilding");

        foreach (var obj in objects)
        {
            if (obj.GetComponent<BuildingStats>().side == Side.Player)
            {
                mainBuilding = obj.transform;
                break;
            }
        }

        if (mainBuilding != null)
            Instantiate(_wallPrefab,
                new Vector3(mainBuildingTile.position.x, _wallYOffset, mainBuildingTile.position.z),
                Quaternion.identity, mainBuilding);
        else
            Debug.Log("Main building not found");

        _mainWallPlaced = true;
    }
    #endregion
}


public static class InputHelper
{
    public static bool IsPointerOverUI()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        return EventSystem.current.IsPointerOverGameObject();
    }
}