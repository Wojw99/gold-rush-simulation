using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentVision : MonoBehaviour
{
    private AgentBrain agentBrain;
    private AgentStatus agentStatus;

    private void Start() {
        agentBrain = GetComponent<AgentBrain>();
        agentStatus = GetComponent<AgentStatus>();
    }

    private void Update() {
        if(agentBrain.goal == AgentBrain.GoalName.SEARCH_FOR_DEPOSIT) {
            SendSphereCastAllSortedByDistance();
        }
    }

    private void SendSphereCastAllSortedByDistance() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        if (hits.Length > 0) {
            if(hits[0].collider.TryGetComponent(out Deposit deposit)) {
                if(!deposit.isOccupied) {
                    agentStatus.nearestSpottedDeposit = hits[0].collider.gameObject;
                }
            }
        }
    }
}
