using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI agentStatsText;
    [SerializeField] Image agentStatsPanel;
    [SerializeField] TextMeshProUGUI gameStatsText;

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

    public void UpdateGameStatsText() {
        var color = "#ffffff";
        var currentTime = TimeManager.instance.CurrentTimeFormatted;
        var timeMultiplier = TimeManager.instance.TimeMultiplier;
        var currentGold = GameStatsManager.instance.CurrentGold;
        var maxGold = GameStatsManager.instance.MaxGold;
        var currentMiners = GameStatsManager.instance.CurrentMiners;
        var maxMiners = GameStatsManager.instance.MaxMiners;
        gameStatsText.text =                         
            $"Current time: <color={color}>{currentTime}</color>\n" +
            $"Time multiplier: <color={color}>{timeMultiplier}</color>\n" +
            $"Gold: <color={color}>{currentGold}</color>/{maxGold}\n" +
            $"Miners: <color={color}>{currentMiners}</color>/{maxMiners}\n";
    }

    void ShowAgentStatsText(AgentStats agentStats) {
        var color = "#feec79";
        agentStatsText.text = 
            $"Name: {agentStats.AgentName}\n\n" +
            
            $"Health: <color={color}>{agentStats.Health}</color>/{agentStats.MaxHealth}\n" +
            $"Stamina: <color={color}>{agentStats.Stamina}</color>/{agentStats.MaxStamina}\n" +
            $"Gold+piryte: <color={color}>{agentStats.Ore}</color>/{agentStats.MaxOre}\n\n" +

            $"Strength: <color={color}>{agentStats.Strength}</color>\n" +
            $"Condition: <color={color}>{agentStats.Condition}</color>\n" +
            $"Fortitude: <color={color}>{agentStats.Fortitude}</color>\n" +
            $"Speed: <color={color}>{agentStats.Speed}</color>\n" +
            $"Intelligence: <color={color}>{agentStats.Intelligence}</color>\n\n" +

            $"GoldRecognition: <color={color}>{agentStats.GoldRecognition}</color>\n" +
            $"PlantsRecognition: <color={color}>{agentStats.PlantsRecognition}</color>\n" +
            $"MiningExpertise: <color={color}>{agentStats.MiningExpertise}</color>\n";
        agentStatsPanel.gameObject.SetActive(true);
    }

    public void RemoveAgentStas() {
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
