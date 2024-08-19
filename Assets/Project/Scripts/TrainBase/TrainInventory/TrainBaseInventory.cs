using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainBaseInventory : MonoBehaviour
{
    
    public static TrainBaseInventory Instance;

    private int numberOfTools = -1;
    private Dictionary<Item, int> itemsInBase;
    [SerializeField] private List<ItemSlot> itemsSlotsList;
    [SerializeField] private GameObject splittingView;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TrainBaseInventory! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        itemsInBase = new Dictionary<Item, int>();

    }

    
    public int GetNumberOfToolsInInventory()
    {
        if (numberOfTools == -1)
        {
            numberOfTools = 0;
            foreach (var itemSlot in itemsSlotsList)
            {
                if (itemSlot.GetItemInSlot() != null)
                {
                    if (itemSlot.GetItemInSlot().itemID == 8)
                    {
                        numberOfTools += itemSlot.amount;
                    }
                }
               
            }
            return numberOfTools;
        }
        
        return numberOfTools;
    }
    
    public void ActivateSplittingView(int maxAmount, DraggableItem draggableItem, ItemSlot itemSlot, ItemSlot previousItemSlot)
    {
        this.splittingView.SetActive(true);
        this.splittingView.GetComponent<SplittingView>().SetUpProperties(maxAmount, draggableItem, itemSlot, previousItemSlot);
    }
    public bool TryAddItemCrateToItemSlot(Item item, int amount, out int remainingItemsWithoutSpace)
    {
        int availableIndex = 0;
        int MAX_AMOUNT_PER_SLOT = 0;
        if (SceneManager.GetActiveScene().name == "TrainBase")
           MAX_AMOUNT_PER_SLOT = TrainInventoryManager.Instance.GetMaxItemsForSlots();
        else
            MAX_AMOUNT_PER_SLOT = InventoryManager.Instance.GetMaxItemsForSlots();
    
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


    public void RemoveItemFromItemSlot(int itemSlotIndex, int amount)
    {
        if (itemsSlotsList[itemSlotIndex].amount == amount)
        {
            Debug.Log("HEMOS QUITADO EL ITEM ENTERO: INDEX: " + itemSlotIndex);
            itemsSlotsList[itemSlotIndex].ClearItemSlot();
        }
        else
        {
            itemsSlotsList[itemSlotIndex].amount -= amount;
        }
    }
    
    
    
    public bool TryCheckIfThereIsSpaceForAllItems(Dictionary<Item, int> items, out Dictionary<int, int> slotsUsed)
    {
        //itemSlot used, amount
        slotsUsed = new Dictionary<int, int>();
        
    int availableIndex = 0;
    int MAX_AMOUNT_PER_SLOT = 0;
    if (SceneManager.GetActiveScene().name == "TrainBase")
       MAX_AMOUNT_PER_SLOT = TrainInventoryManager.Instance.GetMaxItemsForSlots();
    else
        MAX_AMOUNT_PER_SLOT = InventoryManager.Instance.GetMaxItemsForSlots();
    
    foreach (var item in items)
    {

        foreach (var itemSlot in itemsSlotsList)
        {
            if (itemSlot.itemID == item.Key.itemID)
            {
                int totalAmount = itemSlot.amount + item.Value;
                if (totalAmount <= MAX_AMOUNT_PER_SLOT)
                {
                    //ADD ELEMENT TO THIS SLOT
                    itemSlot.AddMoreItemsToSameSlot(item.Value);
                    if (slotsUsed.ContainsKey(itemsSlotsList.IndexOf(itemSlot)))
                    {
                        slotsUsed[itemsSlotsList.IndexOf(itemSlot)] = item.Value;
                    }
                    else
                    {
                        slotsUsed.Add(itemsSlotsList.IndexOf(itemSlot), item.Value);
                    }

                }
                else
                {
                    //We refill one slot and create another one
                    if (itemSlot.amount != MAX_AMOUNT_PER_SLOT) //We dont check a full slot
                    {
                        int amountToFill = MAX_AMOUNT_PER_SLOT - itemSlot.amount;
                        int amountRemaining = item.Value - amountToFill;
                        itemSlot.AddMoreItemsToSameSlot(amountToFill);
                        
                        if (slotsUsed.ContainsKey(itemsSlotsList.IndexOf(itemSlot)))
                        {
                            slotsUsed[itemsSlotsList.IndexOf(itemSlot)] = amountToFill;
                        }
                        else
                        {
                            slotsUsed.Add(itemsSlotsList.IndexOf(itemSlot), amountToFill);
                        }
                        if (amountRemaining > 0)
                        {
                            availableIndex = GetFirstIndexSlotAvailable();
                            if (availableIndex != -1)
                            {
                                itemsSlotsList[availableIndex]
                                    .SetItemSlotProperties(item.Key, amountRemaining); 
                                slotsUsed.Add(availableIndex, amountRemaining);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        //WE CREATE A NEW SLOT, IF AVAILABLE (NEED TO CHECK)
        availableIndex = GetFirstIndexSlotAvailable();
        if (availableIndex != -1)
        {
            itemsSlotsList[availableIndex].SetItemSlotProperties(item.Key, item.Value);
            slotsUsed.Add(availableIndex, item.Value);
        }
        else
        {
            return false;
        }
    }
    return true;
    }

    public void DeleteItemsFromItemSlot(Item item, int amount)
    {
        int remainingAmount = amount; 
        foreach (var itemSlot in itemsSlotsList)
        {
            if (itemSlot.GetItemInSlot() != null)
            {
                if (itemSlot.GetItemInSlot().itemID == item.itemID)
                {
                    if (itemSlot.amount > remainingAmount)
                    {
                        itemSlot.amount -= amount;
                        break;
                    }
                    else
                    {
                        remainingAmount -= itemSlot.amount;
                        itemSlot.ClearItemSlot();
                    }
                } 
            }
          
        }
    }

    
    public bool GetIfItemIsInInventory(Item item, int amount)
    {
        int amountAux = -1;
        itemsInBase.TryGetValue(item, out amountAux);
        return amountAux >= amount;
    }

    public void AddItemInXSlot(int itemSlotIndex, Item item, int value)
    {
        itemsSlotsList[itemSlotIndex].SetItemSlotProperties(item, value);
    }
    
    
    public void AddItemToList(Item item, int amount)
    {
        if (itemsInBase.ContainsKey(item))
        {
            itemsInBase[item] += amount;
        }
        else
        {
            itemsInBase.Add(item, amount);
        }
    }   
    
    public void DeleteItemFromList(Item item, int amount)
    {
        if (itemsInBase[item] > amount)
        {
            itemsInBase[item] -= amount; 
        }
        else
        {
            itemsInBase.Remove(item); 
        }
    }
    
    public Dictionary<int, ItemInBaseDataSave> GetBaseInventoryToSave()
    {
        Dictionary<int, ItemInBaseDataSave> itemList = new Dictionary<int, ItemInBaseDataSave>();

        foreach (var itemSlot in itemsSlotsList)
        {
            ItemInBaseDataSave item = new ItemInBaseDataSave(itemSlot.itemID, itemSlot.amount);
            itemList.Add(itemsSlotsList.IndexOf(itemSlot), item);
        }
        return itemList;
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
}
