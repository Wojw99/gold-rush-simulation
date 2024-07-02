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
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI goalText;
    [SerializeField] TextMeshProUGUI actionText;
    Camera mainCamera;
    Canvas canvas;
    GAgent gAgent;
    AgentStats agentStats;

    void Start() {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        agentStats = GetComponentInParent<AgentStats>();
        gAgent = GetComponentInParent<GAgent>();

        gAgent.GoalChanged += OnGoalChanged;
        gAgent.ActionChanged += OnActionChanged;
        agentStats.StatsChanged += OnStatsChanged;

        Initialize();
    }

    void Update() {
        canvas.transform.rotation = GetLookAtCameraRotation();
    }

    void Initialize() {
        // TODO: This method may be unnecessary if Awake will be used 
        OnGoalChanged(gAgent.CurrentGoal);
        OnActionChanged(gAgent.CurrentAction);
        OnStatsChanged();
    }

    Quaternion GetLookAtCameraRotation() {
        var newVector = transform.position - mainCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(0f, newVector.y, newVector.z));
    }

    void OnStatsChanged() {
        healthBar.fillAmount = agentStats.Health / agentStats.MaxHealth;
        staminaBar.fillAmount = agentStats.Stamina / agentStats.MaxStamina;
        oreBar.fillAmount = agentStats.Ore / agentStats.MaxOre;
        nameText.text = agentStats.Name;
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
