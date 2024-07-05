using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] List<GameObject> buildingParts;
    [SerializeField] Image progressBar;
    [SerializeField] Material materialTemplate;
    Canvas canvas;
    Camera mainCamera;
    int _currentSize = 0;
    [SerializeField] GameObject agentPrefab = null;
    [SerializeField] Team ownerTeam = null;
    [SerializeField] int startSize = 0;

    void Start() {
        canvas = GetComponentInChildren<Canvas>();
        mainCamera = Camera.main;

        foreach (var part in buildingParts) {
            part.SetActive(false);
        }

        progressBar.fillAmount = 0;
        CurrentSize = startSize;
        if(ownerTeam != null) {
            ColorParts(ownerTeam.color);
        }
    }

    void Update() {
        canvas.transform.rotation = HelperFunctions.Instance.GetLookAtCameraRotation(transform, mainCamera);
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
            BreedNewAgent();
        }
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
            var partMaterial = new Material(materialTemplate);
            partMaterial.color = color;
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
            progressBar.fillAmount = (float)_currentSize / buildingParts.Count;
            UpdateActiveState();
        }
    }

    public bool IsComplete => CurrentSize == buildingParts.Count;
}
