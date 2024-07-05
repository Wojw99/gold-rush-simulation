using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentType {
    SETTLER,
    UNDEAD,
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

