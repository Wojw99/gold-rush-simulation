using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10;
    private float _health;
    [SerializeField] private float maxStamina = 20;
    private float _stamina;
    [SerializeField] private float maxOre = 20;
    private float _ore;

    private string _name;
    [SerializeField] private bool isAgressive;
    [SerializeField] private AgentType agentType;

    public event Action<float> HealthChanged;
    public event Action<float> StaminaChanged;
    public event Action<float> OreChanged;
    public event Action<string> NameChanged;

    private void Start() {
        Health = maxHealth;
        Stamina = maxStamina;
        Ore = 0;
        Name = RandomGenerator.Instance.GenerateName();
    }

    public float Health {
        get => _health;
        set {
            _health = Mathf.Clamp(value, 0, maxHealth);
            HealthChanged?.Invoke(_health);
        }
    }

    public float Stamina {
        get => _stamina;
        set {
            _stamina = Mathf.Clamp(value, 0, maxStamina);
            StaminaChanged?.Invoke(_stamina);
        }
    }

    public float Ore {
        get => _ore;
        set {
            _ore = Mathf.Clamp(value, 0, maxOre);
            OreChanged?.Invoke(_ore);
        }
    }

    public string Name {
        get => _name;
        set {
            _name = value;
            NameChanged?.Invoke(_name);
        }
    }

    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float MaxOre => maxOre;
    public bool IsAgressive => isAgressive;
    public AgentType AgentType => agentType;

    private void OnDestroy() {
        HealthChanged = null;
        StaminaChanged = null;
        OreChanged = null;
        NameChanged = null;
    }
}