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

    [SerializeField] private GameObject[] prefabsArray = new GameObject[10];
    [SerializeField] private int selectedPrefabIndex = 0;

    public event Action<GameObject[]> PrefabsArrayChanged;

    private void Start() {
        InitializeSelectedPrefabIndex();
    }

    private void Update() {
        ChangeSelectedIndexOnInput();
        SpawnPrefabOnInput();
    }

    private void InitializeSelectedPrefabIndex() {
        selectedPrefabIndex = 0;
        PrefabsArrayChanged?.Invoke(prefabsArray);
    }

    private void SpawnPrefabOnInput() {
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
