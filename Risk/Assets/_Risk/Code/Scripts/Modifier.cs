using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    [SerializeField] private ModifierType modifierType = ModifierType.DAMAGE;
    [SerializeField] private float modifierValue = 1f;
    [SerializeField] private float modifierLifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, modifierLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            var modifier = new ModifierInfo(modifierType, modifierValue, modifierLifetime);
            agentInteractionSensor.OnModifierEnter(modifier);
        }
    }
}