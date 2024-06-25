using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

namespace Loot
{

    public class LooteableObjectTrigger : MonoBehaviour
    {
        private LooteableObject looteableObject;
        
        private void Start()
        {
            looteableObject = this.gameObject.GetComponentInParent<LooteableObject>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                looteableObject.ActivateKeyHotkeyImage();
                LooteableObjectSelector.Instance.AddOneInTrigger(looteableObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                looteableObject.DesactivateKeyHotkeyImage();
                LooteableObjectSelector.Instance.DecreaseOneInTrigger(looteableObject);
            }
        }
    }
}
