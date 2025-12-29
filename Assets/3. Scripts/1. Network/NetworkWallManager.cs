using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;

public class NetworkWallManager : NetworkBehaviour
{
    [SerializeField] private WallParent _wallPrefab;
    
    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;

    public static NetworkWallManager Instance;
    internal void Awake()
    {
      Instance = this;  
    }

    internal void PlaceWalls(Transform currentTile)
    {
        Vector3 _currentTileCords = currentTile.transform.position;

        List<Vector2Int> adjacentTileCords = NetworkCubeGridManager.Instance.GetCardinalNeighbors(
            new Vector2Int((int)(_currentTileCords.x / NetworkCubeGridManager.Instance.cellSize),
                           (int)(_currentTileCords.z / NetworkCubeGridManager.Instance.cellSize)));

        List<NetworkTile> adjacentTiles = new List<NetworkTile>();

        foreach (var cord in adjacentTileCords)
        {
            if (NetworkCubeGridManager.Instance.GetCube(cord) == null)
                adjacentTiles.Add(new GameObject().AddComponent<NetworkTile>());
            else
                adjacentTiles.Add(NetworkCubeGridManager.Instance.GetCube(cord).GetComponent<NetworkTile>());
        }

        WallParent currentWall = Instantiate(_wallPrefab,
            new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z),
            Quaternion.identity, currentTile.transform);

        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            if (adjacentTiles[i].CurrentVisualOwner == NetworkSide.Enemy)
                continue;

            if (adjacentTiles[i].IsOccupied)
            {
                switch (i)
                {
                    case 0:
                        currentWall.DisableWall(0);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(1);
                        break;
                    case 1:
                        currentWall.DisableWall(1);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(0);
                        break;
                    case 2:
                        currentWall.DisableWall(2);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(3);
                        break;
                    case 3:
                        currentWall.DisableWall(3);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(2);
                        break;
                }
            }
        }
    }

    internal void PlaceWallsOnMainBuilding(Vector3 pos, Transform mainTile, PlayerRef owner)
    {
       // if (_mainWallPlaced) return;
       Debug.Log("Placing main building wall at :" +mainTile.position);
        Transform mainBuildingTile = mainTile;
        
        Runner.Spawn(_wallPrefab.gameObject, new Vector3(pos.x, _wallYOffset, pos.z), Quaternion.identity, owner,
            (runner, obj) =>
            {
                obj.transform.SetParent(mainBuildingTile, true);
            }
        );

        /*Instantiate(_wallPrefab,
            new Vector3(pos.x, _wallYOffset, pos.z),
            Quaternion.identity, mainBuildingTile);
            */

       // _mainWallPlaced = true;
    }
}
