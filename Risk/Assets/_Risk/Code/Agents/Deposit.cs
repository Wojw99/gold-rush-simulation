using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    int occupierId = -1;
    
    public bool IsFreeToMine(int agentId) {
        return occupierId == -1 || occupierId == agentId;
    }

    public void Occupy(int agentId) {
        occupierId = agentId;
    }

    public void Release(int agentId) {
        if(occupierId == agentId) {
            occupierId = -1;
        }
    }
}
