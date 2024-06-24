using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Loot;
using Player;
using UnityEngine;
using Utils.CustomLogs;

public class LootUIManager : MonoBehaviour
{
    public static LootUIManager Instance;
    
    
    [SerializeField] private GameObject lootUIPanel;
    private List<ItemSlot> itemsSlotsList;
    private LooteableObject currentCrateLooting;
    private bool getIfCrateIsOpened;
    private float distanceNeededToClosePanel = 2f;
    [SerializeField] private GameObject splittingView; 
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LootUIManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        itemsSlotsList = new List<ItemSlot>();
        itemsSlotsList = lootUIPanel.GetComponentsInChildren<ItemSlot>().ToList();
        lootUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (lootUIPanel.activeSelf)
        {
            //Check if we want to take all items pressing E Button
            if (Input.GetKeyDown(KeyCode.E))
            {
                LootAllItemsInCrate();
            }
        }
    }

    public void LootAllItemsInCrate()
    {
        currentCrateLooting.LootAllItems();
        DesactivateLootUIPanel();
        InventoryManager.Instance.DesactivateInventory();
        currentCrateLooting = null;
    }
    
    public void LoadItemsInSlots(Dictionary<Item, int> itemsInLootableObject)
    {
        foreach (var item in itemsInLootableObject)
        {
             int remainingItems = 0;
             int nextRemainingItems = 0;
             if (!itemsSlotsList[GetFirstIndexSlotAvailable()]
                 .TrySetItemSlotPropertiesForManager(item.Key, item.Value, out remainingItems))
             { 
                 itemsSlotsList[GetFirstIndexSlotAvailable()].TrySetItemSlotPropertiesForManager(item.Key,
                     remainingItems, out  nextRemainingItems);
                 while (nextRemainingItems > 0)
                 {
                     remainingItems = nextRemainingItems;
                     itemsSlotsList[GetFirstIndexSlotAvailable()].TrySetItemSlotPropertiesForManager(item.Key,
                         remainingItems, out nextRemainingItems);
                 }
             }
        }
    }

    public void UnloadItemsInSlots()
    {
        foreach (var itemSlot in itemsSlotsList)
        {
            itemSlot.ClearItemSlot();
        } 
    }
    
    
    public void ActivateLootUIPanel()
    {
        SoundManager.Instance.ActivateSoundByName(SoundAction.Inventory_OpenCrate);
        lootUIPanel.SetActive(true);
        getIfCrateIsOpened = true;
    }

    public void DesactivateLootUIPanel()
    {
        SoundManager.Instance.ActivateSoundByName(SoundAction.Inventory_CloseCrate);
        lootUIPanel.SetActive(false);
        getIfCrateIsOpened = false;
    }
    
    public void ActivateSplittingView(int maxAmount, DraggableItem draggableItem, ItemSlot itemSlot, ItemSlot previousItemSlot)
    {
        this.splittingView.SetActive(true);
        this.splittingView.GetComponent<SplittingView>().SetUpProperties(maxAmount, draggableItem, itemSlot, previousItemSlot);
    }
    public bool TryAddItemCrateToItemSlot(Item item, int amount, out int remainingItemsWithoutSpace)
        {
            int availableIndex = 0;
            int MAX_AMOUNT_PER_SLOT = InventoryManager.Instance.GetMaxItemsForSlots();
            remainingItemsWithoutSpace = 0;
            foreach (var itemSlot in itemsSlotsList)
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
                                    itemsSlotsList[availableIndex]
                                        .SetItemSlotProperties(item, amountRemaining); 
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
            //WE CREATE A NEW SLOT, IF AVAILABLE (NEED TO CHECK)
            availableIndex = GetFirstIndexSlotAvailable();
            if (availableIndex != -1)
            {
                itemsSlotsList[availableIndex].SetItemSlotProperties(item, amount);
                return true; 
            }
            else
            {
                remainingItemsWithoutSpace = amount;
                return false;
            }
        }
    private int GetFirstIndexSlotAvailable()
    {
        for (int i = 0; i < itemsSlotsList.Count; i++)
        {
            if (itemsSlotsList[i].itemID == 0)
            {
                return i;
            }
        }
        return -1;
    }
    
    public bool GetIfCrateIsOpened()
    {
        return getIfCrateIsOpened;
    }

    public LooteableObject GetCurrentLootableObject()
    {
        return currentCrateLooting;
    }

    public void SetCurrentCrateLooting(LooteableObject aux)
    {
        this.currentCrateLooting = aux;
    }
    
    public void SetPropertiesAndLoadPanel(LooteableObject looteableObject, Dictionary<Item, int> itemsInLootableObject)
    {
        LootUIManager.Instance.UnloadItemsInSlots();
        LootUIManager.Instance.SetCurrentCrateLooting(looteableObject);
        LootUIManager.Instance.LoadItemsInSlots(itemsInLootableObject);
        LootUIManager.Instance.ActivateLootUIPanel();
    }


}
