using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject selectionPrefab;
    [SerializeField] GameObject[] prefabsArray = new GameObject[10];
    
    int _selectedPrefabIndex = -1;
    GameObject _selectedAgent = null;
    GameObject _selectionMarker;

    void Start() {
        SelectedPrefabIndex = -1;
    }

    void Update() {
        ChangeSelectedPrefabIndex();
        SpawnPrefab();
        SelectPosition();
        SelectAgent();
        ClearSelection();
        ManageTime();
    }

    void ManageTime() {
        if(Input.GetKeyDown(KeyCode.RightBracket)) 
        {
            TimeManager.instance.FastForwardTime();
        } 
        else if (Input.GetKeyDown(KeyCode.Space)) 
        {
            TimeManager.instance.PlayPauseTime();
        } 
        else if (Input.GetKeyDown(KeyCode.LeftBracket)) 
        {
            TimeManager.instance.SlowDownTime();
        }
    }

    void ClearSelection () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            SelectedPrefabIndex = -1;
            SelectedAgent = null;
            ClearSelectionMarker();
        }
    }

    void SelectPosition() {
        if(SelectedPrefabIndex != -1 || SelectedAgent == null) return;
        if(Input.GetMouseButtonDown(0)) {
            var mouseGroundPosition = GetMouseGroundPosition();
            ClearSelectionMarker();
            SelectionMarker = Instantiate(selectionPrefab, mouseGroundPosition, Quaternion.identity);
        }
    }

    void SelectAgent() {
        if (SelectedPrefabIndex != -1) return;
        if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var layerMask = LayerMask.GetMask("Agent");
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
                var agent = hit.collider.gameObject;
                if(agent.TryGetComponent(out AgentStats agentStats)) {
                    SelectedAgent = agent;
                    Debug.Log("Selected agent: " + agentStats.AgentName);
                }
            }
        }
    }

    void ClearSelectionMarker() {
        if (SelectionMarker != null) {
            Destroy(SelectionMarker);
        }
    }

    Vector3 GetMouseGroundPosition() {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        var layerMask = LayerMask.GetMask("Terrain");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
            return hit.point;
        }
        return Input.mousePosition;
    }

    void SpawnPrefab() {
        if(SelectedPrefabIndex == -1) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            var mouseGroundPosition = GetMouseGroundPosition();
            var spawnPosition = mouseGroundPosition + Vector3.up * 0.01f;
            var prefab = prefabsArray[SelectedPrefabIndex];
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }

    void ChangeSelectedPrefabIndex() {
        for (int i = 0; i < prefabsArray.Length; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                SelectedPrefabIndex = i;
            }
        }
    }

    public Vector3 GetMarkerPosition() {
        if(SelectionMarker == null) {
            return Vector3.zero;
        }
        return SelectionMarker.transform.position;
    }

    public GameObject SelectionMarker {
        get {
            return _selectionMarker;
        } 
        set {
            _selectionMarker = value;
            SelectionMarkerChanged?.Invoke();
        }
    }

    public event Action<GameObject[]> PrefabsSelectionChanged;

    public event Action SelectionMarkerChanged;
    public event Action SelectionChanged;

    public GameObject[] PrefabsArray {
        get => prefabsArray;
    }

    public int SelectedPrefabIndex {
        get => _selectedPrefabIndex;
        set {
            _selectedPrefabIndex = value;
            PrefabsSelectionChanged?.Invoke(prefabsArray);
        }
    }

    public GameObject SelectedAgent {
        get => _selectedAgent;
        set {
            _selectedAgent = value;
            SelectionChanged?.Invoke();
        }
    }

    private void OnDestroy() {
        SelectionChanged = null;
        PrefabsSelectionChanged = null;
    }
}
