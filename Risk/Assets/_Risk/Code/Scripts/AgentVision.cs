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
        SendSphereCastAll();
    }

    private void SendSphereCastAll() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        var spottedUndeads = new List<GameObject>();
        var spottedDeposits = new List<GameObject>();
        var spottedRests = new List<GameObject>();

        foreach (var hit in hits) {
            if(hit.collider.TryGetComponent(out UndeadInteractor undeadInteractor)) 
            {
                spottedUndeads.Add(hit.collider.gameObject);
            } 
            else if (hit.collider.TryGetComponent(out Deposit deposit)) 
            {
                spottedDeposits.Add(hit.collider.gameObject);
                deposit.OnSeen();
            }
            else if (hit.collider.TryGetComponent(out Campfire campfire)) 
            {
                spottedRests.Add(hit.collider.gameObject);
            }
        }

        if (spottedDeposits.Count > 0) {
            var dep = spottedDeposits[0];
            if(!dep.GetComponent<Deposit>().isOccupied) {
                agentStatus.nearestSpottedDeposit = dep;
            }
        }

        if (spottedRests.Count > 0) {
            var rest = spottedRests[0];
            agentStatus.nearestSpottedRest = rest;
        }
    }
}
