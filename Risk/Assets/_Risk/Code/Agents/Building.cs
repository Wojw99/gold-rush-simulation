using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] List<GameObject> buildingParts;
    [SerializeField] Image oreProgressBar;
    [SerializeField] Image breedProgressBar;
    [SerializeField] Material materialTemplate;
    Canvas canvas;
    Camera mainCamera;
    int _currentSize = 0;
    [SerializeField] GameObject agentPrefab = null;
    [SerializeField] Team ownerTeam = null;
    [SerializeField] int startSize = 0;
    [SerializeField] float breedInterval = 100f;
    CountdownTimer timer;

    void Awake() {
        canvas = GetComponentInChildren<Canvas>();
        mainCamera = Camera.main;
    }

    void Start() {
        foreach (var part in buildingParts) {
            part.SetActive(false);
        }

        oreProgressBar.gameObject.SetActive(true);
        breedProgressBar.gameObject.SetActive(false);

        oreProgressBar.fillAmount = 0;
        breedProgressBar.fillAmount = 0;
        CurrentSize = startSize;
        if(ownerTeam != null) {
            ColorParts(ownerTeam.color);
        }
    }

    void Update() {
        canvas.transform.rotation = HelperFunctions.Instance.GetLookAtCameraRotation(transform, mainCamera);

        if(timer != null) {
            timer.Tick(Time.deltaTime);
            breedProgressBar.fillAmount = 1f - timer.Progress;
        }
    }

    public void ContinueBuilding(Team team) {
        if(ownerTeam == null) {
            ownerTeam = team;
            ColorParts(team.color);
        } else if (ownerTeam != team) {
            Debug.LogWarning("The agent attempt to build a building that is already owned by another team");
            return;
        } else if (IsComplete) {
            Debug.LogWarning("The agent attempt to build a building that is already complete");
            return;
        }
        CurrentSize += 6;
        if(IsComplete) {
            OnBuildingComplete();
        }   
    }

    void OnBuildingComplete() {
        BreedNewAgent();
        SetupBreedTimer();

        oreProgressBar.gameObject.SetActive(false);
        breedProgressBar.gameObject.SetActive(true);

        breedProgressBar.color = ownerTeam.color;
    }

    void SetupBreedTimer() {
        timer = new CountdownTimer(breedInterval);
        timer.OnTimerStop += () => {
            if(IsComplete) {
                BreedNewAgent();
                timer.Start();
            }
        };
        timer.Start();
    }

    void BreedNewAgent() {
        agentPrefab.GetComponent<AgentStats>().Team = ownerTeam;
        var agent = Instantiate(agentPrefab, transform.position, Quaternion.identity);
    }

    void UpdateActiveState() {
        for (int i = 0; i < buildingParts.Count; i++) {
            if(i < CurrentSize) {
                buildingParts[i].SetActive(true);
            } else {
                buildingParts[i].SetActive(false);
            }
        }
    }

    void ColorParts(Color color) {
        foreach (var part in buildingParts) {
            var partRenderer = part.GetComponent<Renderer>();
            var partMaterial = new Material(materialTemplate)
            {
                color = color
            };
            partRenderer.material = partMaterial;
        }
    }

    public bool CanBeBuilt(int agentTeamId) {
        return !IsComplete && (ownerTeam == null || ownerTeam.id == agentTeamId);
    }

    int CurrentSize {
        get {
            return _currentSize;
        }
        set {
            _currentSize = value;
            _currentSize = Mathf.Clamp(_currentSize, 0, buildingParts.Count);
            oreProgressBar.fillAmount = (float)_currentSize / buildingParts.Count;
            UpdateActiveState();
        }
    }

    public bool IsComplete => CurrentSize == buildingParts.Count;
}
