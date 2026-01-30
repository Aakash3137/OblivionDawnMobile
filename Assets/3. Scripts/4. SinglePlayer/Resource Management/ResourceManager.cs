
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int currentFood { get; protected set; }
    public int currentGold { get; protected set; }
    public int currentMetal { get; protected set; }
    public int CurrentPower { get; protected set; }

    [field: Header("EDITOR VIEW ONLY")]
    [field: SerializeField, ReadOnly]
    public float currentFoodGenerationRate { get; protected set; }
    [field: SerializeField, ReadOnly]
    public float currentGoldGenerationRate { get; protected set; }
    [field: SerializeField, ReadOnly]
    public float currentMetalGenerationRate { get; protected set; }
    [field: SerializeField, ReadOnly]
    public float currentPowerGenerationRate { get; protected set; }

    public Action OnResourcesChanged;


}
