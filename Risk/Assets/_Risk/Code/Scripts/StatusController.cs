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
            agentStatus.ore++;
        };

        agentBrain.DamageTaken += () => {
            agentStatus.health--;
        };
    }

    private void Update() {
        if(agentBrain.goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            DecreaseStamina();
        } else if (agentBrain.goal == AgentBrain.GoalName.TAKE_REST) {
            IncreaseStamina();
        } else if (agentBrain.goal == AgentBrain.GoalName.TAKE_HEALING) {
            IncreaseHealth();
        }
    }

    private void IncreaseHealth() {
        agentStatus.health += healthIncreaseRate * Time.deltaTime;
    }

    private void DecreaseStamina() {
        agentStatus.stamina -= staminaDecreaseRate * Time.deltaTime;
    }

    private void IncreaseStamina() {
        agentStatus.stamina += staminaIncreaseRate * Time.deltaTime;
    }
}
