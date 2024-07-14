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

    public class InventoryManager : MonoBehaviour
    {

        public static InventoryManager Instance;

        [Header("Inventory Panels")]
        [SerializeField] private GameObject inventoryHUD;
        [SerializeField] private GameObject inventoryHUDPanel;
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private TextMeshProUGUI inventoryText;
        [SerializeField] private List<ItemSlot> itemSlotList;
        private int nextIndexSlotAvailable = 0;
        [SerializeField] private GameObject looteableObjectPrefab;
        [SerializeField] private GameObject rightClickInterfacePrefab;
        
        private GameObject currentRightClickInterface;
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
            if (!inventoryHUD.activeSelf)
            {
                SoundManager.Instance.ActivateSoundByName(SoundAction.Inventory_OpenInventory);
            }
            else
            {
                SoundManager.Instance.ActivateSoundByName(SoundAction.Inventory_CloseInventory);
                TryDestroyContextMenu();
                
            }
              
            inventoryHUD.SetActive(!inventoryHUD.activeSelf);
            inventoryHUDPanel.SetActive(!inventoryHUDPanel.activeSelf);
            GameManager.Instance.GameState = inventoryHUD.activeSelf ? GameState.OnInventory: GameState.OnGame;
        }

        public void ActivateInventory()
        {
            GameManager.Instance.GameState = GameState.OnInventory;
            inventoryHUD.SetActive(true);
            inventoryHUDPanel.SetActive(true);
        }

        public void DesactivateInventory()
        {
            GameManager.Instance.GameState = GameState.OnGame;
            inventoryHUD.SetActive(false);
            TryDestroyContextMenu();
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
            int availableIndex = 0;
            remainingItemsWithoutSpace = 0;
            foreach (var itemSlot in itemSlotList)
            {
                if (itemSlot.itemID == item.itemID)
                {
                    int totalAmount = itemSlot.amount + amount;
                    if (totalAmount <= MAX_AMOUNT_PER_SLOT)
                    {
                        LogManager.Log("FIND EMPTY SLOT", FeatureType.Loot);
                        //ADD ELEMENT TO THIS SLOT
                        itemSlot.AddMoreItemsToSameSlot(amount);
                        return true;
                    }
                    else
                    {
                        if (itemSlot.amount != MAX_AMOUNT_PER_SLOT) //We dont want to check a full slot
                        {
                            //We refill one slot and fill other slots until we are completed
                            int amountToFill = MAX_AMOUNT_PER_SLOT - itemSlot.amount; //Space available
                            int amountRemaining = amount - amountToFill; //Remaining Items
                            itemSlot.AddMoreItemsToSameSlot(amountToFill);
                            if (amountRemaining > 0) //If there are items left to save
                            {
                                availableIndex = GetFirstIndexSlotAvailable();
                                if (availableIndex != -1)
                                {
                                    return SetRemainingItemsSlots(amountRemaining, availableIndex, item, out remainingItemsWithoutSpace);
                                }
                                else
                                {
                                    remainingItemsWithoutSpace = amountRemaining;
                                    return false;
                                }
                            }

                            return true;
                        }
                    }
                }
            }
            //NO SLOTS WITH THIS ITEM IN INVENTORY, WE CREATE A NEW ONE
            availableIndex = GetFirstIndexSlotAvailable();
            if (availableIndex != -1) //-1 => NO SLOT AVAILABLE
            {
                return SetRemainingItemsSlots(amount, availableIndex, item, out remainingItemsWithoutSpace);
            }

            remainingItemsWithoutSpace = amount;
            return false;
        }

        private bool SetRemainingItemsSlots(int amount, int availableIndex, Item item, out int remainingAmount)
        {
            if (amount >= MAX_AMOUNT_PER_SLOT)
            {
                itemSlotList[availableIndex].SetItemSlotProperties(item, MAX_AMOUNT_PER_SLOT);
                int remainingItemsAux = amount - MAX_AMOUNT_PER_SLOT;
                int remainingItemsInLoop = 0;
                while (remainingItemsAux > 0)
                {
                    int nextAmountToFill = GetNextAmountToFill(remainingItemsAux, out remainingItemsInLoop);
                    availableIndex = GetFirstIndexSlotAvailable();
                    if (availableIndex != -1)
                    {
                        itemSlotList[availableIndex]
                            .SetItemSlotProperties(item, nextAmountToFill);
                        remainingItemsAux = remainingItemsInLoop;
                    }
                    else
                    {
                        remainingAmount = remainingItemsInLoop;
                        return false;
                    }
                }
            }
            else
            {
                itemSlotList[availableIndex].SetItemSlotProperties(item, amount);
                remainingAmount = 0;
                return true;
            }
            remainingAmount = 0;
            return true; 
        }

        private int GetNextAmountToFill(int totalAmount, out int remainingItems)
        {
            // if > MAX -> itemToFill = MAX | remaining -= MAX
            // if < MAX -> itemToFill = remaining
            remainingItems = totalAmount;
            if (totalAmount >= MAX_AMOUNT_PER_SLOT)
            {
                remainingItems -= MAX_AMOUNT_PER_SLOT;
                return MAX_AMOUNT_PER_SLOT;
            }
            else
            {
                remainingItems = 0;
                return totalAmount;
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

        public int GetMaxItemsForSlots()
        {
            return MAX_AMOUNT_PER_SLOT;
        }

        public GameObject GetLooteableObjectPrefab()
        {
            return looteableObjectPrefab; 
        }

        public void ActivateContextMenuInterface(ItemSlot itemSlot)
        {
            
            if (currentRightClickInterface != null)
            {
                Destroy(currentRightClickInterface.gameObject);
            }
            GameObject rightClickInterface = Instantiate(rightClickInterfacePrefab, Input.mousePosition, Quaternion.identity);
            rightClickInterface.GetComponentInChildren<ContextMenu>().SetItemSlotProperties(itemSlot);
            //Adding offset
            Vector2 newPositon = new Vector2(Input.mousePosition.x + 80, Input.mousePosition.y + 80);
            rightClickInterface.GetComponentInChildren<Image>().rectTransform.anchoredPosition = newPositon;
            currentRightClickInterface = rightClickInterface;
        }

        public void TryDestroyContextMenu()
        {
            if (currentRightClickInterface != null)
            {
                Destroy(currentRightClickInterface);
                currentRightClickInterface = null; 
            }
        }

    }
}

