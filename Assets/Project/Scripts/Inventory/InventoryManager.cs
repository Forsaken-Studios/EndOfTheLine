using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils.CustomLogs;

namespace Inventory
{

    public class InventoryManager : IInventoryManager
    {

        public static InventoryManager Instance;

        [Header("Inventory Panels")]
        [SerializeField] private GameObject inventoryHUDPanel;
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private TextMeshProUGUI inventoryText;
        [SerializeField] private GameObject looteableObjectPrefab;

        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one InventoryManager! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

        }
        
        public override void Start()
        {
            base.Start();
            inventoryHUDPanel.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ReverseInventoryStatus();
                if(LootUIManager.Instance.GetIfCrateIsOpened())
                    LootUIManager.Instance.DesactivateLootUIPanel();
            }
        }

        /// <summary>
        /// If inventory is opened, we close it, if it is the other way, we open it
        /// </summary>
        public void ReverseInventoryStatus()
        {
            base.ReverseInventoryStatus();
            inventoryHUDPanel.SetActive(!inventoryHUDPanel.activeSelf);
        }
        public void ActivateInventory()
        {
            base.ActivateInventory();
            inventoryHUDPanel.SetActive(true);
        }
        public void DesactivateInventory()
        {
            base.DesactivateInventory();
            inventoryHUDPanel.SetActive(false);
        }
        public void ChangeText(Dictionary<Item, int> inventoryItems)
        {
            inventoryText.text = "";
            foreach (var item in inventoryItems)
            {
                inventoryText.text += item.Key.name + "  x" + item.Value + "\n";
            }

            weightText.text = PlayerInventory.Instance.GetCurrentWeight().ToString("F2") + " / " +
                              PlayerInventory.Instance.GetMaxWeight() + " KG";
        }
        /// <summary>
        /// Methods that finds and set the first available spot for item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="remainingItemsWithoutSpace"></param>
        /// <returns></returns>
        public bool TryAddInventoryToItemSlot(Item item, int amount, out int remainingItemsWithoutSpace)
        {
            return base.TryAddInventoryToItemSlot(item, amount, out remainingItemsWithoutSpace);
        }
        
        public GameObject GetLooteableObjectPrefab()
        {
            return looteableObjectPrefab; 
        }


    }
}

