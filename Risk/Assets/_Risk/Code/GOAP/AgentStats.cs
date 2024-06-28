using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float maxStamina = 100;
    [SerializeField] float maxOre = 100;
    float health = 100;
    float stamina = 100;
    float ore = 100;

    bool isFillingHealth = false;
    bool isFillingStamina = false;

    CountdownTimer statsTimer;

    public event Action StatsChanged;

    void Awake() {
        health = maxHealth;
        stamina = maxStamina;
        ore = maxOre;
    }

    void Start() {
        SetupTimer();
    }

    void Update() {
        statsTimer.Tick(Time.deltaTime);
    }

    void SetupTimer() {
        statsTimer = new CountdownTimer(0.5f);
        statsTimer.OnTimerStop += () => {
            UpdateStats();
            statsTimer.Start();
        };
        statsTimer.Start();
    }


    void UpdateStats() {
        health += isFillingHealth ? 20 : -10;
        stamina += isFillingStamina ? 20 : -5;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        health = Mathf.Clamp(health, 0, maxHealth);
        StatsChanged?.Invoke();
    }

    public void StartFillingStamina() {
        isFillingStamina = true;
    }

    public void StopFillingStamina() {
        isFillingStamina = false;
    }

    public void StartFillingHealth() {
        isFillingHealth = true;
    }

    public void StopFillingHealth() {
        isFillingHealth = false;
    }

    public float Health => health;
    public float Stamina => stamina;
    public float Ore => ore;
    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float MaxOre => maxOre;

    void OnDestroy() {
        StatsChanged = null;
    }
}
