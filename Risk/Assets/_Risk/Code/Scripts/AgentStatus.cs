using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus : MonoBehaviour
{
    public float maxHealth = 10;
    public float health;
    public float maxStamina = 20;
    public float stamina;
    public float maxOre = 10;
    public float ore;

    private void Start() {
        health = maxHealth;
        stamina = maxStamina;
        ore = 0;
    }
}