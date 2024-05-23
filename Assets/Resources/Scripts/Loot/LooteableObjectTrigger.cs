using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooteableObjectTrigger : MonoBehaviour
{
    private LooteableObject looteableObject;
    
    private void Start()
    {
        looteableObject = this.gameObject.GetComponent<LooteableObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            looteableObject.ActivateKeyHotkeyImage();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            looteableObject.DesactivateKeyHotkeyImage();
        }
    }
}
