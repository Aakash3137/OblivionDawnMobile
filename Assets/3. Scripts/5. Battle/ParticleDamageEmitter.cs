using System.Collections.Generic;
using UnityEngine;

public class ParticleDamageEmitter : MonoBehaviour
{
    [Header("Particle")]
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private Transform muzzlePoint;

    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 100f;
    [SerializeField] private float damageMultiplier = 1f;

    [Header("Read Only")]
    private ParticleSystem particleInstance;
    [SerializeField]private float damagePerParticle;
    private Stats Stats;
    private Side unitSide;

    private void Start()
    {
        Stats = GetComponent<Stats>();
        if (Stats != null)
            unitSide = Stats.side;

        
        if (!particlePrefab || !muzzlePoint)
        {
            Debug.LogError("ParticleDamageEmitter missing prefab or muzzle point", this);
            return;
        }

        particleInstance = Instantiate(particlePrefab, muzzlePoint);
        particleInstance.transform.localPosition = Vector3.zero;
        particleInstance.transform.localRotation = Quaternion.identity;

        SetupCollision();
        CalculateDamagePerParticle();

        particleInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    
    public void SetFireAngle(float xAngle)
    {
        if (particleInstance == null)
            return;

        var rot = particleInstance.transform.localEulerAngles;
        rot.x = xAngle;
        particleInstance.transform.localEulerAngles = rot;
    }
    
    
    public void StartFiring()
    {
        if (particleInstance == null)
            return;

        if (!particleInstance.isPlaying)
            particleInstance.Play();
    }

    public void StopFiring()
    {
        if (particleInstance == null)
            return;

        if (particleInstance.isPlaying)
            particleInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void SetupCollision()
    {
        var collision = particleInstance.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.sendCollisionMessages = true;

        collision.bounce = 0f;
        collision.dampen = 0f;
    }

    private void CalculateDamagePerParticle()
    {
        var main = particleInstance.main;
        damagePerParticle = (damagePerSecond / Mathf.Max(1, main.maxParticles)) * damageMultiplier;
    }

    public void ApplyParticleDamage(
        ParticleSystem system,
        GameObject other,
        List<ParticleCollisionEvent> collisionEvents)
    {
        if (other == gameObject)
            return;
          

        if (!other.TryGetComponent<Stats>(out var stats))
            return;

        GameDebug.Log($"Particle hit { stats.name} - {stats.side} vs myside- {unitSide}");
        if (stats.side == unitSide)
            return;

        int count = system.GetCollisionEvents(other, collisionEvents);
        if (count <= 0) return;

        float damage = damagePerParticle * count;
        stats.TakeDamage(damage);
    }
}
