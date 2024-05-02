using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    public GameObject sphere;
    public float maxOre = 5;
    private float currentOre;
    private AgentInteractor agentInteractor;
    public bool isOccupied = false;

    private void Start() {
        currentOre = maxOre;
    }

    private void Update() {
        DecrementOreIfOccupied();
    }

    private void DecrementOreIfOccupied() {
        if (isOccupied) {
            currentOre -= Time.deltaTime;
            if (currentOre <= 0) {
                isOccupied = false;
                agentInteractor.OnDepositExtracted();
                Destroy(gameObject);
            }
        }
    }

    public void OnSeen() {
        if(sphere.activeSelf) return;
        sphere.SetActive(true);
        StartCoroutine(DeactivateAfterSeconds(3));
    }

    private IEnumerator DeactivateAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        sphere.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOccupied && other.TryGetComponent(out AgentInteractor agentInteractor))
        {
            agentInteractor.OnDepositEnter(gameObject);
            this.agentInteractor = agentInteractor;
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (isOccupied && other.TryGetComponent(out AgentInteractor agentInteractor))
        {
            isOccupied = false;
            agentInteractor.OnDepositInterrupted();
            this.agentInteractor = null;
        }
    }
}
