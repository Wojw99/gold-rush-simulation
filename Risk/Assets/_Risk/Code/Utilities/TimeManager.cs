using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    int _timeMultiplier = 1;
    bool timePaused = false;

    readonly int minTimeMultiplier = 1;
    readonly int maxTimeMultiplier = 10;

    public event Action<int> TimeMultiplierChanged;

    public void FastForwardTime() {
        TimeMultiplier += 1;
        PlayTime();
    }

    public void SlowDownTime() {
        TimeMultiplier -= 1;
        PlayTime();
    }

    public void PlayPauseTime() {
        if(timePaused) {
            PlayTime();
        } else {
            PauseTime();
        }
    }

    void PlayTime() {
        timePaused = false;
        Time.timeScale = 1 * TimeMultiplier;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void PauseTime() {
        timePaused = true;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
    }

    void ResetTimeParameters() {
        TimeMultiplier = 1;
        timePaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    public int TimeMultiplier {
        get {
            return _timeMultiplier;
        }
        set {
            _timeMultiplier = value;
            _timeMultiplier = Mathf.Clamp(_timeMultiplier, minTimeMultiplier, maxTimeMultiplier);
            TimeMultiplierChanged?.Invoke(_timeMultiplier);
        }
    }

    #region Singleton

    public static TimeManager instance;

    private void Awake() {
        instance = this;
    }

    private TimeManager() {
        
    }

    #endregion
}
