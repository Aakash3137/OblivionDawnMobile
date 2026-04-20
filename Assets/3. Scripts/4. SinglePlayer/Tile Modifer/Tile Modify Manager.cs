using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TileModifyManager : MonoBehaviour
{
    [SerializeField] private TileEffect[] allTileEffects;

    private List<TileEffect> offenseTileEffects = new();
    private List<TileEffect> defenseTileEffects = new();
    private List<TileEffect> resourceTileEffects = new();
    private List<TileEffect> buildingTileEffects = new();
    private List<TileEffect> waterTileEffects = new();
    private List<TileEffect> lavaTileEffects = new();


    private GameManager gmInstance => GameManager.Instance;
    private CubeGridManager cgmInstance => CubeGridManager.Instance;

    private void Awake()
    {
        PopulateTileEffects();
    }

    public void Initialize(TileModificationData[] tileModificationData)
    {
        foreach (var data in tileModificationData)
        {
            switch (data.tileEffectType)
            {
                case TileEffectType.OffenseTile:
                    if (!data.inCorners)
                        GenerateTileEffects(offenseTileEffects, data);
                    else
                        GenerateInCorner(offenseTileEffects, data);
                    break;

                case TileEffectType.DefenseTile:
                    if (!data.inCorners)
                        GenerateTileEffects(defenseTileEffects, data);
                    else
                        GenerateInCorner(defenseTileEffects, data);
                    break;

                case TileEffectType.ResourceTile:
                    if (!data.inCorners)
                        GenerateTileEffects(resourceTileEffects, data);
                    else
                        GenerateInCorner(resourceTileEffects, data);
                    break;

                case TileEffectType.WaterTile:
                    GenerateTileEffects(waterTileEffects, data);
                    break;

                case TileEffectType.LavaTile:
                    GenerateTileEffects(lavaTileEffects, data);
                    break;

                case TileEffectType.BuildingTile:
                    if (!data.inCorners)
                        GenerateTileEffects(buildingTileEffects, data);
                    else
                        GenerateInCorner(buildingTileEffects, data);
                    break;
            }
        }
    }

    private void GenerateTileEffects(List<TileEffect> tileEffectList, TileModificationData data)
    {
        TileEffect highPriorityEffect = tileEffectList[0];

        for (int i = 0; i < tileEffectList.Count; i++)
        {
            if (tileEffectList[i].visualPriority > highPriorityEffect.visualPriority)
                highPriorityEffect = tileEffectList[i];
        }

        if (data.isGrouped)
        {
            List<Tile> randomTiles = new();

            randomTiles = cgmInstance.GetGroupedTiles(data.tileCount, data.minDistanceFromMainBuilding);

            for (int j = 0; j < randomTiles.Count; j++)
            {
                highPriorityEffect.ApplyVisuals(randomTiles[j]);
                randomTiles[j].tileEffectType = data.tileEffectType;
                for (int k = 0; k < tileEffectList.Count; k++)
                {
                    randomTiles[j].tileEffects.Add(tileEffectList[k]);
                }

                if (randomTiles[j] != null && data.isObstacle)
                {
                    var navMeshObstacle = randomTiles[j].AddComponent<NavMeshObstacle>();
                    navMeshObstacle.enabled = true;
                    navMeshObstacle.carving = true;
                    navMeshObstacle.center = Vector3.up * 2f;
                }
            }
        }
        else
        {
            for (int j = 0; j < data.tileCount; j++)
            {
                Tile randomTile;

                if (j % 2 == 0)
                    randomTile = cgmInstance.GetRandomTile(Side.Player, data.minDistanceFromMainBuilding);
                else
                    randomTile = cgmInstance.GetRandomTile(Side.Enemy, data.minDistanceFromMainBuilding);

                highPriorityEffect.ApplyVisuals(randomTile);
                randomTile.tileEffectType = data.tileEffectType;
                for (int k = 0; k < tileEffectList.Count; k++)
                {
                    randomTile.tileEffects.Add(tileEffectList[k]);
                }

                if (randomTile != null && data.isObstacle)
                {
                    var navMeshObstacle = randomTile.AddComponent<NavMeshObstacle>();
                    navMeshObstacle.enabled = true;
                    navMeshObstacle.carving = true;
                    navMeshObstacle.center = Vector3.up * 2f;
                }
            }
        }
    }

    // temporary purpose -> Later Implement area based tile selection for better work flow
    private void GenerateInCorner(List<TileEffect> tileEffectList, TileModificationData data)
    {
        TileEffect highPriorityEffect = tileEffectList[0];
        for (int i = 0; i < tileEffectList.Count; i++)
        {
            if (tileEffectList[i].visualPriority > highPriorityEffect.visualPriority)
                highPriorityEffect = tileEffectList[i];
        }

        for (int j = 0; j < data.tileCount; j++)
        {
            Tile randomTile;

            if (j % 2 == 0)
                randomTile = cgmInstance.GetRandomCornerTile(Side.Player, 2, 5);
            else
                randomTile = cgmInstance.GetRandomCornerTile(Side.Enemy, 2, 5);

            highPriorityEffect.ApplyVisuals(randomTile);
            randomTile.tileEffectType = data.tileEffectType;
            for (int k = 0; k < tileEffectList.Count; k++)
            {
                randomTile.tileEffects.Add(tileEffectList[k]);
            }

            if (randomTile != null && data.isObstacle)
            {
                var navMeshObstacle = randomTile.AddComponent<NavMeshObstacle>();
                navMeshObstacle.enabled = true;
                navMeshObstacle.carving = true;
                navMeshObstacle.center = Vector3.up * 2f;
            }
        }

    }
    private void PopulateTileEffects()
    {
        foreach (var effect in allTileEffects)
        {
            switch (effect)
            {
                case BasicStatsEffect:
                    // offenseTileEffects.Add(effect);
                    // defenseTileEffects.Add(effect);
                    resourceTileEffects.Add(effect);
                    break;
                case DamageEffect:
                case RangeEffect:
                    buildingTileEffects.Add(effect);
                    defenseTileEffects.Add(effect);
                    break;
                case UnitProductionEffect:
                    buildingTileEffects.Add(effect);
                    offenseTileEffects.Add(effect);
                    break;
                case ResourceProductionEffect:
                    buildingTileEffects.Add(effect);
                    resourceTileEffects.Add(effect);
                    break;
                case WaterEffect:
                    waterTileEffects.Add(effect);
                    break;
                case LavaEffect:
                    lavaTileEffects.Add(effect);
                    break;
            }
        }
    }
}

[Serializable]
public class TileModificationData
{
    public TileEffectType tileEffectType;
    public int tileCount;
    [ReadOnly] public int clusterSize;
    public int minDistanceFromMainBuilding;
    public bool isGrouped;
    public bool isObstacle;
    public bool inCorners;
}

public enum TileEffectType { NONE, OffenseTile, DefenseTile, ResourceTile, WaterTile, LavaTile, BuildingTile }
