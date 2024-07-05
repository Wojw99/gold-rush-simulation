using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    public Quaternion GetLookAtCameraRotation(Transform transform, Camera mainCamera) {
        var newVector = transform.position - mainCamera.transform.position;
        return Quaternion.LookRotation(new Vector3(0f, newVector.y, newVector.z));
    }

    #region Singleton
    private static HelperFunctions _instance;
    public static HelperFunctions Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HelperFunctions();
            }
            return _instance;
        }
    }
    private HelperFunctions() {}
    #endregion
}
