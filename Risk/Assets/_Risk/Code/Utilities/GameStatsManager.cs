using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    float _currentGold = 0;
    float _maxGold = 1000;
    int _currentAgents = 0;
    int _maxAgents = 10;
    bool _timePaused = false;

    [SerializeField] Team[] teams;
    [SerializeField] Building[] storages;

    public float CurrentGold => _currentGold;
    public float MaxGold => _maxGold;
    public int CurrentAgents => _currentAgents;
    public int MaxAgents => _maxAgents;
    public Team[] Teams => teams;
    public Building[] Buildings => storages;

    private void Start() {
        _maxGold = CountGoldInDeposits();
        _currentGold = CountGoldInDeposits();
        _maxAgents = CountAgents();
        _currentAgents = CountAgents();
    }

    private void Update() {
        UpdateStats();
        PauseTimeIfNecessery();
    }

    private void CountAgentsInTeam(Team team, Building camp) {
        var agents = FindObjectsOfType<AgentStats>();
        var aliveAgents = 0;
        var gold = 0f;

        foreach(var agent in agents) {
            if(agent.Team == team ) {
                if(!agent.IsDead) {
                    aliveAgents += 1;
                }
                gold += agent.Ore;
            }
        }
        gold += camp.GoldAmount;

        team.AliveAgents = aliveAgents;
        team.CurrentGold = gold;
    }

    private void PauseTimeIfNecessery() {
        if(_currentAgents == 0 || _currentGold == 0) {
            if(!_timePaused) {
                TimeManager.instance.PauseTime();
                _timePaused = true;
            }
        }
    }

    public void UpdateStats() {
        _currentGold = CountGoldInDeposits();
        _currentAgents = CountAgents();
        for(int i = 0; i < teams.Length; i++) {
            CountAgentsInTeam(teams[i], storages[i]);
        }
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