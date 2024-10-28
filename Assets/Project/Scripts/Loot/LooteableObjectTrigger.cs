using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using LootSystem;
using UnityEngine;

namespace Loot
{

    public class LooteableObjectTrigger : MonoBehaviour
    {
        private LooteableObject looteableObject;
        private GameObject shortCut;
        private void Start()
        {
            looteableObject = this.gameObject.GetComponentInParent<LooteableObject>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                looteableObject.ActivateKeyHotkeyImage();
                shortCut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.lootCrate);
                LooteableObjectSelector.Instance.AddOneInTrigger(looteableObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                looteableObject.DesactivateKeyHotkeyImage();
                ShortcutsUIManager.Instance.RemoveShortcut(shortCut);
                LooteableObjectSelector.Instance.DecreaseOneInTrigger(looteableObject);
            }
        }
    }
}
