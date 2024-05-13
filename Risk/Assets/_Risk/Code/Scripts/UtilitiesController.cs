using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitiesController : MonoBehaviour
{
    [SerializeField] private GameObject swordNode;
    [SerializeField] private GameObject shovelNode;

    private AgentBrain agentBrain;

    private void Start()
    {
        agentBrain = GetComponent<AgentBrain>();
        agentBrain.GoalChanged += OnGoalChanged;
    }

    private void OnGoalChanged(AgentBrain.GoalName goal)
    {
        if(goal == AgentBrain.GoalName.ATTACK) {
            swordNode.SetActive(true);
            shovelNode.SetActive(false);
        } else if (goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            swordNode.SetActive(false);
            shovelNode.SetActive(true);
        } else {
            swordNode.SetActive(false);
            shovelNode.SetActive(false);
        }

        if(goal == AgentBrain.GoalName.DIE) {

        }
    }
}
