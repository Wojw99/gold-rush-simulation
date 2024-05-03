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
        if (agentBrain.goal == AgentBrain.GoalName.SEARCH_FOR_DEPOSIT 
        || agentBrain.goal == AgentBrain.GoalName.SEARCH_FOR_REST) {
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
            var destination = GetTransformOfNearestVisible(agentVisionSensor.visibles, VisionType.DEPOSIT);
            navMeshAgent.SetDestination(destination.position);
        }

        if(agentBrain.goal == AgentBrain.GoalName.GO_TO_NEAREST_REST) {
            var destination = GetTransformOfNearestVisible(agentVisionSensor.visibles, VisionType.REST);
            navMeshAgent.SetDestination(destination.position);
        }

        if(agentBrain.goal == AgentBrain.GoalName.MINE_DEPOSIT 
        || agentBrain.goal == AgentBrain.GoalName.TAKE_REST) {
            navMeshAgent.isStopped = true;
        }

        if(agentBrain.goal == AgentBrain.GoalName.RUN_FOR_YOUR_LIFE) {
            // set destination to the opposite side of the nearest spotted undead
            var nearestSpottedUndead = GetTransformOfNearestVisible(agentVisionSensor.visibles, VisionType.UNDEAD);

            navMeshAgent.SetDestination(transform.position + (transform.position - nearestSpottedUndead.transform.position));
            navMeshAgent.speed = runSpeed;
        }

        // When some of animations are interrupted, position and rotation of the avatar are changing. This is a workaround to fix it.
        EqialiseTransforms(transform, transform.GetChild(0)); // TODO: maybe should be used only once at the end of movement
    }

    private Transform GetTransformOfNearestVisible(List<VisionInfo> visibles, VisionType visionType)
    {
        var visibleOrdered = visibles.OrderBy(v => Vector3.Distance(transform.position, v.gameObject.transform.position));
        var nearestVisible = visibleOrdered.FirstOrDefault(v => v.visionType == visionType);
        return nearestVisible.gameObject.transform;
    }

    private void EqialiseTransforms(Transform source, Transform target)
    {
        target.position = source.position;
        target.rotation = source.rotation;
    }
}
