using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum AirUnitState { Hover, Fly, Combat, Destroyed }
[RequireComponent(typeof(NavMeshAgent))]
public class AirUnitAI : MonoBehaviour
{
    private UnitStats stats;
    private AirUnitState currentState = AirUnitState.Fly;
    [Header("Animation")]
    [SerializeField] private Animator animator;
}
