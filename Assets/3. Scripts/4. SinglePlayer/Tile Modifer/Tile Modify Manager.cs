using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TileModifyManager : MonoBehaviour
{
    [SerializeField] private TileEffect[] allTileEffects;

    [Space(5)]
    [SerializeField] private TileModificationData[] tileModificationData;

    private List<TileEffect> offenseTileEffects = new List<TileEffect>();
    private List<TileEffect> defenseTileEffects = new List<TileEffect>();
    private List<TileEffect> resourceTileEffects = new List<TileEffect>();
    private List<TileEffect> waterTileEffects = new List<TileEffect>();
    private List<TileEffect> lavaTileEffects = new List<TileEffect>();


    private GameManager gmInstance => GameManager.Instance;
    private CubeGridManager cgmInstance => CubeGridManager.Instance;

    private void Awake()
    {
        PopulateTileEffects();
    }
    private void Start()
    {
        GenerateTileModificationVisuals();
    }
    private void GenerateTileModificationVisuals()
    {
        foreach (var data in tileModificationData)
        {
            switch (data.tileEffectType)
            {
                case TileEffectType.OffenseTile:
                    GenerateTileEffects(offenseTileEffects, data);
                    break;

                case TileEffectType.DefenseTile:
                    GenerateTileEffects(defenseTileEffects, data);
                    break;

                case TileEffectType.ResourceTile:
                    GenerateTileEffects(resourceTileEffects, data);
                    break;

                case TileEffectType.WaterTile:
                    GenerateTileEffects(waterTileEffects, data);
                    break;

                case TileEffectType.LavaTile:
                    GenerateTileEffects(lavaTileEffects, data);
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
                ApplyTileEffects(highPriorityEffect, randomTiles[j]);

                if (data.isObstacle)
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
                Tile randomTile = null;
                if (j % 2 == 0)
                    randomTile = cgmInstance.GetRandomTile(Side.Player, data.minDistanceFromMainBuilding);
                else
                    randomTile = cgmInstance.GetRandomTile(Side.Enemy, data.minDistanceFromMainBuilding);

                ApplyTileEffects(highPriorityEffect, randomTile);

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

    private void ApplyTileEffects(TileEffect effect, Tile tile)
    {
        if (effect is BasicStatsEffect basicStatsEffect)
        {
            basicStatsEffect.ApplyVisuals(tile);
        }
        else if (effect is DamageEffect damageEffect)
        {
            damageEffect.ApplyVisuals(tile);
        }
        else if (effect is RangeEffect rangeEffect)
        {
            rangeEffect.ApplyVisuals(tile);
        }
        else if (effect is UnitProductionEffect unitProductionEffect)
        {
            unitProductionEffect.ApplyVisuals(tile);
        }
        else if (effect is ResourceProductionEffect resourceProductionEffect)
        {
            resourceProductionEffect.ApplyVisuals(tile);
        }
        else if (effect is WaterEffect waterEffect)
        {
            waterEffect.ApplyVisuals(tile);
        }
        else if (effect is LavaEffect lavaEffect)
        {
            lavaEffect.ApplyVisuals(tile);
        }
    }

    private void PopulateTileEffects()
    {
        foreach (var effect in allTileEffects)
        {
            switch (effect)
            {
                case BasicStatsEffect:
                    offenseTileEffects.Add(effect);
                    defenseTileEffects.Add(effect);
                    resourceTileEffects.Add(effect);
                    break;
                case DamageEffect:
                case RangeEffect:
                    offenseTileEffects.Add(effect);
                    defenseTileEffects.Add(effect);
                    break;
                case UnitProductionEffect:
                    offenseTileEffects.Add(effect);
                    break;
                case ResourceProductionEffect:
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

    private void OnValidate()
    {
        for (int i = 0; i < tileModificationData.Length; i++)
        {
            tileModificationData[i].clusterSize = (int)(tileModificationData[i].tileCount * 0.5f);
        }
    }
}

[Serializable]
public class TileModificationData
{
    public TileEffectType tileEffectType;
    public int tileCount;
    [ReadOnly]public int clusterSize;
    public int minDistanceFromMainBuilding;
    public bool isGrouped;
    public bool isObstacle;
}

public enum TileEffectType { OffenseTile, DefenseTile, ResourceTile, WaterTile, LavaTile }
