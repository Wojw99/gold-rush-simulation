using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    public GameObject depositPrefab;
    public string menuNumber;
    public bool selected;
    [SerializeField] private TextMeshProUGUI itemTextMesh;
    [SerializeField] private TextMeshProUGUI numTextMesh;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image buttonImage;

    private void Start() {
        itemTextMesh.text = depositPrefab.name;
        numTextMesh.text = menuNumber;    
        // var thumb = AssetPreview.GetMiniThumbnail(depositPrefab);
        // buttonImage.sprite = Sprite.Create(thumb, new Rect(0, 0, thumb.width, thumb.height), new Vector2(0.5f, 0.5f));

        if(selected) {
            backgroundImage.color = new Color32(255, 139, 40, 255);
        } else {
            backgroundImage.color = new Color32(113, 113, 113, 255);
        }
    }
}
