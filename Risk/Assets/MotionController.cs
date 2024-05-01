using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MotionController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private AgentBrain agentBrain;
    private AgentStatus agentStatus;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        agentBrain = GetComponent<AgentBrain>();
        agentStatus = GetComponent<AgentStatus>();
    }

    private void Update()
    {
        if (agentBrain.goal == AgentBrain.GoalName.SEARCH_FOR_DEPOSIT) {
            // move in random direction
            if (navMeshAgent.remainingDistance < 0.5f) {
                navMeshAgent.SetDestination(transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
            }
        }

        if (agentBrain.goal == AgentBrain.GoalName.FREEZE) {
            navMeshAgent.isStopped = true;
            navMeshAgent.destination = transform.position;
        } else {
            navMeshAgent.isStopped = false;
        }

        if(agentBrain.goal == AgentBrain.GoalName.GO_TO_NEAREST_DEPOSIT) {
            navMeshAgent.SetDestination(agentStatus.nearestSpottedDeposit.transform.position);
        }

        if(agentBrain.goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            navMeshAgent.isStopped = true;
        }

        // When some of animations are interrupted, position and rotation of the avatar are changing. This is a workaround to fix it.
        EqialiseTransforms(transform, transform.GetChild(0)); // TODO: maybe should be used only once at the end of movement
    }

    private void EqialiseTransforms(Transform source, Transform target)
    {
        target.position = source.position;
        target.rotation = source.rotation;
    }
}
