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
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private GameObject looteableObjectPrefab;

        [Header("Expanded Inventory Passive")]
        [SerializeField] private bool expandedInventory;
        [SerializeField] private bool expandedInventory2;
        [SerializeField] private bool expandedInventory3;
        [SerializeField]private List<ItemSlot> expandedItemSlotsList;
        
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
            //Lo pongo para que haya diferentes mejoras
            if (expandedInventory)
            {
                for (int i = 0; i < 3; i++)
                {
                    itemSlotList.Add(expandedItemSlotsList[i]);
                    expandedItemSlotsList[i].HideBlackPanel();
                }
            }
            if (expandedInventory2)
            {
                for (int i = 3; i < 6; i++)
                {
                    itemSlotList.Add(expandedItemSlotsList[i]);
                    expandedItemSlotsList[i].HideBlackPanel();
                }
            }
            if (expandedInventory3)
            {
                for (int i = 6; i <= 10; i++)
                {
                    itemSlotList.Add(expandedItemSlotsList[i]);
                    expandedItemSlotsList[i].HideBlackPanel();
                }
            }
            base.Start();
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
        }
        public void ActivateInventory()
        {
            base.ActivateInventory();
        }
        public void DesactivateInventory()
        {
            base.DesactivateInventory();
        }
        public void ChangeText(Dictionary<Item, int> inventoryItems)
        {
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

