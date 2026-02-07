using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDamageReceiver : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleDamageEmitter emitter;
    private List<ParticleCollisionEvent> collisionEvents = new();

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        emitter = GetComponentInParent<ParticleDamageEmitter>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (emitter == null) return;
        emitter.ApplyParticleDamage(ps, other, collisionEvents);
    }
}