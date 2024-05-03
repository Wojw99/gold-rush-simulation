using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentVisionSensor : MonoBehaviour
{
    public List<VisionInfo> visibles = new List<VisionInfo>();
    public event Action EnemySpotted;
    public event Action DepositSpotted;
    public event Action HealSpotted;
    public event Action RestSpotted;

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
                MakeSignal(envVisible.visionType);
                AddToVisibles(envVisible.visionType, hit.collider.gameObject);
            } 
        }
    }

    private void AddToVisibles(VisionType visionType, GameObject gameObject) {
        var visionObject = new VisionInfo(visionType, gameObject);
        visibles.Add(visionObject);
    }

    private void MakeSignal(VisionType visionType) {
        switch (visionType) {
            case VisionType.DEPOSIT:
                DepositSpotted?.Invoke();
                break;
            case VisionType.HEAL:
                HealSpotted?.Invoke();
                break;
            case VisionType.REST:
                RestSpotted?.Invoke();
                break;
            case VisionType.UNDEAD:
                EnemySpotted?.Invoke();
                break;
        }
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
    UNDEAD,
}
