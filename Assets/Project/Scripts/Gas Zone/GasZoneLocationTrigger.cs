using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class GasZoneLocationTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.ActivateSoundByName(SoundAction.GasZone_Geiger);
            other.gameObject.GetComponent<PlayerManager>().PlayerEnteredGasZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.StopSound();
            other.gameObject.GetComponent<PlayerManager>().PlayerExitedGasZone();
        }
    }
}
