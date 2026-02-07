using System.Collections;
using UnityEngine;

public class DragonUnit : AirUnit
{
    [Header("Dragon Attack")] [SerializeField]
    private ParticleDamageEmitter particleDamageEmitter;

    private float attackDuration = 1f;
    private float attackTimer = 0f;
    private bool isFiring = false;

    //dragon unit has no projectile it releases fire from mouth
    protected override void Start()
    {
        base.Start();

        // Disable projectile weapon
        if (projectileShooter != null)
            projectileShooter.enabled = false;

        if (particleDamageEmitter == null)
            particleDamageEmitter = GetComponent<ParticleDamageEmitter>();
    }

    //attack by firing particle system
    protected override void Attack()
    {
        if (airState != AirState.Airborne && airState != AirState.Attacking)
            return;

        if (target == null)
        {
            StopAttack();
            return;
        }

        if (!isFiring)
        {
            isFiring = true;
            attackTimer = 0f;
            
            //setting fire angle bfore start firing
            bool targetIsAir = target.GetComponent<AirUnit>() != null;
            float xAngle = targetIsAir ? 0f : 15f;
            particleDamageEmitter?.SetFireAngle(xAngle);
            
            particleDamageEmitter?.StartFiring();

            if (animator != null)
                animator.SetBool("Fire", true);

            airState = AirState.Attacking;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            StopAttack();
            airState = AirState.Evading;
            evadeCenter = transform.position;
            evadeAngle = 0f;
        }
    }

    private void StopAttack()
    {
        if (isFiring)
        {
            isFiring = false;
            particleDamageEmitter?.StopFiring();

            if (animator != null)
                animator.SetBool("Fire", false);
        }
    }

    protected override void PerformEvade()
    {
        StopAttack();
        base.PerformEvade();
    }

    protected override void IdleCircle()
    {
        StopAttack();
        base.IdleCircle();
    }

}
