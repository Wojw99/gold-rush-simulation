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

    public event Action<ModifierInfo> ModifierStarted;

    public void OnModifierEnter(ModifierInfo modifierInfo) {
        ModifierStarted?.Invoke(modifierInfo);
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
}

public class InteractionInfo {
    public InteractionType interactionType;
    public GameObject gameObject;

    public InteractionInfo(InteractionType interactionType, GameObject gameObject) {
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

public class ModifierInfo {
    public ModifierType modifierType;
    public float modifierValue;
    public float modifierLifetime;

    public ModifierInfo(ModifierType modifierType, float modifierValue, float modifierLifetime) {
        this.modifierType = modifierType;
        this.modifierValue = modifierValue;
        this.modifierLifetime = modifierLifetime;
    }
}

public enum ModifierType
{
    HEAL,
    DAMAGE,
    STAMINA
}
