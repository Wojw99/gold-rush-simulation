using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgentSettings : MonoBehaviour
{
    [SerializeField] List<GoalSetting> goalSettings = new List<GoalSetting>();

    public List<GoalSetting> GoalSettings => goalSettings;
}

[System.Serializable]
public class GoalSetting {
    public string goalName;
    public int goalPriority;
    public string goalDesiredEffect;
    public bool isActive;
}