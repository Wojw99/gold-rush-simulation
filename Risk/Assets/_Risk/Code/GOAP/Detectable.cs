using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detectable : MonoBehaviour
{
    [SerializeField] private DetectionTypeName detectionType = DetectionTypeName.Nothing;

    public DetectionTypeName DetectionType => detectionType;
}
