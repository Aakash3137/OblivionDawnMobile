
using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int currentFood { get; protected set; }
    public int currentGold { get; protected set; }
    public int currentMetal { get; protected set; }
    public int CurrentPower { get; protected set; }

    [field: Header("EDITOR VIEW ONLY")]
    [field: SerializeField]
    public float currentFoodGenerationRate { get; protected set; }
    [field: SerializeField]
    public float currentGoldGenerationRate { get; protected set; }
    [field: SerializeField]
    public float currentMetalGenerationRate { get; protected set; }
    [field: SerializeField]
    public float currentPowerGenerationRate { get; protected set; }
    [HideInInspector]
    
    public Action OnResourcesChanged;
}
