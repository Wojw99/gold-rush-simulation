using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    private Camera mainCamera;

    private void Start() {
        mainCamera = GetComponent<Camera>();
    }

    private void Update() {
        // move camera with WSAD keys
        if (Input.GetKey(KeyCode.W)) {
            mainCamera.transform.position += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S)) {
            mainCamera.transform.position += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A)) {
            mainCamera.transform.position += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            mainCamera.transform.position += new Vector3(1, 0, 0);
        }
        // rotate camera horizontally with Q and E keys
        if (Input.GetKey(KeyCode.Q)) {
            mainCamera.transform.Rotate(new Vector3(0, -1, 0));
        }
        if (Input.GetKey(KeyCode.E)) {
            mainCamera.transform.Rotate(new Vector3(0, 1, 0));
        }
        // rotate camera vertically with R and F keys
        if (Input.GetKey(KeyCode.R)) {
            mainCamera.transform.Rotate(new Vector3(-1, 0, 0));
        }
        if (Input.GetKey(KeyCode.F)) {
            mainCamera.transform.Rotate(new Vector3(1, 0, 0));
        }
    }
}
