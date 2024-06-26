using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AgentSensor : MonoBehaviour
{
    [SerializeField] GameObject testVisual;
    public List<Beacon> beacons = new List<Beacon>();
    public event Action BeaconSensed;

    void Start()
    {
        beacons = new List<Beacon>();
    }

    void Update()
    {

    }

    public bool IsBeaconSensible(BeaconType beaconType) {
        for (int i = 0; i < beacons.Count; i++) {
            if (beacons[i].BeaconType == beaconType) {
                return true;
            }
        }

        return false;
    }

    public Beacon GetAndRemoveNearestBeacon(BeaconType beaconType) {
        var beacon = GetNearestBeacon(beaconType);
        if(beacon != null) {
            beacons.Remove(beacon);
        }
        return beacon;
    }

    public Beacon GetNearestBeacon(BeaconType beaconType) {
        beacons.Sort((a, b) => Vector3.Distance(a.Position, transform.position).CompareTo(Vector3.Distance(b.Position, transform.position)));
        
        for (int i = 0; i < beacons.Count; i++) {
            if (beacons[i].BeaconType == beaconType) {
                return beacons[i];
            }
        }

        return null;
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<Beacon>() != null) {
            var beacon = other.gameObject.GetComponent<Beacon>();
            beacons.Add(beacon);
            beacon.BeaconDestroyed += OnBeaconDestroyed;
            BeaconSensed?.Invoke();
            // SpawnTestVisual(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject.GetComponent<Beacon>() != null) {
            beacons.Remove(other.gameObject.GetComponent<Beacon>());
        }
    }

    void OnBeaconDestroyed(Beacon beacon) {
        beacons.Remove(beacon);
    }

    void SpawnTestVisual(GameObject other) {
        var visual = Instantiate(testVisual, other.transform);
            visual.transform.position = new Vector3(visual.transform.position.x, visual.transform.position.y + 1, visual.transform.position.z);
    }
}
