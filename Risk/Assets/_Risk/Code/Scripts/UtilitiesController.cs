using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitiesController : MonoBehaviour
{
    [SerializeField] private GameObject weaponSlot;
    [SerializeField] private GameObject shovelPrefab;

    private AgentBrain agentBrain;

    private bool weaponEquipped = false;

    private void Start()
    {
        agentBrain = GetComponent<AgentBrain>();
        // agentBrain.GoalChanged += OnGoalChanged;
    }

    private void OnGoalChanged(AgentBrain.GoalName goal)
    {
        if (goal == AgentBrain.GoalName.MINE_DEPOSIT)
        {
            // var position = new Vector3(0.019f, -0.164f, 0.09f);
            var position = new Vector3(0f, 0f, 0f);
            var rotation = new Vector3(24f, 177f, 170f);

            // add the rotation to the rotation of the weapon slot
            var newRotation = Quaternion.Euler(rotation + weaponSlot.transform.rotation.eulerAngles);

            Instantiate(shovelPrefab, weaponSlot.transform.position, newRotation, weaponSlot.transform);
            weaponEquipped = true;
        }
        else
        {
            if(weaponEquipped) {
                Destroy(weaponSlot.transform.GetChild(0).gameObject);
                weaponEquipped = false;
            }
        }
    }
}
