using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentVisionSensor : MonoBehaviour
{
    public event Action<VisionInfo> AgentSpotted;
    public event Action<VisionInfo> DepositSpotted;
    public event Action<VisionInfo> HealSpotted;
    public event Action<VisionInfo> RestSpotted;
    public event Action<VisionInfo> VisionLost;

    private void Update() {
        SendSphereCastAll();
    }

    private void SendSphereCastAll() {
        var hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (var hit in hits) {
            if(hit.collider.TryGetComponent(out BeaconVisible beacon)) 
            {
                var visionInfo = new VisionInfo(beacon.visionType, beacon.gameObject);
                MakeSignal(visionInfo);
            } 
        }
    }

    public void OnVisionLost(VisionType visionType, GameObject gameObject) {
        var visionInfo = new VisionInfo(visionType, gameObject);
        VisionLost?.Invoke(visionInfo);
    }

    private void MakeSignal(VisionInfo visionInfo) {
        switch (visionInfo.visionType) {
            case VisionType.DEPOSIT:
                DepositSpotted?.Invoke(visionInfo);
                break;
            case VisionType.HEAL:
                HealSpotted?.Invoke(visionInfo);
                break;
            case VisionType.REST:
                RestSpotted?.Invoke(visionInfo);
                break;
            case VisionType.AGENT:
                AgentSpotted?.Invoke(visionInfo);
                break;
        }
    }

    private void OnDestroy() {
        VisionLost = null;
        AgentSpotted = null;
        DepositSpotted = null;
        HealSpotted = null;
        RestSpotted = null;
    }
}

public class VisionInfo {
    public VisionType visionType;
    public GameObject gameObject;

    public VisionInfo(VisionType visionType, GameObject gameObject) {
        this.visionType = visionType;
        this.gameObject = gameObject;
    }
}

public enum VisionType {
    DEPOSIT,
    HEAL,
    REST,
    AGENT,
}
