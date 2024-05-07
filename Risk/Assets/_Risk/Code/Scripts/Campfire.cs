using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public InteractionType interactionType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnInteractionEnter(interactionType, gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnInteractionExit(interactionType, gameObject);
        }
    }
}
