using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    [field: SerializeField]
    public AttackStats unitAttackStats { get; private set; }
    [field: SerializeField]
    public RangeStats unitRangeStats { get; private set; }

    public void Initialize(AttackStats attackStats, RangeStats rangeStats)
    {
        unitAttackStats = attackStats;
        unitRangeStats = rangeStats;
    }
    public void Attack()
    {

    }
}
