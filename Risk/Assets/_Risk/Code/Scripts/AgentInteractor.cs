using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractor : MonoBehaviour
{
    private AgentStatus agentStatus;

    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
    }

    public void OnDepositEnter(GameObject depositGameObject)
    {
        if(depositGameObject != null) {
            agentStatus.collidedDeposit = depositGameObject;
        }
    }

    public void OnDepositRunOut()
    {
        agentStatus.collidedDeposit = null;
        agentStatus.nearestSpottedDeposit = null;
    }
}
