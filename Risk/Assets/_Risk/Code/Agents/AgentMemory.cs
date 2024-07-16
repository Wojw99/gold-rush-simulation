using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentMemory : MonoBehaviour
{
    List<TargetInfo> targets = new List<TargetInfo>();
    
    void Awake() {
        
    }

    public void Memorize(TargetInfo targetInfo) {
        targets.Add(targetInfo);
    }

    public GameObject GetNearestTarget(BeaconType type) {
        targets.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        var nearestTarget = targets.Find(target => target.BeaconType == type);
        return nearestTarget?.GameObject;
    }

    public bool ContainsTargetOfType(BeaconType type) {
        return targets.Exists(target => target.BeaconType == type);
    }

    public List<TargetInfo> Targets => targets;
}
