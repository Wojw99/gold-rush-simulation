using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractionSensor : MonoBehaviour
{
    public List<InteractionInfo> interactibles = new List<InteractionInfo>();

    public event Action<InteractionType> InteractionStarted;
    public event Action<InteractionType> InteractionEnded;
    public event Action<InteractionType> InteractionExited;

    public event Action<GameObject> AgentApproached;
    public event Action<GameObject> AgentLeft;

    public event Action PlayerSelect;
    public event Action PlayerDeselect;
    public event Action<GameObject> PlayerOrder;

    public event Action<ModifierInfo> ModifierStarted;

    public void OnModifierEnter(ModifierInfo modifierInfo) {
        ModifierStarted?.Invoke(modifierInfo);
    }

    public void OnPlayerSelect() {
        PlayerSelect?.Invoke();
    }

    public void OnPlayerDeselect() {
        PlayerDeselect?.Invoke();
    }

    public void OnPlayerOrder(GameObject destination) {
        PlayerOrder?.Invoke(destination);
    }

    public void OnInteractionEnter(InteractionType interactionType, GameObject gameObject) {
        InteractionStarted?.Invoke(interactionType);
        AddToInteractibles(interactionType, gameObject);
    }

    public void OnInteractionEnd(InteractionType interactionType, GameObject gameObject) {
        InteractionEnded?.Invoke(interactionType);
        RemoveFromInteractibles(gameObject);
    }

    public void OnInteractionExit(InteractionType interactionType, GameObject gameObject) {
        InteractionExited?.Invoke(interactionType);
        RemoveFromInteractibles(gameObject);
    }

    private void RemoveFromInteractibles(GameObject gameObject) {
        var interactible = interactibles.Find(i => i.gameObject == gameObject);
        interactibles.Remove(interactible);
    }

    private void AddToInteractibles(InteractionType interactionType, GameObject gameObject) {
        var interactible = new InteractionInfo(interactionType, gameObject);
        interactibles.Add(interactible);
    }
    
    private void OnAgentApproached(GameObject gameObject) {
        AgentApproached?.Invoke(gameObject);
    }

    private void OnAgentLeft(GameObject gameObject) {
        AgentLeft?.Invoke(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnAgentApproached(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor))
        {
            agentInteractionSensor.OnAgentLeft(gameObject);
        }
    }

    private void OnDestroy() {
        InteractionStarted = null;
        InteractionEnded = null;
        InteractionExited = null;
        AgentApproached = null;
        AgentLeft = null;
        PlayerSelect = null;
        PlayerDeselect = null;
        PlayerOrder = null;
        ModifierStarted = null;
    }
}
