using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float maxStamina = 100;
    [SerializeField] float maxOre = 100;
    [SerializeField] float maxRisk = 100;
    float health = 100;
    float stamina = 100;
    float ore = 100;
    float risk = 100;
    int id = CalculateId();
    string agentName = "James";
    [SerializeField] int attack = 10;
    [SerializeField] float attackSpeed = 1;
    [SerializeField] Team team;
    int attackModifierMin = 1;
    int attackModifierMax = 10;
    float attackSpeedModifierMin = 0;
    float attackSpeedModifierMax = 0.5f;

    bool isFillingHealth = false;
    bool isFillingStamina = false;
    bool isDrawingStamina = false;
    bool isDrawingHealth = false;
    bool isFillingRisk = true;

    CountdownTimer statsTimer;

    public event Action StatsChanged;

    void Awake() {
        health = maxHealth; 
        stamina = maxStamina;
        ore = 0;
        risk = maxRisk;
        agentName = RandomGenerator.Instance.GenerateName();
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

        if(isFillingRisk) {
            risk += 5;
        } else {
            risk -= 5;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        health = Mathf.Clamp(health, 0, maxHealth);
        risk = Mathf.Clamp(risk, 0, maxRisk);
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

    public void StartDrawingRisk() {
        isFillingRisk = false;
    }

    public void StopDrawingRisk() {
        isFillingRisk = true;
    }

    public float Health {
        get => health;
        set {
            health = value;
            health = Mathf.Clamp(health, 0, maxHealth);
            StatsChanged?.Invoke();
            // TODO: This is a temporary solution, death should be handled differently
            if(health == 0) {
                transform.position = new Vector3(0, -100, 0);
                GetComponent<GAgent>()?.ReevaluatePlan();
                GetComponent<Beacon>()?.Destroy(5f);
            }
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

    public float Risk {
        get => risk;
        set {
            risk = value;
            risk = Mathf.Clamp(risk, 0, maxRisk);
            StatsChanged?.Invoke();
        }
    }

    public float CalculateFinalAttack() {
        var finalAttack = attack + UnityEngine.Random.Range(attackModifierMin, attackModifierMax);

        if (Stamina <= 0) return finalAttack / 2f;
        return finalAttack;
    }

    public float CalculateFinalAttackSpeed() {
        var finalAttackSpeed = attackSpeed + UnityEngine.Random.Range(attackSpeedModifierMin, attackSpeedModifierMax);
       
        if (Stamina <= 0) return finalAttackSpeed / 2f;
        return finalAttackSpeed;
    }

    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float MaxOre => maxOre;
    public float MaxRisk => maxRisk;
    public int ID => id;
    public string AgentName => agentName;
    public int TeamId => team.id;
    public Team Team {
        get => team;
        set {
            team = value;
        }
    }

    // public int Attack => attack;
    // public int AttackSpeed => attackSpeed;
    // public int AttackModifierMin => attackModifierMin;
    // public int AttackModifierMax => attackModifierMax;
    // public int AttackSpeedModifierMin => attackSpeedModifierMin;
    // public int AttackSpeedModifierMax => attackSpeedModifierMax;

    void OnDestroy() {
        StatsChanged = null;
    }

    private static int _maxId = -1;
    public static int CalculateId() {
        _maxId++;
        return _maxId;
    }
}
