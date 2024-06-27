using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentUIController : MonoBehaviour
{
    private readonly string EMPTY_GOAL_ACTION_TEXT = "-";

    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image oreBar;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;
    private Camera mainCamera;
    private Canvas canvas;
    private GAgent gAgent;

    private void Start() {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;

        gAgent = GetComponentInParent<GAgent>();
        gAgent.GoalChanged += OnGoalChanged;
        gAgent.ActionChanged += OnActionChanged;

        Initialize();
    }

    private void Update() {
        canvas.transform.rotation = GetLookAtCameraRotation();
    }

    private void Initialize() {
        // TODO: This method may be unnecessary if Awake will be used 

        // OnHealthChanged(agentStatus.Health);
        // OnStaminaChanged(agentStatus.Stamina);
        // OnOreChanged(agentStatus.Ore);
        // OnNameChanged(agentStatus.Name);
        OnGoalChanged(gAgent.CurrentGoal);
        OnActionChanged(gAgent.CurrentAction);
    }

    private Quaternion GetLookAtCameraRotation() {
        var newVector = transform.position - mainCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(0f, newVector.y, newVector.z));
    }

    private void OnNameChanged(string name) {
        nameText.text = name;
    }

    private void OnActionChanged(GAgentAction action) {
        if (action != null)
            actionText.text = action.Name;
        else
            goalText.text = EMPTY_GOAL_ACTION_TEXT;
    }

    private void OnGoalChanged(GAgentGoal goal) {
        if (goal != null)
            goalText.text = goal.Name;
        else
            goalText.text = EMPTY_GOAL_ACTION_TEXT;
    }

    private void OnHealthChanged(float health) {
        healthBar.fillAmount = health / 1f; // agentStatus.MaxHealth;
    }

    private void OnStaminaChanged(float stamina) {
        staminaBar.fillAmount = stamina / 1f; // agentStatus.MaxStamina;
    }

    private void OnOreChanged(float ore) {
        oreBar.fillAmount = ore / 1f; // agentStatus.MaxOre;
    }
}
