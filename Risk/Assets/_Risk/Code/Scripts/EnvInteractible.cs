using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvInteractible : MonoBehaviour
{
    public InteractionType interactionType;
    public bool isOccupied = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOccupied && other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnEnter(interactionType, gameObject);
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (isOccupied && other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnExit(gameObject);
            isOccupied = false;
        }
    }
}
