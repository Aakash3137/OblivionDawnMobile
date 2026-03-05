using UnityEngine;

[RequireComponent(typeof(ParticleDamageEmitter))]
public class DragonUnit : AirUnit
{
    [Header("Dragon Fire Settings")]
    [SerializeField] private ParticleDamageEmitter particleDamageEmitter;
    [SerializeField] private float fireDuration = 1.5f;

    private float fireTimer = 0f;
    private bool isFiring = false;

    protected override void Start()
    {
        base.Start();

        // Disable projectile shooter completely
        if (projectileShooter != null)
            projectileShooter.enabled = false;

        if (particleDamageEmitter == null)
            particleDamageEmitter = GetComponent<ParticleDamageEmitter>();
    }

    protected override void Update()
    {
        base.Update();

        // If currently firing, maintain fire logic
        if (isFiring)
        {
            MaintainFire();
        }
    }

    protected override void Attack()
    {
        if (airState != AirState.Airborne && airState != AirState.Attacking)
            return;

        if (target == null)
            return;

        if (!IsFacingTarget(target))
            return;

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist > AttackRange)
            return;

        // Start firing
        if (!isFiring)
        {
            isFiring = true;
            fireTimer = 0f;

            bool targetIsAir = target.GetComponent<AirUnit>() != null;
            float xAngle = targetIsAir ? 0f : 15f;

            particleDamageEmitter.SetFireAngle(xAngle);
            particleDamageEmitter.StartFiring();

            if (animator != null)
                animator.SetBool("Fire", true);

            airState = AirState.Attacking;
        }
    }

    private void MaintainFire()
    {
        if (target == null)
        {
            StopFire();
            airState = AirState.Airborne;
            return;
        }

        fireTimer += Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.transform.position);

        // Stop if target leaves range or facing lost
        if (dist > AttackRange || !IsFacingTarget(target))
        {
            StopFire();
            airState = AirState.Airborne;
            return;
        }

        // Fire duration complete → Evade
        if (fireTimer >= fireDuration)
        {
            StopFire();

            airState = AirState.Evading;
            evadeCenter = transform.position;
            evadeAngle = 0f;
        }
    }

    private void StopFire()
    {
        if (!isFiring)
            return;

        isFiring = false;
        fireTimer = 0f;

        particleDamageEmitter.StopFiring();

        if (animator != null)
            animator.SetBool("Fire", false);
    }

    protected override void PerformEvade()
    {
        StopFire();
        base.PerformEvade();
    }

    protected override void IdleCircle()
    {
        StopFire();
        base.IdleCircle();
    }

    // Prevent forward movement while breathing fire
    protected override void FlyTowards(Vector3 targetPosition)
    {
        if (isFiring)
        {
            // Move straight forward without rotating
            Vector3 pos = transform.position + transform.forward * (moveSpeed * Time.deltaTime);
            pos.y = transform.position.y; // maintain fly height
            transform.position = pos;
            return;
        }

        base.FlyTowards(targetPosition);
    }
    
}