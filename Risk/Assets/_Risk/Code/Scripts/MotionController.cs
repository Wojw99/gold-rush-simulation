using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MotionController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private AgentBrain agentBrain;
    private AgentStatus agentStatus;
    private AgentVisionSensor agentVisionSensor;

    private float walkSpeed = 2.5f;
    private float runSpeed = 6f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        agentBrain = GetComponent<AgentBrain>();
        agentStatus = GetComponent<AgentStatus>();
        agentVisionSensor = GetComponent<AgentVisionSensor>();
    }

    private void Update()
    {
        if (agentBrain.Goal == AgentBrain.GoalName.SEARCH_FOR_DEPOSIT 
        || agentBrain.Goal == AgentBrain.GoalName.SEARCH_FOR_REST
        || agentBrain.Goal == AgentBrain.GoalName.SEARCH_FOR_HEALING) {
            // move in random direction
            if (navMeshAgent.remainingDistance < 0.5f) {
                navMeshAgent.SetDestination(transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
            }
        }

        if (agentBrain.Goal == AgentBrain.GoalName.FREEZE) {
            navMeshAgent.isStopped = true;
            navMeshAgent.destination = transform.position;
        } else {
            navMeshAgent.isStopped = false;
        }

        if(agentBrain.Goal == AgentBrain.GoalName.GO_TO_NEAREST_DEPOSIT
        || agentBrain.Goal == AgentBrain.GoalName.GO_TO_NEAREST_REST
        || agentBrain.Goal == AgentBrain.GoalName.GO_TO_NEAREST_HEALING
        || agentBrain.Goal == AgentBrain.GoalName.GO_TO_DESTINATION) {
            var destination = agentBrain.Destination;
            if(destination != null)
                navMeshAgent.SetDestination(destination.transform.position);
        }

        if (agentBrain.Goal == AgentBrain.GoalName.RUN_FOR_YOUR_LIFE 
        || agentBrain.Goal == AgentBrain.GoalName.GO_TO_DESTINATION) {
            navMeshAgent.speed = runSpeed;
        } else {
            navMeshAgent.speed = walkSpeed;
        }

        if(agentBrain.Goal == AgentBrain.GoalName.MINE_DEPOSIT 
        || agentBrain.Goal == AgentBrain.GoalName.TAKE_REST 
        || agentBrain.Goal == AgentBrain.GoalName.TAKE_HEALING
        || agentBrain.Goal == AgentBrain.GoalName.TAKE_DAMAGE
        || agentBrain.Goal == AgentBrain.GoalName.DIE) {
            navMeshAgent.isStopped = true;
        }

        if(agentBrain.Goal == AgentBrain.GoalName.RUN_FOR_YOUR_LIFE) {
            // set destination in the opposite direction to the current target
            var destination = agentBrain.Destination;
            var direction = (transform.position - destination.transform.position).normalized;
            navMeshAgent.SetDestination(transform.position + direction * 10);
        }

        // When some of animations are interrupted, position and rotation of the avatar are changing. This is a workaround to fix it.
        EqialiseTransforms(transform, transform.GetChild(0)); // TODO: maybe should be used only once at the end of movement
    }

    private void EqialiseTransforms(Transform source, Transform target)
    {
        if(agentBrain.Goal != AgentBrain.GoalName.DIE) {
            target.position = source.position;
            target.rotation = source.rotation;
        }
    }
}
