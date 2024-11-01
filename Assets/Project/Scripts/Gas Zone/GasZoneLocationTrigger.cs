using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class GasZoneLocationTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource = SoundManager.Instance.ActivateSoundByName(SoundAction.GasZone_Geiger, null, true);
            other.gameObject.GetComponent<PlayerManager>().PlayerEnteredGasZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(audioSource != null)
                Destroy(audioSource.gameObject);
            other.gameObject.GetComponent<PlayerManager>().PlayerExitedGasZone();
        }
    }
}
