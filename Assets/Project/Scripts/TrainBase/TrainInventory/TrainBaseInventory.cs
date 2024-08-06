using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class TrainBaseInventory : MonoBehaviour
{
    
    public static TrainBaseInventory Instance;
        
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
}
