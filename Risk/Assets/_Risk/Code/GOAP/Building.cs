using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] List<GameObject> buildingParts;
    [SerializeField] Image progressBar;
    Canvas canvas;
    Camera mainCamera;
    int maxSize = 0;
    int _currentSize = 0;

    void Start() {
        canvas = GetComponentInChildren<Canvas>();
        mainCamera = Camera.main;

        foreach (var part in buildingParts) {
            part.SetActive(false);
        }

        maxSize = buildingParts.Count;
        progressBar.fillAmount = 0;
    }

    void Update() {
        canvas.transform.rotation = HelperFunctions.Instance.GetLookAtCameraRotation(transform, mainCamera);
    }

    public void ContinueBuilding() {
        foreach (var part in buildingParts) {
            if(part.activeSelf == false) {
                part.SetActive(true);
                CurrentSize += 1;
                break;
            }
        }
    }

    int CurrentSize {
        get {
            return _currentSize;
        }
        set {
            _currentSize = value;
            progressBar.fillAmount = (float)_currentSize / maxSize;
        }
    }

    public bool IsComplete => CurrentSize == maxSize;
}
