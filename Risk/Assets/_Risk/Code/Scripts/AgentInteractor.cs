using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractor : MonoBehaviour
{
    private AgentStatus agentStatus;
    private AgentBrain agentBrain;

    private void Update() {

    }

    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        agentBrain = GetComponent<AgentBrain>();
    }

    public void OnDepositInterrupted() {
        agentStatus.collidedDeposit = null;
    }

    public void OnDepositEnter(GameObject depositGameObject)
    {
        if(depositGameObject != null) {
            agentStatus.collidedDeposit = depositGameObject;
        }
    }

    public void OnDepositExtracted()
    {
        agentStatus.collidedDeposit = null;
        agentStatus.nearestSpottedDeposit = null;
        agentStatus.depositExtracted = true;
    }

    public void OnRestInterrupted() {
        agentStatus.collidedRest = null;
    }

    public void OnRestEnter(GameObject restGameObject)
    {
        if(restGameObject != null) {
            agentStatus.collidedRest = restGameObject;
        }
    }

    public void OnRestEnded()
    {
        agentStatus.collidedRest = null;
        agentStatus.nearestSpottedRest = null;
    }
}
