using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    private AgentStatus agentStatus;
    private AgentBrain agentBrain;

    private readonly float staminaDecreaseRate = 0.1f;
    private readonly float staminaIncreaseRate = 0.1f;
    private readonly float healthIncreaseRate = 0.1f;
    private readonly float selfDamageValue = 5;

    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        agentBrain = GetComponent<AgentBrain>();

        agentBrain.DepositExtracted += () => {
            agentStatus.Ore++;
        };

        agentBrain.DamageTaken += (damageValue) => {
            agentStatus.Health -= damageValue;
        };

        agentBrain.GoalChanged += OnGoalChanged;
    }

    private void OnGoalChanged(AgentBrain.GoalName goal)
    {
        var delay = 0.1f;
        var action = new Action(() => { });

        if(agentBrain.Goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            action = DecreaseStamina;
        } else if (agentBrain.Goal == AgentBrain.GoalName.TAKE_REST) {
            action = IncreaseStamina;
        } else if (agentBrain.Goal == AgentBrain.GoalName.TAKE_HEALING) {
            action = IncreaseHealth;
        }

        StartCoroutine(ChangeParameterContinuously(delay, goal, action));
    }

    private IEnumerator ChangeParameterContinuously(float delay, AgentBrain.GoalName goal, Action parameterChangeAction) {
        parameterChangeAction();
        yield return new WaitForSeconds(delay);
        if(goal == agentBrain.Goal) {
            StartCoroutine(ChangeParameterContinuously(delay, goal, parameterChangeAction));
        }
    }

    private void IncreaseHealth() {
        agentStatus.Health += healthIncreaseRate;
    }

    private void DecreaseStamina() {
        agentStatus.Stamina -= staminaDecreaseRate;
    }

    private void IncreaseStamina() {
        agentStatus.Stamina += staminaIncreaseRate;
    }
}
