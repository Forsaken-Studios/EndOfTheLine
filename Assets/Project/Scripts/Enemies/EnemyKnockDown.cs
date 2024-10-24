using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockDown : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyEvents.OnIsOnBack?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyEvents.OnIsOnBack?.Invoke(false);
        }
    }
}
