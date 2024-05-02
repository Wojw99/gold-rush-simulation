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

    public GameObject nearestSpottedDeposit;
    public GameObject collidedDeposit;
    public bool depositExtracted = false;

    public GameObject nearestSpottedRest;
    public GameObject collidedRest;

    public GameObject nearestSpottedUndead;

    private void Start() {
        health = maxHealth;
        stamina = maxStamina;
        ore = 0;
    }
}
