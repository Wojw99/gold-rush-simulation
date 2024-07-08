using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private ExplosionType type = ExplosionType.Health;
    [SerializeField] private float damageAmount = 10;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentStats stats))
        {
            switch (type)
            {
                case ExplosionType.Health:
                    stats.Health -= damageAmount;
                    break;
                case ExplosionType.Stamina:
                    stats.Stamina -= damageAmount;
                    break;
            }
        }
    }
}

public enum ExplosionType {
    Stamina,
    Health,
}
