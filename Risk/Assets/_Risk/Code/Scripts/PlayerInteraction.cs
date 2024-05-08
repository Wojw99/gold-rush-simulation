using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    #region Singleton

    public static PlayerInteraction instance;

    private void Awake() {
        instance = this;
    }

    private PlayerInteraction() {

    }

    #endregion

    [SerializeField] private GameObject fancySpherePrefab;
    [SerializeField] private GameObject[] prefabsArray = new GameObject[10];
    [SerializeField] private int selectedPrefabIndex = -1;

    private GameObject selectedAgent = null;

    public event Action<GameObject[]> PrefabsArrayChanged;

    private void Start() {
        InitializeSelectedPrefabIndex();
    }

    private void Update() {
        ChangeSelectedIndexOnInput();
        SpawnPrefabOnInput();
        SelectAgentOnInput();
        OrderAgentMovement();
        ClearSelection();
    }

    private void ClearSelection () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            selectedPrefabIndex = -1;
            PrefabsArrayChanged?.Invoke(prefabsArray);
        }
    }

    private void OrderAgentMovement() {
        if (selectedPrefabIndex != -1) return;
        if (selectedAgent != null && Input.GetMouseButtonDown(1)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var agentInteraction = selectedAgent.GetComponent<AgentInteractionSensor>();
                var destination = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                // spawn fancy sphere at collider position
                var fancySphere = Instantiate(fancySpherePrefab, destination, Quaternion.identity);
                agentInteraction.OnPlayerOrder(fancySphere);
            }
        }
    }

    private void SelectAgentOnInput() {
        if (selectedPrefabIndex != -1) return;
        if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var agentInteraction = hit.collider.GetComponent<AgentInteractionSensor>();
                if (agentInteraction != null) {
                    agentInteraction.OnPlayerSelect();

                    if (selectedAgent != null) {
                        selectedAgent.GetComponent<AgentInteractionSensor>().OnPlayerDeselect();
                    }

                    selectedAgent = hit.collider.gameObject;
                } else {
                    if (selectedAgent != null) {
                        selectedAgent.GetComponent<AgentInteractionSensor>().OnPlayerDeselect();
                        selectedAgent = null;
                    }
                }
            }
        }
    }

    private void InitializeSelectedPrefabIndex() {
        selectedPrefabIndex = -1;
        PrefabsArrayChanged?.Invoke(prefabsArray);
    }

    private void SpawnPrefabOnInput() {
        if(selectedPrefabIndex == -1) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var spawnPoint = new Vector3(hit.point.x, hit.point.y + 1f, hit.point.z);
                var prefab = prefabsArray[selectedPrefabIndex];
                Instantiate(prefab, spawnPoint, Quaternion.identity);
            }
        }
    }

    private void ChangeSelectedIndexOnInput() {
        for (int i = 0; i < prefabsArray.Length; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                selectedPrefabIndex = i;
                PrefabsArrayChanged?.Invoke(prefabsArray);
            }
        }
    }

    public GameObject[] PrefabsArray {
        get => prefabsArray;
    }

    public int SelectedPrefabIndex {
        get => selectedPrefabIndex;
    }
}
