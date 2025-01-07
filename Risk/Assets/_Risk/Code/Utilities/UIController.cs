using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI agentStatsText;
    [SerializeField] Image agentStatsPanel;
    [SerializeField] TextMeshProUGUI gameStatsText;
    [SerializeField] GameObject stopPanel;

    AgentStats agentStats;

    void Update() {
        if(agentStats != null) {
            ShowAgentStatsText(agentStats);
        }
        UpdateGameStatsText();
    }

    public void UpdateAgentStatsText(AgentStats agentStats) {
        ShowAgentStatsText(agentStats);
        this.agentStats = agentStats;
    }

    public void ShowStopPanel() {
        stopPanel.gameObject.SetActive(true);
    }

    public void HideStopPanel() {
        stopPanel.gameObject.SetActive(false);
    }

    public void UpdateGameStatsText() {
        var color = "#ffffff";
        var team1Color = GameStatsManager.instance.Team1.color;
        var team2Color = GameStatsManager.instance.Team2.color;
        var currentTime = TimeManager.instance.CurrentTimeFormatted;
        var timeMultiplier = TimeManager.instance.TimeMultiplier;
        var currentGold = GameStatsManager.instance.CurrentGold;
        var maxGold = GameStatsManager.instance.MaxGold;
        var currentMiners = GameStatsManager.instance.CurrentAgents;
        var maxMiners = GameStatsManager.instance.MaxAgents;
        gameStatsText.text =                         
            $"Current time: <color={color}>{currentTime}</color>\n" +
            $"Time multiplier: <color={color}>{timeMultiplier}</color>\n" +
            $"Gold in deposits: <color={color}>{currentGold}</color>/{maxGold}\n" +
            $"Miners: <color={color}>{currentMiners}</color>/{maxMiners}\n\n" + 

            $"<color=#{team1Color.ToHexString()}>{GameStatsManager.instance.Team1.name}</color>\n" + 
            $"Miners: {GameStatsManager.instance.Team1.AliveAgents}\n" + 
            $"Gold: {GameStatsManager.instance.Team1.CurrentGold}\n\n" + 

            $"<color=#{team2Color.ToHexString()}>{GameStatsManager.instance.Team2.name}</color>\n" + 
            $"Miners: {GameStatsManager.instance.Team2.AliveAgents}\n" + 
            $"Gold: {GameStatsManager.instance.Team2.CurrentGold}\n\n";
    }

    void ShowAgentStatsText(AgentStats agentStats) {
        var color = "#feec79";
        agentStatsText.text = 
            $"Name: {agentStats.AgentName}\n\n" +
            
            $"Health: <color={color}>{agentStats.Health}</color>/{agentStats.MaxHealth}\n" +
            $"Stamina: <color={color}>{agentStats.Stamina}</color>/{agentStats.MaxStamina}\n" +
            $"Gold: <color={color}>{agentStats.Ore}</color>/{agentStats.MaxOre}\n\n" +
            $"Piryte: <color={color}>{agentStats.PyriteModifier}</color>\n\n" +

            $"Strength: <color={color}>{agentStats.Strength}</color>\n" +
            $"Condition: <color={color}>{agentStats.Condition}</color>\n" +
            $"Fortitude: <color={color}>{agentStats.Fortitude}</color>\n" +
            $"Speed: <color={color}>{agentStats.SpeedForNavMeshAgent}</color>\n" +
            $"Intelligence: <color={color}>{agentStats.Intelligence}</color>\n\n" +

            $"GoldRecognition: <color={color}>{agentStats.GoldRecognition}</color>\n" +
            $"PlantsRecognition: <color={color}>{agentStats.PlantsRecognition}</color>\n" +
            $"MiningExpertise: <color={color}>{agentStats.MiningExpertise}</color>\n";
        agentStatsPanel.gameObject.SetActive(true);
    }

    public void RemoveAgentStatsText() {
        agentStats = null;
        agentStatsText.text = "";
        agentStatsPanel.gameObject.SetActive(false);
    }

    #region Singleton

    public static UIController instance;

    private void Awake() {
        instance = this;
    }

    private UIController() {
        
    }

    #endregion
}
