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
    int id = CalculateId();

    bool isFillingHealth = false;
    bool isFillingStamina = false;
    bool isDrawingStamina = false;
    bool isDrawingHealth = false;

    CountdownTimer statsTimer;

    public event Action StatsChanged;

    void Awake() {
        health = 10; // maxHealth;
        stamina = maxStamina;
        ore = 0;
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
        if(isFillingHealth) {
            health += 20;
        } else if (isDrawingHealth) {
            health -= 10;
        }

        if(isFillingStamina) {
            stamina += 20;
        } else if (isDrawingStamina) {
            stamina -= 5;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        health = Mathf.Clamp(health, 0, maxHealth);
        StatsChanged?.Invoke();
    }

    public void StartDrawingStamina() {
        isDrawingStamina = true;
    }

    public void StopDrawingStamina() {
        isDrawingStamina = false;
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

    public float Health {
        get => health;
        set {
            health = value;
            health = Mathf.Clamp(health, 0, maxHealth);
            StatsChanged?.Invoke();
        }
    }

    public float Stamina {
        get => stamina;
        set {
            stamina = value;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            StatsChanged?.Invoke();
        }
    }

    public float Ore {
        get => ore;
        set {
            ore = value;
            ore = Mathf.Clamp(ore, 0, maxOre);
            StatsChanged?.Invoke();
        }
    }

    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float MaxOre => maxOre;
    public int ID => id;

    void OnDestroy() {
        StatsChanged = null;
    }

    private static int _maxId = -1;
    public static int CalculateId() {
        _maxId++;
        return _maxId;
    }
}
