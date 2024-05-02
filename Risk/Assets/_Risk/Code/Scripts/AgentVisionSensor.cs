using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentVisionSensor : MonoBehaviour
{
    private AgentBrain agentBrain;
    private AgentStatus agentStatus;

    public List<VisionObject> visibles = new List<VisionObject>();
    public event Action EnemySpotted;
    public event Action DepositSpotted;
    public event Action HealSpotted;
    public event Action RestSpotted;

    private void Start() {
        agentBrain = GetComponent<AgentBrain>();
        agentStatus = GetComponent<AgentStatus>();
    }

    private void Update() {
        SendSphereCastAll();
    }

    private void SendSphereCastAll() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        visibles.Clear();
        foreach (var hit in hits) {
            if(hit.collider.TryGetComponent(out EnvVisible envVisible)) 
            {
                var visionObject = new VisionObject(envVisible.visionType, hit.collider.gameObject);
                visibles.Add(visionObject);
            } 
        }
    }
}

public class VisionObject {
    public VisionType visionType;
    public GameObject gameObject;

    public VisionObject(VisionType visionType, GameObject gameObject) {
        this.visionType = visionType;
        this.gameObject = gameObject;
    }
}

public enum VisionType {
    DEPOSIT,
    HEAL,
    REST,
    UNDEAD,
}
