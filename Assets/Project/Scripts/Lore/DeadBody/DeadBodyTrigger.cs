using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBodyTrigger : MonoBehaviour
{
    private DeadBody deadBody;
        
    private void Start()
    {
        deadBody = this.gameObject.GetComponentInParent<DeadBody>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!deadBody.GetIfItIsAlreadyLooted())
                deadBody.ActivateKeyHotkeyImage();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            deadBody.DesactivateKeyHotkeyImage();
    }
}
