using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentVisionSensor : MonoBehaviour
{
    public event Action<BeaconInfo> AgentSpotted;
    public event Action<BeaconInfo> DepositSpotted;
    public event Action<BeaconInfo> HealSpotted;
    public event Action<BeaconInfo> RestSpotted;
    public event Action<BeaconInfo> BeaconLost;

    private List<GameObject> historyBeacons = new List<GameObject>();

    private void Update() {
        SendSphereCastAll();
    }

    private void SendSphereCastAll() {
        var hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        var newBeacons = SearchForBeacons(hits);
        MakeSpottedSignals(newBeacons);
        historyBeacons = newBeacons;
    }

    private List<GameObject> SearchForBeacons(RaycastHit[] hits) {
        var newBeacons = new List<GameObject>();
        foreach (var hit in hits) {
            if(hit.collider.TryGetComponent(out BeaconVisible beacon)) 
            {
                if(beacon.gameObject != gameObject) {
                    newBeacons.Add(beacon.gameObject);
                }
            } 
        }
        return newBeacons;
    }

    private void MakeSpottedSignals(List<GameObject> newBeacons) {
        foreach (var beacon in newBeacons) {
            if (!historyBeacons.Contains(beacon)) {
                var type = beacon.GetComponent<BeaconVisible>().beaconType;
                MakeSignal(new BeaconInfo(type, beacon));
            }
        }
    }

    private void MakeLostSignals(List<GameObject> newBeacons) {
        foreach (var beacon in historyBeacons) {
            if (!newBeacons.Contains(beacon)) {
                var beaconType = beacon.GetComponent<BeaconVisible>().beaconType;
                // OnVisionLost(visionType, beacon);
            }
        }
    }

    public void OnVisionLost(BeaconType visionType, GameObject gameObject) {
        var beaconInfo = new BeaconInfo(visionType, gameObject);
        BeaconLost?.Invoke(beaconInfo);
    }

    private void MakeSignal(BeaconInfo becaonInfo) {
        switch (becaonInfo.beaconType) {
            case BeaconType.DEPOSIT:
                DepositSpotted?.Invoke(becaonInfo);
                break;
            case BeaconType.HEAL:
                HealSpotted?.Invoke(becaonInfo);
                break;
            case BeaconType.REST:
                RestSpotted?.Invoke(becaonInfo);
                break;
            case BeaconType.AGENT:
                AgentSpotted?.Invoke(becaonInfo);
                break;
        }
    }

    private void OnDestroy() {
        BeaconLost = null;
        AgentSpotted = null;
        DepositSpotted = null;
        HealSpotted = null;
        RestSpotted = null;
    }
}

public class BeaconInfo {
    public BeaconType beaconType;
    public GameObject gameObject;

    public BeaconInfo(BeaconType visionType, GameObject gameObject) {
        this.beaconType = visionType;
        this.gameObject = gameObject;
    }
}

public enum BeaconType {
    DEPOSIT,
    HEAL,
    REST,
    AGENT,
}
