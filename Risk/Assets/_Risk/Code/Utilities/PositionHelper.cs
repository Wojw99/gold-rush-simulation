using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHelper : MonoBehaviour
{
    [SerializeField] GameObject hellPosition;

    public GameObject HellPosition => hellPosition;

    #region Singleton

    public static PositionHelper instance;

    private void Awake() {
        instance = this;
    }

    private PositionHelper() {
        
    }

    #endregion
}
