using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    [Range(10, 100)]
    [SerializeField] float strength = 10;

    [Range(10, 100)]
    [SerializeField] float condition = 10;

    [Range(10, 100)]
    [SerializeField] float fortitude = 10;

    [Range(10, 100)]
    [SerializeField] float speed = 3.5f;

    [Range(10, 100)]
    [SerializeField] float intelligence = 10;

    [Range(1, 100)]
    [SerializeField] float goldRecognition = 50;

    [Range(1, 100)]
    [SerializeField] float plantsRecognition = 50;

    [Range(1, 100)]
    [SerializeField] float miningExpertise = 50;

    float maxGeneral = 100;
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
    [SerializeField] GameObject restGameObject;
    [SerializeField] GameObject shrineGameObject;
    [SerializeField] GameObject storageGameObject;
    int attackModifierMin = 1;
    int attackModifierMax = 10;
    float attackSpeedModifierMin = 0;
    float attackSpeedModifierMax = 0.5f;

    bool isFillingStamina = false;
    bool isDrawingStamina = false;
    float staminaPerTimeUnit = 1f;

    bool isFillingHealth = false;
    bool isDrawingHealth = false;
    float healthPerTimeUnit = 1f;


    CountdownTimer statsTimer;

    public event Action StatsChanged;

    void Awake() {
        maxHealth = 100 + fortitude * 2; 
        maxStamina = 100 + condition * 2;
        maxOre = strength / 10;
        if(maxOre < 1) maxOre = 1;

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
        Health = 20;
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
            health += healthPerTimeUnit;
        } else if (isDrawingHealth) {
            health -= healthPerTimeUnit;
        }

        if(isFillingStamina) {
            stamina += staminaPerTimeUnit;
        } else if (isDrawingStamina) {
            stamina -= staminaPerTimeUnit;
        }

        if(stamina > maxStamina * 0.2f) {
            health += 1;
        }

        if(stamina <= 0) {
            health -= 1;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        health = Mathf.Clamp(health, 0, maxHealth);
        relax = Mathf.Clamp(relax, 0, maxRelax);
        StatsChanged?.Invoke();
        ServeDeath();
    }

    public float Health {
        get => health;
        set {
            health = value;
            health = Mathf.Clamp(health, 0, maxHealth);
            StatsChanged?.Invoke();
            // TODO: This is a temporary solution, death should be handled differently
            ServeDeath();
        }
    }

    private void ServeDeath() {
        if(health <= 0) {
            transform.position = new Vector3(0, -100, 0);
            GetComponent<GAgent>()?.ReevaluatePlan();
            GetComponent<Beacon>()?.Destroy(5f);
            Debug.Log($"{agentName} died");
            GameStatsManager.instance.UpdateStats();
        }
    }

    public bool IsDead => health <= 0;

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
            pyriteModifier = Mathf.Clamp(pyriteModifier, 0, maxOre);
            StatsChanged?.Invoke();
        }
    }

    public float GoldRecognition {
        get => goldRecognition;
        set {
            goldRecognition = value;
            goldRecognition = Mathf.Clamp(goldRecognition, 0, maxGeneral);
            StatsChanged?.Invoke();
        }
    }

    public float PlantsRecognition {
        get => plantsRecognition;
        set {
            plantsRecognition = value;
            plantsRecognition = Mathf.Clamp(plantsRecognition, 0, maxGeneral);
            StatsChanged?.Invoke();
        }
    }

    public float MiningExpertise {
        get => miningExpertise;
        set {
            miningExpertise = value;
            miningExpertise = Mathf.Clamp(miningExpertise, 0, maxGeneral);
            StatsChanged?.Invoke();
        }
    }

    public float CalculateMiningDuration() {
        return 10 - (miningExpertise / 10);
    }

    public float CalculateStorageDuration() {
        return 10 - (miningExpertise / 10);
    }

    public float CalculateLearningIncrement() {
        return intelligence / 10;
    }

    public void StartDrawingStamina(float staminaPerTimeUnit) {
        this.staminaPerTimeUnit = staminaPerTimeUnit;
        isDrawingStamina = true;
        isFillingStamina = false;
    }

    public void StartFillingStamina(float staminaPerTimeUnit) {
        this.staminaPerTimeUnit = staminaPerTimeUnit;
        isFillingStamina = true;
        isDrawingStamina = false;
    }
    
    public void StartDrawingHealth(float healthPerTimeUnit) {
        this.healthPerTimeUnit = healthPerTimeUnit;
        isFillingHealth = false;
        isDrawingHealth = true;
    }

    public void StartFillingHealth(float healthPerTimeUnit) {
        this.healthPerTimeUnit = healthPerTimeUnit;
        isFillingHealth = true;
        isDrawingHealth = false;
    }

    public void StopHealthUpdates() {
        isDrawingHealth = false;
        isFillingHealth = false;
    }

    public void StopStaminaUpdates() {
        isDrawingStamina = false;
        isFillingStamina = false;
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
    public float Strength => strength;
    public float Condition => condition;
    public float Fortitude => fortitude;
    public float SpeedForNavMeshAgent => speed / 10;
    public float Intelligence => intelligence;
    public Vector3 RestPosition => restGameObject.transform.position;
    public Vector3 ShrinePosition => shrineGameObject.transform.position;
    public Vector3 StoragePosition => storageGameObject.transform.position;

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
