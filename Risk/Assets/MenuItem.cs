using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    [SerializeField] private GameObject depositPrefab;
    [SerializeField] private string menuNumber;
    [SerializeField] private bool selected;
    [SerializeField] private TextMeshProUGUI itemTextMesh;
    [SerializeField] private TextMeshProUGUI numTextMesh;
    [SerializeField] private Image backgroundImage;

    private void Start() {
        itemTextMesh.text = depositPrefab.name;
        numTextMesh.text = menuNumber;    

        if(selected) {
            backgroundImage.color = new Color32(255, 139, 40, 255);
        } else {
            backgroundImage.color = new Color32(113, 113, 113, 255);
        }
    }
}
