using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI goldAmountText;
    Camera mainCamera;
    float _goldAmount = 0;

    void Awake() {
        mainCamera = Camera.main;
        goldAmountText.text = _goldAmount.ToString();
    }

    void Update() {
        canvas.transform.rotation = HelperFunctions.Instance.GetLookAtCameraRotation(transform, mainCamera);
    }

    public void AddGold(float amount, float pyriteModifier) {
        if(amount > 0) {
            var addedGold = amount - pyriteModifier;
            _goldAmount += addedGold;
            UpdateUI(); 
            Debug.Log($"{pyriteModifier} of {amount} is pyrite. Added {amount} gold to building.");
        } else {
            Debug.LogError("Amount of added gold cannot be less than or equal to 0.");
        }
    }

    public void UpdateUI() {
        goldAmountText.text = _goldAmount.ToString();
    }

    public bool IsComplete => true;
}
