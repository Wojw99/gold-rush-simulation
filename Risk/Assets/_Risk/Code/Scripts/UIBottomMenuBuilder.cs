using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIBottomMenuBuilder : MonoBehaviour
{
    [SerializeField] private GameObject menuItemPrefab;

    private readonly float xStart = -390f;
    private readonly float menuItemWidth = 65f;

    private void Start() {
        PlayerInteraction.instance.PrefabsArrayChanged += OnPrefabsArrayChanged;
    }

    private void OnPrefabsArrayChanged(GameObject[] prefabsArray) {
        CleanMenu();
        UpdateBottomMenu(prefabsArray);
    }

    private void CleanMenu() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    private void UpdateBottomMenu(GameObject[] prefabsArray) {
        for (int i = 1; i < prefabsArray.Length; i++) {
            if(prefabsArray[i] != null) {
                BuildItem(i, prefabsArray, xStart);
            }
        }
        BuildItem(0, prefabsArray, prefabsArray.Length * menuItemWidth + xStart);
    }

    private void BuildItem(int index, GameObject[] prefabsArray, float xStart) {
        var menuItemObj = Instantiate(menuItemPrefab, transform);
        var menuItem = menuItemObj.GetComponent<MenuItem>();

        var rectTransform = menuItemObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xStart + (index * menuItemWidth), 0);
            
        menuItem.depositPrefab = prefabsArray[index];
        menuItem.menuNumber = index.ToString();
        menuItem.selected = index == PlayerInteraction.instance.SelectedPrefabIndex;
    }
}
