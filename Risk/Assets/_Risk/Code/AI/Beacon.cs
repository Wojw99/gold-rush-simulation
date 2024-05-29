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

    public void Destroy() {
        Destroy(gameObject);
    }

    private void OnDestroy() {
        BeaconDestroyed?.Invoke(this);
        BeaconDestroyed = null;
    }
}

public enum BeaconType {
    DEPOSIT,
    HEAL,
    REST,
    AGENT,
}
