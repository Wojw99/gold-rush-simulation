using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentUIController : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image oreBar;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;
    private AgentStatus agentStatus;
    private AgentBrain agentBrain;
    private Camera mainCamera;
    private Canvas canvas;

    private void Start() {
        agentStatus = GetComponentInParent<AgentStatus>();
        agentBrain = GetComponentInParent<AgentBrain>();
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;

        if(agentStatus != null) {
            agentStatus.HealthChanged += OnHealthChanged;
            agentStatus.StaminaChanged += OnStaminaChanged;
            agentStatus.OreChanged += OnOreChanged;
            agentStatus.NameChanged += OnNameChanged;
        }

        if(agentBrain != null) {
            agentBrain.GoalChanged += OnGoalChanged;
            agentBrain.ActionChanged += OnActionChanged;
        }

        Initialize();
    }

    private void Update() {
        canvas.transform.rotation = GetLookAtCameraRotation();
    }

    private void Initialize() {
        OnHealthChanged(agentStatus.Health);
        OnStaminaChanged(agentStatus.Stamina);
        OnOreChanged(agentStatus.Ore);
        OnNameChanged(agentStatus.Name);
        OnGoalChanged(agentBrain.CurrentGoal);
    }

    private Quaternion GetLookAtCameraRotation() {
        var newVector = transform.position - mainCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(0f, newVector.y, newVector.z));
    }

    private void OnNameChanged(string name) {
        nameText.text = name;
    }

    private void OnActionChanged(AgentAction action) {
        if (action != null)
            actionText.text = action.Name;
    }

    private void OnGoalChanged(AgentGoal goal) {
        if (goal != null)
            goalText.text = goal.Name;
    }

    private void OnHealthChanged(float health) {
        healthBar.fillAmount = health / agentStatus.MaxHealth;
    }

    private void OnStaminaChanged(float stamina) {
        staminaBar.fillAmount = stamina / agentStatus.MaxStamina;
    }

    private void OnOreChanged(float ore) {
        oreBar.fillAmount = ore / agentStatus.MaxOre;
    }
}
