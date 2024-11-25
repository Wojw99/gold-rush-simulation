using System;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    [SerializeField] float strength = 1; // 0 - 10
    [SerializeField] float condition = 10; // 0 - 100
    [SerializeField] float fortitude = 10; // 0 - 100
    [SerializeField] float speed = 10;
    [SerializeField] float miningExpertise = 50; // 0 - 100
    [SerializeField] float plantExpertise = 50; // 0 - 100

    float maxHealth = 100;
    float maxStamina = 100;
    float maxOre = 100;
    float maxRelax = 100;
    int maxCost = 100;
    float health = 100;
    float stamina = 100;
    float ore = 100;
    float relax = 100;
    float pyriteModifier = 100;

    int id = CalculateId();
    [SerializeField] string agentName = null;
    [SerializeField] int attack = 10;
    [SerializeField] float attackSpeed = 1;
    [SerializeField] Team team;
    int attackModifierMin = 1;
    int attackModifierMax = 10;
    float attackSpeedModifierMin = 0;
    float attackSpeedModifierMax = 0.5f;

    public bool isFillingHealth = false;
    public bool isFillingStamina = false;
    public bool isDrawingStamina = false;
    public bool isDrawingHealth = false;
    public bool isFillingRelax = false;

    CountdownTimer statsTimer;

    public event Action StatsChanged;

    void Awake() {
        maxHealth = 100 + fortitude; 
        maxStamina = 100 + condition;
        maxOre = 2 + strength;

        health = maxHealth; 
        stamina = maxStamina;
        ore = 0;

        relax = 0;
        pyriteModifier = 0;

        if(agentName.IsNullOrEmpty()) {
            agentName = RandomGenerator.Instance.GenerateName();
        }
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

        if(isFillingRelax) {
            relax += 10;
        } else {
            relax -= 1;
        }

        if(stamina > maxStamina * 0.2f) {
            health += 1;
        } 

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        health = Mathf.Clamp(health, 0, maxHealth);
        relax = Mathf.Clamp(relax, 0, maxRelax);
        StatsChanged?.Invoke();
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

    public float Relax {
        get => relax;
        set {
            relax = value;
            relax = Mathf.Clamp(relax, 0, maxRelax);
            StatsChanged?.Invoke();
        }
    }
    
    public float PyriteModifier {
        get => pyriteModifier;
        set {
            pyriteModifier = value;
        }
    }

    public float MiningExpertise {
        get => miningExpertise;
        set {
            miningExpertise = value;
        }
    }

    public float PlantExpertise {
        get => plantExpertise;
        set {
            plantExpertise = value;
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
    public float MaxRelax => maxRelax;
    public int ID => id;
    public string AgentName => agentName;
    public int TeamId => team.id;

    public Team Team {
        get => team;
        set {
            team = value;
        }
    }

    void OnDestroy() {
        StatsChanged = null;
    }

    private static int _maxId = -1;
    public static int CalculateId() {
        _maxId++;
        return _maxId;
    }
}
