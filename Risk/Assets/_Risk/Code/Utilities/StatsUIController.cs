using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatsUIController : MonoBehaviour
{
    VisualElement ui;

    void Awake() {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    void OnEnable() {
            
    }
}
