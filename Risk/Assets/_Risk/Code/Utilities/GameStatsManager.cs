using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    float _currentGold = 0;
    float _maxGold = 1000;
    int _currentAgents = 0;
    int _maxAgents = 10;

    public float CurrentGold => _currentGold;
    public float MaxGold => _maxGold;
    public int CurrentAgents => _currentAgents;
    public int MaxAgents => _maxAgents;

    private void Start() {
        _maxGold = CountGoldInDeposits();
        _currentGold = CountGoldInStorages();
        _maxAgents = CountAgents();
        _currentAgents = CountAgents();
    }

    public void UpdateStats() {
        _currentGold = CountGoldInStorages();
        _currentAgents = CountAgents();
    }

    private float CountGoldInStorages() {
        var buildings = FindObjectsOfType<Building>();
        var gold = 0f;

        foreach(var building in buildings) {
            gold += building.GoldAmount;
        }

        return gold;
    }

    private int CountAgents() {
        var agents = FindObjectsOfType<AgentStats>();
        var aliveAgents = 0;
        
        foreach(var agent in agents) {
            if(!agent.IsDead) {
                aliveAgents += 1;
            }
        }

        return aliveAgents;
    }

    private float CountGoldInDeposits() {
        var deposits = FindObjectsOfType<Deposit>();
        var gold = 0f;
        foreach(var deposit in deposits) {
            if(!deposit.IsPyrite) {
                gold += deposit.GoldAmount;
            }
        }
        return gold;
    }

    #region Singleton

    public static GameStatsManager instance;

    private void Awake() {
        instance = this;
    }

    private GameStatsManager() {
        
    }

    #endregion
}
