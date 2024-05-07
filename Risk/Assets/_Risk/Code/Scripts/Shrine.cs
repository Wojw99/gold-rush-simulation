using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    private InteractionType interactionType = InteractionType.HEAL;

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
