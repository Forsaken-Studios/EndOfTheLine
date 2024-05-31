using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Inventory
{

    public class InventoryManager : MonoBehaviour
    {

        public static InventoryManager Instance;

        [SerializeField] private GameObject inventoryHUD;
        [SerializeField] private GameObject inventoryHUDPanel;
        [SerializeField] private TextMeshProUGUI inventoryText;
        [SerializeField] private List<ItemSlot> itemSlotList;

        [SerializeField] private int nextIndexSlotAvailable = 0;
        private int MAX_AMOUNT_PER_SLOT = 4;

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

        private void Start()
        {
            inventoryHUD.SetActive(false);
            inventoryHUDPanel.SetActive(false);
            foreach (var itemSlot in itemSlotList)
            {
                // itemSlot.ClearItemSlot();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inventoryHUD.SetActive(!inventoryHUD.activeSelf);
                inventoryHUDPanel.SetActive(!inventoryHUDPanel.activeSelf);

            }
        }

        public void ChangeText(Dictionary<Item, int> inventoryItems)
        {
            inventoryText.text = "";
            foreach (var item in inventoryItems)
            {
                inventoryText.text += item.Key.name + "  x" + item.Value + "\n";
            }
        }

        public void CheckIfThereIsSlotAvailable()
        {
            
        }

        public bool TryAddInventoryToItemSlot(Item item, int amount)
        {
            int availableIndex = 0;
            foreach (var itemSlot in itemSlotList)
            {
                if (itemSlot.itemID == item.itemID)
                {
                    int totalAmount = itemSlot.amount + amount;
                    if (totalAmount <= MAX_AMOUNT_PER_SLOT)
                    {
                        //ADD ELEMENT TO THIS SLOT
                        itemSlot.AddMoreItemsToSameSlot(amount);
                        return true;
                    }
                    else
                    {
                        //We refill one slot and create another one
                        if (itemSlot.amount != MAX_AMOUNT_PER_SLOT) //We dont check a full slot
                        {
                            int amountToFill = MAX_AMOUNT_PER_SLOT - itemSlot.amount;
                            int amountRemaining = amount - amountToFill;
                            itemSlot.AddMoreItemsToSameSlot(amountToFill);
                            if (amountRemaining > 0)
                            {
                                
                                availableIndex = GetFirstIndexSlotAvailable();
                                if (availableIndex != -1)
                                {
                                    itemSlotList[availableIndex]
                                        .SetItemSlotProperties(item.itemIcon, amountRemaining, item.itemID); 
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            //WE CREATE A NEW SLOT, IF AVAILABLE (NEED TO CHECK)
            availableIndex = GetFirstIndexSlotAvailable();

            if (availableIndex != -1)
            {
                Debug.Log(availableIndex);
                itemSlotList[availableIndex].SetItemSlotProperties(item.itemIcon, amount, item.itemID);
                return true; 
            }
            else
            {
                return false;
            }
        
        }

        private int GetFirstIndexSlotAvailable()
        {
            for (int i = 0; i < itemSlotList.Count; i++)
            {
                if (itemSlotList[i].itemID == 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}

