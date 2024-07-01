using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    [SerializeField] BeaconType beaconType;
    public event Action<Beacon> BeaconDestroyed;

    public BeaconType BeaconType => beaconType;
    public Vector3 Position => transform.position;
    public bool IsActive => gameObject.activeSelf;

    int occupierId = -1;

    public void AddOccupierId(int id) {
        occupierId = id;
    }

    public void ClearOccupierId() {
        occupierId = -1;
    }

    public bool IsOccupiedByStranger(int agentId) {
        return occupierId != -1 && occupierId != agentId;
    }

    public void Destroy() {
        Destroy(gameObject);
    }

    private void OnDestroy() {
        BeaconDestroyed?.Invoke(this);
        BeaconDestroyed = null;
    }

    public int OccupierId => occupierId; 
}

public enum BeaconType {
    DEPOSIT,
    HEAL,
    REST,
    AGENT,
    DAMAGE,
}
