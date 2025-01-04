using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    int _currentGold = 0;
    int _maxGold = 1000;
    int _currentMiners = 0;
    int _maxMiners = 10;

    public int CurrentGold => _currentGold;
    public int MaxGold => _maxGold;
    public int CurrentMiners => _currentMiners;
    public int MaxMiners => _maxMiners;

    #region Singleton

    public static GameStatsManager instance;

    private void Awake() {
        instance = this;
    }

    private GameStatsManager() {
        
    }

    #endregion
}
