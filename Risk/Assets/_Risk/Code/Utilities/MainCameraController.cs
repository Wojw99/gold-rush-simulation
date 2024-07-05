using System.Collections;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    RTS_Camera rtsCamera;

    float keyboardMovementSpeedInitial;
    float screenEdgeMovementSpeedInitial;
    float rotationSpedInitial;
    float followingSpeedInitial;
    float panningSpeedInitial;
    float mouseRotationSpeedInitial;
    float scrollWheelZoomingSensitivityInitial;
    float keyboardZoomingSensitivityInitial;

    void Start() {
        rtsCamera = GetComponent<RTS_Camera>();
        TimeManager.instance.TimeMultiplierChanged += OnTimeMultiplierChanged;
        
        keyboardMovementSpeedInitial = rtsCamera.keyboardMovementSpeed;
        screenEdgeMovementSpeedInitial = rtsCamera.screenEdgeMovementSpeed;
        rotationSpedInitial = rtsCamera.rotationSped;
        followingSpeedInitial = rtsCamera.followingSpeed;
        panningSpeedInitial = rtsCamera.panningSpeed;
        mouseRotationSpeedInitial = rtsCamera.mouseRotationSpeed;
        scrollWheelZoomingSensitivityInitial = rtsCamera.scrollWheelZoomingSensitivity;
        keyboardZoomingSensitivityInitial = rtsCamera.keyboardZoomingSensitivity;
    }

    void OnTimeMultiplierChanged(int timeMultiplier) {
        float tmf = (float)timeMultiplier;

        rtsCamera.keyboardMovementSpeed = keyboardMovementSpeedInitial / tmf;
        rtsCamera.screenEdgeMovementSpeed = screenEdgeMovementSpeedInitial / tmf;
        rtsCamera.rotationSped = rotationSpedInitial / tmf;
        rtsCamera.followingSpeed = followingSpeedInitial / tmf;
        rtsCamera.panningSpeed = panningSpeedInitial / tmf;
        rtsCamera.mouseRotationSpeed = mouseRotationSpeedInitial / tmf;
        rtsCamera.scrollWheelZoomingSensitivity = scrollWheelZoomingSensitivityInitial / tmf;
        rtsCamera.keyboardZoomingSensitivity = keyboardZoomingSensitivityInitial / tmf;

        Debug.Log($"Time Multiplier Changed: {timeMultiplier}, (float){tmf}");
    }
}
