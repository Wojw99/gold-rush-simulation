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
    private AgentStatus agentStatus;
    private AgentBrain agentBrain;
    private Camera mainCamera;
    private Canvas canvas;

    private void Start() {
        agentStatus = GetComponent<AgentStatus>();
        agentBrain = GetComponent<AgentBrain>();
        canvas = GetComponentInChildren<Canvas>();
        mainCamera = Camera.main;

        agentStatus.HealthChanged += OnHealthChanged;
        agentStatus.StaminaChanged += OnStaminaChanged;
        agentStatus.OreChanged += OnOreChanged;
        agentStatus.NameChanged += OnNameChanged;

        agentBrain.GoalChanged += OnGoalChanged;
    }

    private void Update() {
        canvas.transform.rotation = GetLookAtCameraRotation();
    }

    private Quaternion GetLookAtCameraRotation() {
        var newVector = transform.position - mainCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(0f, newVector.y, newVector.z));
    }

    private void OnNameChanged(string name) {
        nameText.text = name;
    }

    private void OnGoalChanged(AgentBrain.GoalName goal) {
        goalText.text = goal.ToString();
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
