using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractor agentInteractor))
        {
            agentInteractor.OnRestEnter(gameObject);
        }
    }
}
