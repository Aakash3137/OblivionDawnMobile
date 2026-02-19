using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyPersonality", menuName = "AI/Enemy Personality")]
public class EnemyPersonality : ScriptableObject
{
    public string personalityName;

    // BUILDING STRATEGY

    [Header("=== Building Category Weights ===")]
    [Range(0f, 1f)] public float unitBuildingWeight = 0.5f;
    [Range(0f, 1f)] public float resourceBuildingWeight = 0.3f;
    [Range(0f, 1f)] public float defenseBuildingWeight = 0.2f;

    [Space(15)]
    [Header("=== Unit Type Weights (Air, Melee, Ranged, AOE) ===")]
    public float[] unitTypeWeights = new float[4] { 0.25f, 0.25f, 0.25f, 0.25f };

    [Space(10)]
    [Header("=== Resource Type Weights (Food, Gold, Metal, Power) ===")]
    public float[] resourceTypeWeights = new float[4] { 0.25f, 0.25f, 0.25f, 0.25f };

    [Space(10)]
    [Header("=== Defense Type Weights (AntiAir, AntiTank, Turret) ===")]
    public float[] defenseTypeWeights = new float[3] { 0.33f, 0.33f, 0.34f };

    // TIMING

    [Space(20)]
    [Header("=== Thinking Time ===")]
    public float spawnInterval = 5f;

    // STRATEGIC ERRORS

    [Space(20)]
    [Header("=== Strategic Behavior ===")]
    [Range(0f, 1f)] public float mistakeProbability = 0.1f;

    // MAP KNOWLEDGE

    [Space(20)]
    [Header("=== Map Knowledge ===")]

    [Header("Discipline in using Mapping strategy")]
    [Range(0,1)] public float tacticalDiscipline = 0.8f;

    [Header("Precision in using Mapping strategy")]
    [Range(0,1)] public float tacticalPrecision = 0.8f;

    // LIMITS

    [Space(20)] [Header("=== Max Building Capacity ===")]
    public int maxEnemyBuildings = 150;

    [Space(20)]
    [Header("=== Unit Levels (Not Implemented Yet) ===")]
    public AnimationCurve unitLevelCurve;
}
