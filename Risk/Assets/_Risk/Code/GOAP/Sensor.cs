using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtils;

[RequireComponent(typeof(SphereCollider))]
public class Sensor : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float timerInterval = 1f;
    public DetectionTypeName detectionType = DetectionTypeName.Nothing;

    private SphereCollider detectionRange;

    public event Action OnTargetChanged = delegate { };

    public Vector3 TargetPosition => target ? target.transform.position : Vector3.zero;
    public bool IsTargetInRange => TargetPosition != Vector3.zero;

    private GameObject target;
    private Vector3 lastKnownPosition;
    private CountdownTimer timer;

    public Sensor WithDetectionType(DetectionTypeName detectionType) {
        // return copy of this Sensor with new detection type
        var sensor = Instantiate(this);
        sensor.detectionType = detectionType;
        return sensor;
    }

    private void Awake() {
        detectionRange = GetComponent<SphereCollider>();
        detectionRange.isTrigger = true;
        detectionRange.radius = detectionRadius;
    }

    private void Start() {
        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () => {
            UpdateTargetPosition(target.OrNull());
            timer.Start();
        };
        timer.Start();
    }

    private void Update() {
        timer.Tick(Time.deltaTime);
    }

    private void UpdateTargetPosition(GameObject target = null) {
        this.target = target;
        if(IsTargetInRange && (lastKnownPosition != TargetPosition || lastKnownPosition != Vector3.zero)) {
            lastKnownPosition = TargetPosition;
            OnTargetChanged.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("$ Sensor: trigerred {other.gameObject.name}");
        if(other.TryGetComponent(out Detectable detectable)) {
            if(detectable.DetectionType == detectionType) {
                Debug.Log("$ Sensor: is detection type.");
                UpdateTargetPosition(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.TryGetComponent(out Detectable detectable)) {
            if(detectable.DetectionType == detectionType) {
                UpdateTargetPosition(other.gameObject);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = IsTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

public enum DetectionTypeName {
    Nothing,
    Deposit,
    Enemy,
}