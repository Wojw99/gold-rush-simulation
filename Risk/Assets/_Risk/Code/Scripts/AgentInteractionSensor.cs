using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractionSensor : MonoBehaviour
{
    private AgentStatus agentStatus;
    private AgentBrain agentBrain;
    public List<InteractionObject> interactibles = new List<InteractionObject>();

    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        agentBrain = GetComponent<AgentBrain>();
    }

    public void OnEnter(InteractionType interactionType, GameObject gameObject) {
        var interactible = new InteractionObject(interactionType, gameObject);
        interactibles.Add(interactible);
    }

    public void OnExit(GameObject gameObject) {
        var interactible = interactibles.Find(i => i.gameObject == gameObject);
        interactibles.Remove(interactible);
    }
}

public class InteractionObject {
    public InteractionType interactionType;
    public GameObject gameObject;

    public InteractionObject(InteractionType interactionType, GameObject gameObject) {
        this.interactionType = interactionType;
        this.gameObject = gameObject;
    }
}

public enum InteractionType {
    DEPOSIT,
    REST,
    HEAL,
    DAMAGE,
}
