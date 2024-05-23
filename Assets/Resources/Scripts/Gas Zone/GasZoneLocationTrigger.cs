using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasZoneLocationTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("GAS ZONE");
            other.gameObject.GetComponent<PlayerManager>().PlayerEnteredGasZone();
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerManager>().PlayerExitedGasZone();
        }
    }
}
