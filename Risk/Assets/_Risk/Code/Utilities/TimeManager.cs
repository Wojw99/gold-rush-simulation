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

    // get current time
    public float CurrentTime {
        get {
            return Time.time;
        }
    }

    // get current time in format: hh:mm:ss
    public string CurrentTimeFormatted {
        get {
            // return TimeSpan.FromSeconds(Time.time).ToString();
            float gameTime = Time.time; 
            int hours = Mathf.FloorToInt(gameTime / 3600);
            int minutes = Mathf.FloorToInt((gameTime % 3600) / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            string formattedTime = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            return formattedTime;
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