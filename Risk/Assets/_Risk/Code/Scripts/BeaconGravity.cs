using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconGravity : MonoBehaviour
{
    private Rigidbody rb;
    private void Start() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(SetKinematic());
    }

    private IEnumerator SetKinematic() {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = true;
    }
}
