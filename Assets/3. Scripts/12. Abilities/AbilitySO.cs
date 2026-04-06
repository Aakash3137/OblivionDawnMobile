using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
public class AbilitySO : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName;
    public AbilityType abilityType;
    public Sprite abilityIcon;
    
    [Header("Scope")]
    public AbilityScope abilityScope = AbilityScope.Personal;  // Personal by default
    public Side casterSide = Side.Player;  // Which side is using this ability

    [Header("Targeting")]
    public AbilityTargetType targetType;
    public GameUnitName targetUnitName;
    public ScenarioUnitType targetUnitType; 

    [Header("Timing")] 
    public float duration;
    public float cooldown;

    [Header("Effects")]
    public List<AbilityEffect> effects;
}