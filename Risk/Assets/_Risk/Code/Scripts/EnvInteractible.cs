using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvInteractible : MonoBehaviour
{
    public InteractionType interactionType;
    public bool isOccupied = false;
    public float interactionTime = 3f;
    private AgentInteractionSensor agentInteractionSensor = null;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOccupied && other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnInteractionEnter(interactionType, gameObject);
            this.agentInteractionSensor = agentInteractionSensor;
            isOccupied = true;
            StartCoroutine(Interact());
        }
    }

    private IEnumerator Interact()
    {
        yield return new WaitForSeconds(interactionTime);
        if (isOccupied)
        {
            isOccupied = false;
            agentInteractionSensor.OnInteractionEnd(interactionType, gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (isOccupied && other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnInteractionExit(interactionType, gameObject);
            this.agentInteractionSensor = null;
            isOccupied = false;
        }
    }
}
