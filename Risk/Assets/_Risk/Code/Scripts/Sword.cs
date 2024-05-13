using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private ModifierType modifierType = ModifierType.DAMAGE;
    private float damageTime = 1f;
    private bool isDamage = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentInteractionSensor agentInteractionSensor) && isDamage)
        {
            var modifier = new ModifierInfo(modifierType, 1f, 1f);
            agentInteractionSensor.OnModifierEnter(modifier);
            isDamage = false;
            StartCoroutine(ResetDamage());
        }
    }

    private IEnumerator ResetDamage()
    {
        yield return new WaitForSeconds(damageTime);
        isDamage = true;
    }
}
