using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private float spawnRadius = 1f;
    [SerializeField] private float destroyTimeOffset = 0f;

    private void Spawn() {
        var spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            Spawn();
            Destroy(gameObject, destroyTimeOffset);
        }
    }
}
