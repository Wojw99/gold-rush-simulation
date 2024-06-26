using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus : MonoBehaviour
{
    [SerializeField] float maxHealth = 10;
    float _health;
    [SerializeField] float maxStamina = 20;
    float _stamina;
    [SerializeField] float maxOre = 20;
    float _ore;

    private string _name;
    [SerializeField] private bool isAgressive;
    [SerializeField] private AgentType agentType;

    public event Action<float> HealthChanged;
    public event Action<float> StaminaChanged;
    public event Action<float> OreChanged;
    public event Action<string> NameChanged;

    HealthState _healthState;
    StaminaState _staminaState;
    OreState _oreState;

    public event Action StateChange;

    void Start() {
        Health = maxHealth;
        Stamina = maxStamina;
        Ore = 0;
        Name = RandomGenerator.Instance.GenerateName();
    }

    Coroutine staminaCoroutine;
    Coroutine healthCoroutine;
    Coroutine oreCoroutine;

    public void StartDrawing(AttributeName attributeName, float costPerSecond = 2, float interval = 0.05f) 
    {
        var costPerInterval = costPerSecond * interval;

        if(attributeName == AttributeName.Health) {
            healthCoroutine = StartCoroutine(DrawHealth(costPerInterval, interval));
        }
        else if(attributeName == AttributeName.Stamina) {
            staminaCoroutine = StartCoroutine(DrawStamina(costPerInterval, interval));
        } 
        else if(attributeName == AttributeName.Ore) {
            oreCoroutine = StartCoroutine(DrawOre(costPerInterval, interval));
        }    
    }

    public void StopDrawing(AttributeName attributeName) 
    {
        try {
            if(attributeName == AttributeName.Health) {
                StopCoroutine(healthCoroutine);
            }
            else if(attributeName == AttributeName.Stamina) {
                StopCoroutine(staminaCoroutine);
            } 
            else if(attributeName == AttributeName.Ore) {
                StopCoroutine(oreCoroutine);
            }
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    IEnumerator DrawStamina(float costPerInterval, float interval) 
    {
        while(true) 
        {
            Stamina -= costPerInterval;
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator DrawHealth(float costPerInterval, float interval) 
    {
        while(true) 
        {
            Health -= costPerInterval;
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator DrawOre(float costPerInterval, float interval) 
    {
        while(true) 
        {
            Ore -= costPerInterval;
            yield return new WaitForSeconds(interval);
        }
    }

    void UpdateHealthState() {
        if(Health == 0) {
            HealthState = HealthState.Died;
        }
        else if(Health < maxHealth / 3) {
            HealthState = HealthState.Dying;
        }
        else if(Health < maxHealth) {
            HealthState = HealthState.Injured;
        }
        else {
            HealthState = HealthState.Healthy;
        }
    }

    void UpdateStaminaState() {
        if(Stamina < maxStamina / 3) {
            StaminaState = StaminaState.Exhausted;
        }
        else {
            StaminaState = StaminaState.Rested;
        }
    }

    void UpdateOreState() {
        if(Ore == maxOre) {
            OreState = OreState.Full;
        }
        else {
            OreState = OreState.NotFull;
        }
    }

    public HealthState HealthState {
        get => _healthState;
        set {
            _healthState = value;
            StateChange?.Invoke();
        }
    }

    public StaminaState StaminaState {
        get => _staminaState;
        set {
            _staminaState = value;
            StateChange?.Invoke();
        }
    }

    public OreState OreState {
        get => _oreState;
        set {
            _oreState = value;
            StateChange?.Invoke();
        }
    }

    public float Health {
        get => _health;
        set {
            _health = Mathf.Clamp(value, 0, maxHealth);
            HealthChanged?.Invoke(_health);
            UpdateHealthState();
        }
    }

    public float Stamina {
        get => _stamina;
        set {
            _stamina = Mathf.Clamp(value, 0, maxStamina);
            StaminaChanged?.Invoke(_stamina);
            UpdateStaminaState();
        }
    }

    public float Ore {
        get => _ore;
        set {
            _ore = Mathf.Clamp(value, 0, maxOre);
            OreChanged?.Invoke(_ore);
            UpdateOreState();
        }
    }

    public string Name {
        get => _name;
        set {
            _name = value;
            NameChanged?.Invoke(_name);
        }
    }

    public float GetAttribute(AttributeName attributeName) {
        if(attributeName == AttributeName.Health) {
            return Health;
        }
        else if(attributeName == AttributeName.Stamina) {
            return Stamina;
        }
        else if(attributeName == AttributeName.Ore) {
            return Ore;
        }
        throw new Exception("Attribute not found");
    }

    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float MaxOre => maxOre;
    public bool IsAgressive => isAgressive;
    public AgentType AgentType => agentType;

    void OnDestroy() {
        HealthChanged = null;
        StaminaChanged = null;
        OreChanged = null;
        NameChanged = null;
    }
}

public enum AttributeName {
    Health,
    Stamina,
    Ore
}

public enum HealthState {
    Died,
    Dying,
    Injured,
    Healthy
}

public enum StaminaState {
    Exhausted,
    Rested
}

public enum OreState {
    NotFull,
    Full
}