using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject depositPrefab;
    [SerializeField] private GameObject restPrefab;
    [SerializeField] private GameObject[] prefabsArray = new GameObject[10];
    private GameObject selectedPrefab;

    private void Start() {
        selectedPrefab = prefabsArray[0];
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                var spawnPoint = new Vector3(hit.point.x, hit.point.y + 1f, hit.point.z);
                Instantiate(selectedPrefab, spawnPoint, Quaternion.identity);
            }
        }

        for (int i = 0; i < 9; i++) {
            if (Input.GetKeyDown((i + 1).ToString())) {
                selectedPrefab = prefabsArray[i];
            }
        }
    }
}
