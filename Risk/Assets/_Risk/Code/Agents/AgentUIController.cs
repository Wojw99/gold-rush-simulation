using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentUIController : MonoBehaviour
{
    private readonly string EMPTY_GOAL_ACTION_TEXT = "-";

    [SerializeField] Image healthBar;
    [SerializeField] Image staminaBar;
    [SerializeField] Image oreBar;
    [SerializeField] Image riskBar;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI goalText;
    [SerializeField] TextMeshProUGUI actionText;
    Camera mainCamera;
    Canvas canvas;
    GAgent gAgent;
    AgentStats agentStats;

    void Awake() {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        agentStats = GetComponentInParent<AgentStats>();
        gAgent = GetComponentInParent<GAgent>();
    }

    void Start() {
        gAgent.GoalChanged += OnGoalChanged;
        gAgent.ActionChanged += OnActionChanged;
        agentStats.StatsChanged += OnStatsChanged;
        nameText.color = agentStats.Team.color;
        
        Initialize();
    }

    void Update() {
        canvas.transform.rotation = HelperFunctions.Instance.GetLookAtCameraRotation(transform, mainCamera);
    }

    void Initialize() {
        // TODO: This method may be unnecessary if Awake will be used 
        OnGoalChanged(gAgent.CurrentGoal);
        OnActionChanged(gAgent.CurrentAction);
        OnStatsChanged();
    }

    void OnStatsChanged() {
        healthBar.fillAmount = agentStats.Health / agentStats.MaxHealth;
        staminaBar.fillAmount = agentStats.Stamina / agentStats.MaxStamina;
        oreBar.fillAmount = agentStats.Ore / agentStats.MaxOre;
        riskBar.fillAmount = agentStats.Relax / agentStats.MaxRelax;
        nameText.text = agentStats.AgentName;
    }

    void OnActionChanged(GAgentAction action) {
        if (action != null)
            actionText.text = action.Name;
        else
            goalText.text = EMPTY_GOAL_ACTION_TEXT;
    }

    void OnGoalChanged(GAgentGoal goal) {
        if (goal != null)
            goalText.text = goal.Name;
        else
            goalText.text = EMPTY_GOAL_ACTION_TEXT;
    }
}
