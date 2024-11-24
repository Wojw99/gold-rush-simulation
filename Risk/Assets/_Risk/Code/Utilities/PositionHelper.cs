using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHelper : MonoBehaviour
{
    [SerializeField] GameObject campGameObject;
    [SerializeField] GameObject storageGameObject;

    public Vector3 GetCampPosition()
    {
        return campGameObject.transform.position;
    }

    public Vector3 GetStoragePosition()
    {
        return storageGameObject.transform.position;
    }

    #region Singleton

    public static PositionHelper instance;

    private void Awake() {
        instance = this;
    }

    private PositionHelper() {
        
    }

    #endregion
}
