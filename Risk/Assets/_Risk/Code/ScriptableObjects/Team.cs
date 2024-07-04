using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Risk/Team")]
public class Team : ScriptableObject
{
    [Tooltip("Agent with different ID will be fighting each other.")]
    public int id = 0;

    [Tooltip("The name of the team")]
    public string name = "Agents";

    [Tooltip("The color of the team")]
    public Color color = Color.white;

    [Tooltip("The visual model for agents of this team")]
    public GameObject visualModel;
}
