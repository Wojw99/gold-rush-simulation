using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    private AgentStatus agentStatus;
    private AgentBrain agentBrain;

    private readonly float staminaDecreaseRate = 1;
    private readonly float staminaIncreaseRate = 1;
    private readonly float healthIncreaseRate = 1;

    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        agentBrain = GetComponent<AgentBrain>();

        agentBrain.DepositExtracted += () => {
            agentStatus.Ore++;
        };

        agentBrain.DamageTaken += () => {
            agentStatus.Health--;
        };
    }

    private void Update() {
        if(agentBrain.Goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            DecreaseStamina();
        } else if (agentBrain.Goal == AgentBrain.GoalName.TAKE_REST) {
            IncreaseStamina();
        } else if (agentBrain.Goal == AgentBrain.GoalName.TAKE_HEALING) {
            IncreaseHealth();
        }
    }

    private void IncreaseHealth() {
        agentStatus.Health += healthIncreaseRate * Time.deltaTime;
    }

    private void DecreaseStamina() {
        agentStatus.Stamina -= staminaDecreaseRate * Time.deltaTime;
    }

    private void IncreaseStamina() {
        agentStatus.Stamina += staminaIncreaseRate * Time.deltaTime;
    }
}
