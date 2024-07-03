using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

[RequireComponent(typeof(SphereCollider))]
public class Sensor : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float evaluationInterval = 0.3f;

    SphereCollider detectionSphere;
    CountdownTimer timer;
    List<TargetInfo> targets = new List<TargetInfo>();

    void Awake() {
        detectionSphere = GetComponent<SphereCollider>();
        detectionSphere.isTrigger = true;
        detectionSphere.radius = detectionRange;
    }

    void Start() {
        timer = new CountdownTimer(evaluationInterval);
        timer.OnTimerStop += () => {
            UpdateTargetList();
            timer.Start();
        };
        timer.Start();
    }

    void Update() {
        timer.Tick(Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Beacon beacon)) {
            var targetInfo = new TargetInfo(beacon.gameObject, Vector3.Distance(transform.position, beacon.Position), beacon.BeaconType);
            targets.Add(targetInfo);
            beacon.BeaconDestroyed += OnBeaconDestroyed;
        }
    }

    void OnBeaconDestroyed(Beacon beacon) {
        targets.RemoveAll(target => target.GameObject == beacon.gameObject);
    }

    void OnTriggerExit(Collider other) {
        if(other.TryGetComponent(out Beacon beacon)) {
            targets.RemoveAll(target => target.GameObject == beacon.gameObject);
        }
    }

    public bool TryGetTargetOfType(BeaconType type, out GameObject target) {
        var nearestTarget = GetNearestTarget(type);
        target = nearestTarget;
        return nearestTarget != null;
    }

    public bool TryGetBeaconOfType(BeaconType type, out Beacon beacon) {
        var nearestTarget = GetNearestTarget(type);
        beacon = nearestTarget?.GetComponent<Beacon>();
        return beacon != null;
    }

    public bool TryGetBuilding(out Building building) {
        var nearestTarget = GetNearestTarget(BeaconType.BUILDING);
        building = nearestTarget?.GetComponent<Building>();
        return building != null;
    }

    public bool ContainsTargetOfType(BeaconType type) {
        return targets.Exists(target => target.BeaconType == type);
    }

    public GameObject GetNearestTarget(BeaconType type) {
        targets.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        var nearestTarget = targets.Find(target => target.BeaconType == type);
        return nearestTarget?.GameObject;
    }

    void UpdateTargetList() {
        // targets.RemoveAll(target => Vector3.Distance(transform.position, target.GameObject.transform.position) > detectionRange);
    }

    void OnDrawGizmos() {
        Gizmos.color = targets.Count > 0 ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}


public class TargetInfo {
    GameObject gameObject;
    float distance;
    BeaconType beaconType;

    public TargetInfo(GameObject gameObject, float distance, BeaconType beaconType) {
        this.gameObject = gameObject;
        this.distance = distance;
        this.beaconType = beaconType;
    }

    public GameObject GameObject => gameObject;
    public float Distance => distance;
    public BeaconType BeaconType => beaconType;
}