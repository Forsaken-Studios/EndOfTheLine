using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    [SerializeField] private DoorUI doorUIKeycard;
    [SerializeField] private GameObject keycardSprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doorUIKeycard.SwapKeycardIcon(false);
            keycardSprite.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doorUIKeycard.SwapKeycardIcon(true);
            keycardSprite.SetActive(false);
        }
    }
}
