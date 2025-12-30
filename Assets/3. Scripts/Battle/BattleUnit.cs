using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnit : MonoBehaviour
{
    [Header("References")]
    public BattleUnitEnum battleUnitEnum;

    [SerializeField] internal FadeHealthBar healthBarFade;

    [Header("Stats")]
    //public float currentHealth;
    public float DetectionRange = 10f;
    public float AttackRange = 3f;
    
    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackCooldown = .5f;
    private float attackTimer;

    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 2f;

    public UnitStats unitStats;
    
    public NavMeshAgent agent;
    public Animator animator;
    public GameObject target;
    
    private float targetCheckTimer;
    private const float targetCheckInterval = 0.5f;

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        
        unitStats.currentHealth = unitStats.maxHealth;
        agent.speed = moveSpeed;
        agent.stoppingDistance = AttackRange;
        agent.updateRotation = true;
        
        ArrangeRotation();
        ApplySeparation();
    }
    
    private void Update()
    {
        // Validate target FIRST
        if (target != null && (!target || target.GetComponent<BattleUnit>() == null))
        {
            target = null;
            agent.isStopped = true;
        }
        
        if (target == null)
        {
            targetCheckTimer += Time.deltaTime;
            
            if (targetCheckTimer >= targetCheckInterval)
            {
                targetCheckTimer = 0f;
                FindTarget();
            }

            agent.isStopped = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (!agent.hasPath && distance > AttackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
        
        if (distance > AttackRange)
        {
            agent.isStopped = false;

            if (!agent.hasPath || agent.remainingDistance > AttackRange)
            {
                agent.SetDestination(target.transform.position);
            }

            if (animator != null)
                animator.SetFloat("Move", agent.velocity.magnitude);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            if (animator != null)
                animator.SetFloat("Move", agent.velocity.magnitude);
            
            ApplySeparation();
            FaceTarget();
            Attack();
        }
        
        if (target != null)
        {
            if (!target || target.GetComponent<BattleUnit>() == null)
            {
                target = null;
                agent.isStopped = true;
                return;
            }
        }

    }
    
    private void ArrangeRotation()
    {
        if (GetComponent<SideScenario>().side == Side.Player)
        {
            transform.rotation = Quaternion.Euler(0, 45, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -135, 0);
        }
    }
    
    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectionRange);

        foreach (Collider hit in hits)
        {
            BattleUnit unit = hit.GetComponent<BattleUnit>();
            if (unit != null && unit != this)
            {
                // Check side
                if (unit.GetComponent<SideScenario>().side !=
                    GetComponent<SideScenario>().side)
                {
                    target = unit.gameObject;
                    attackTimer = 0f;
                    agent.ResetPath();
                    agent.isStopped = false;
                    return;
                }
            }
        }
    }

    void FaceTarget()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
        }
    }
    
    void ApplySeparation()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius);

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider hit in hits)
        {
            BattleUnit unit = hit.GetComponent<BattleUnit>();

            if (unit != null && unit != this)
            {
                // only separate from same side
                if (unit.GetComponent<SideScenario>().side ==
                    GetComponent<SideScenario>().side)
                {
                    Vector3 dir = transform.position - unit.transform.position;
                    float dist = dir.magnitude;

                    if (dist > 0.01f)
                    {
                        separation += dir.normalized / dist;
                        count++;
                    }
                }
            }
        }

        if (count > 0)
        {
            Vector3 move = separation.normalized * separationStrength;
            agent.Move(move * Time.deltaTime);
            if (animator != null)
                animator.SetFloat("Move", agent.velocity.magnitude);
        }
    }

    
    void Attack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (target != null)
            {
                BattleUnit enemy = target.GetComponent<BattleUnit>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    
                    if (animator != null)
                    {
                        animator.SetBool("Fire", true);
                        StartCoroutine(ResetFire());
                    }
                }
            }
        }
    }

    IEnumerator ResetFire()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("Fire", false);
    }
    
    // here units get damage from enemies
    public void TakeDamage(float damage)
    {
        unitStats.currentHealth -= damage;
        unitStats.currentHealth = Mathf.Clamp(unitStats.currentHealth, 0, unitStats.maxHealth);
        if (healthBarFade != null)
        {
            healthBarFade.ShowOnHit();
            healthBarFade.Isvisible = true;
        }
        if (unitStats.currentHealth <= 0)
        {
            Die();
        }
        
    }
    
    //Destroy enemy
    public void Die()
    {
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        BattleUnit[] units = FindObjectsOfType<BattleUnit>();
        foreach (var unit in units)
        {
            if (unit.target == gameObject)
            {
                unit.target = null;
            }
        }
    }

    #region TargetDetection
    
    public static bool AnyPlayerHasTarget()
    {
        BattleUnit[] units = FindObjectsOfType<BattleUnit>();
        foreach (var unit in units)
        {
            if (unit.GetComponent<SideScenario>().side == Side.Player &&
                unit.target != null)
            {
                return true;
            }
        }
        return false;
    }

    public static bool AnyEnemyHasTarget()
    {
        BattleUnit[] units = FindObjectsOfType<BattleUnit>();
        foreach (var unit in units)
        {
            if (unit.GetComponent<SideScenario>().side == Side.Enemy &&
                unit.target != null)
            {
                return true;
            }
        }
        return false;
    }


    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
    #endregion
    
}
