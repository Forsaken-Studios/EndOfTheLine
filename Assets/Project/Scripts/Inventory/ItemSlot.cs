using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils.CustomLogs;

namespace Inventory
{

    public class ItemSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        
        [FormerlySerializedAs("comeFromLootCrate")] [SerializeField] private bool isLootCrate = false;
        private Sprite emptySprite;
        [SerializeField] private Image itemSlotImage;
        private TextMeshProUGUI itemSlotAmountText;
        private int ItemID;
        private Item itemInSlot;
        public int itemID
        {
            get { return ItemID; }
            set { this.ItemID = value; }
        }

        private int Amount;

        public int amount
        {
            get { return Amount; }
            set { this.Amount = value; }
        }

        private void Awake()
        {
            this.itemSlotAmountText = this.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            this.itemSlotAmountText.text = "";
             emptySprite = (Sprite) UnityEngine.Resources.Load<Sprite>("Sprites/EmptySprite");
        }

        /// <summary>
        /// Initialize item slot
        /// </summary>
        /// <param name="itemImage"></param>
        /// <param name="itemSlotAmount"></param>
        /// <param name="itemID"></param>
        public void SetItemSlotProperties(Item item, int itemSlotAmount)
        {
            this.itemInSlot = item;
            this.itemSlotImage.sprite = item.itemIcon;
            this.itemID = item.itemID;
            this.itemSlotAmountText.text = itemSlotAmount == 1 ? "" : "x" + itemSlotAmount.ToString();
            this.amount = itemSlotAmount;
            
        }
  
        public bool TrySetItemSlotPropertiesForManager(Item item, int itemSlotAmount, out int remainingItems)
        {
            int MAX_ITEMS_SLOT = InventoryManager.Instance.GetMaxItemsForSlots();
            if (itemSlotAmount > MAX_ITEMS_SLOT)
            {
                SetItemSlotProperties(item, MAX_ITEMS_SLOT);
                remainingItems = itemSlotAmount - MAX_ITEMS_SLOT;
                return false; 
            }
            SetItemSlotProperties(item, itemSlotAmount);
            remainingItems = 0; 
            return true;
            
        }

        /// <summary>
        /// Clear Item Slot properties
        /// </summary>
        public void ClearItemSlot()
        {
            this.itemInSlot = null;
            this.itemSlotImage.sprite = emptySprite;
            this.itemID = 0;
            this.itemSlotAmountText.text = "";
            this.amount = 0; 
        }

        public void ModifyItemSlotAmount(int amount)
        {
            this.amount = amount; 
            this.itemSlotAmountText.text = amount == 1 ? "" : "x" + amount.ToString();
        }
        
        /// <summary>
        /// Add items to a used slot.
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoreItemsToSameSlot(int amount)
        {
            this.amount += amount;
            this.itemSlotAmountText.text = "x" + this.amount;
        }


        /// <summary>
        /// Method to change itemSlotImage reference.
        /// </summary>
        /// <param name="newImage"></param>
        public void ChangeImage(GameObject newImage)
        {
            newImage.transform.SetParent(this.gameObject.transform);
            this.itemSlotImage = newImage.GetComponent<Image>();
        }


        /// <summary>
        /// Method when we drop an item in a itemSlot
        /// We swaps images, amounts and itemIDs. 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            ItemSlot previousItemSlot = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            SwapItemsBetweenSlots(draggableItem, previousItemSlot);
        }


        private void SwapItemsBetweenSlots(DraggableItem draggableItem, ItemSlot previousItemSlot)
        {
            //Check to do, to add more amount to an item
            if (this.itemID == 0 && previousItemSlot.itemID != 0)
            {
                if (this.isLootCrate)
                {
                    if (!previousItemSlot.isLootCrate)
                    {
                        //Adding element to crate 
                        LogManager.Log("MOVING FROM INVENTORY TO CRATE", FeatureType.Loot);
                        MovingItemToOtherSlot(draggableItem, true, false);
                    }
                    else
                    {
                        //Moving element in crate to crate
                        LogManager.Log("MOVING FROM CRATE TO CRATE", FeatureType.Loot);
                        MoveItemInCrate(draggableItem); 
                    }
                }
                else
                {
                    if (!draggableItem.parentBeforeDrag.GetComponent<ItemSlot>().isLootCrate)
                    {

                        //Adding element to inventory
                        LogManager.Log("MOVING FROM INVENTORY TO INVENTORY", FeatureType.Loot);
                        MovingItemToOtherSlot(draggableItem, false, false);
                    }
                    else
                    {
                        //Adding element to inventory
                        LogManager.Log("MOVING FROM CRATE TO INVENTORY", FeatureType.Loot);
                        MovingItemToOtherSlot(draggableItem, false, true);
                    }

                }
               
            }else if (previousItemSlot.itemID != 0 && this.itemID == previousItemSlot.itemID)
            {
                if (!previousItemSlot.isLootCrate)
                {
                    if (!this.isLootCrate)
                    {
                        if (draggableItem.parentBeforeDrag.GetComponent<ItemSlot>() != this)
                        {
                            LogManager.Log("MOVING FROM INVENTORY TO INVENTORY (STACKING)", FeatureType.Loot);
                            //Moving from inventory to inventory 
                            DraggingItemToOtherItem(draggableItem); 
                        }
                    }
                    else
                    {
                        LogManager.Log("MOVING FROM INVENTORY TO CRATE (STACKING)", FeatureType.Loot);
                        //Moving from inventory to crate
                        AddItemFromInventoryToCrate(draggableItem);
                    }
   
                }
                else
                {
                    if (!this.isLootCrate)
                    {
                        //Moving from crate to inventory
                        LogManager.Log("MOVING FROM CRATE TO INVENTORY (STACKING)", FeatureType.Loot);
                        AddExistingItemToInventoryDragging(draggableItem);
                    }
                    else
                    {
                        //Moving from crate to crate (stacking)
                        LogManager.Log("MOVING FROM CRATE TO CRATE (STACKING)", FeatureType.Loot);
                        StackingItemsInCrateDragging(draggableItem);
                    }
   
                }
            }
        }

        private void MoveItemInCrate(DraggableItem draggableItem)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            this.SetItemSlotProperties(itemToAdd, auxAmount);
        }

        private void DraggingItemToOtherItem(DraggableItem draggableItem)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            PlayerInventory.Instance.RemovingItem(itemToAdd, auxAmount);
            if (InventoryManager.Instance.TryAddInventoryToItemSlot(itemToAdd, 
                    auxAmount, out remainingItems))
            {
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), auxAmount, false);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
        }
        
        private void AddItemFromInventoryToCrate(DraggableItem draggableItem)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;

            if (LootUIManager.Instance.TryAddItemCrateToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), 
                    auxAmount, out remainingItems))
            {
                //We have size, we take all items of this type
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                draggableItem.SetItemComingFromInventoryToCrate(true);
                if (!itemSlotBeforeDrop.GetIfIsLootCrate() && this.GetIfIsLootCrate())
                {
                    PlayerInventory.Instance.RemovingItem(this.GetItemInSlot(), auxAmount);
                    LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToAdd, 
                        auxAmount);
                    PlayerInventory.Instance.RemovingItem(itemToAdd, auxAmount);
                }
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemSlotBeforeDrop.GetItemInSlot(), 
                   auxAmount - remainingItems);
                PlayerInventory.Instance.RemovingItem(itemToAdd, auxAmount - remainingItems);
            }
        }
        
        private void AddExistingItemToInventoryDragging(DraggableItem draggableItem)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;

            if (InventoryManager.Instance.TryAddInventoryToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), auxAmount,
                    out remainingItems))
            {
                //We have size, we take all items of this type
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), auxAmount, true);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), auxAmount - remainingItems, true);
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
            }
            
            LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToAdd,
                auxAmount);
        }
        
        private void StackingItemsInCrateDragging(DraggableItem draggableItem)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToAdd,
                auxAmount);
            if (LootUIManager.Instance.TryAddItemCrateToItemSlot(itemToAdd, 
                    auxAmount, out remainingItems))
            {
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToAdd, auxAmount);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            } 
        }

        private void MovingItemToOtherSlot(DraggableItem draggableItem, bool fromInventoryToCrate, bool showMessage)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), itemSlotBeforeDrop.amount);
            if (fromInventoryToCrate)
            {
                draggableItem.SetItemComingFromInventoryToCrate(true); 
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(this.GetItemInSlot(), 
                    itemSlotBeforeDrop.amount);
                PlayerInventory.Instance.RemovingItem(this.GetItemInSlot(), itemSlotBeforeDrop.amount);
            }
            else
            {
                if (itemSlotBeforeDrop.GetIfIsLootCrate() && !this.GetIfIsLootCrate())
                {
                    PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), itemSlotBeforeDrop.amount, 
                        showMessage);
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemSlotBeforeDrop.GetItemInSlot(),
                        itemSlotBeforeDrop.amount);
                    draggableItem.SetItemComingFromInventoryToCrate(false); 
                }
            }
            ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            draggableItem.parentAfterDrag = this.transform;
            itemSlotImage.gameObject.transform.position = draggableItem.parentAfterDrag.position;
        }    
      
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2) {
                if (ValidMovementFromInventoryToCrate())
                {
                    //MOVING FROM INVENTORY TO CRATE
                   // LogManager.Log("[DOUBLE CLICK] MOVING ITEM FROM INVENTORY TO CRATE", FeatureType.Loot);
                    DoubleClickOnItemFromInventoryToCrate();

                }
                else if(ValidMovementFromCrateToInventory())
                {
                    //MOVING FROM CRATE TO INVENTORY
                   // LogManager.Log("[DOUBLE CLICK] MOVING ITEM FROM CRATE TO INVENTORY", FeatureType.Loot);
                    DoubleClickOnItemFromCrateToInventory();
                }
            }
        }

        private void DoubleClickOnItemFromInventoryToCrate()
        {
            int remainingItemsWithoutSpace = 0;
            Item itemToLoot = this.itemInSlot;
            int amountToLoot = this.amount; 
            LootUIManager.Instance.TryAddItemCrateToItemSlot(itemToLoot, amountToLoot, out remainingItemsWithoutSpace);
            Debug.Log("REMAINING ITEMS: " + remainingItemsWithoutSpace);
            if (remainingItemsWithoutSpace > 0)
            {
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToLoot, 
                    amountToLoot - remainingItemsWithoutSpace);
                PlayerInventory.Instance.RemovingItem(itemToLoot, amountToLoot - remainingItemsWithoutSpace);
                ModifyItemSlotAmount(remainingItemsWithoutSpace);
            }
            else
            {
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToLoot, amountToLoot);
                PlayerInventory.Instance.RemovingItem(itemToLoot, amountToLoot);
                this.ClearItemSlot();
            }
        }

        private void DoubleClickOnItemFromCrateToInventory()
        {
            int remainingItemsWithoutSpace = 0;
            Item itemToLoot = this.itemInSlot; 
            PlayerInventory.Instance.TryAddItem(itemToLoot, this.amount, out remainingItemsWithoutSpace, true); 
            if (remainingItemsWithoutSpace > 0)
            {
                //We return remainingItems To crate
                LootUIManager.Instance.TryAddItemCrateToItemSlot(itemToLoot, 
                    remainingItemsWithoutSpace, out int remainingItems); 
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToLoot, remainingItemsWithoutSpace);
                //And update item slot
                ModifyItemSlotAmount(remainingItemsWithoutSpace);
            }
            else
            {
                LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToLoot, this.amount);
                this.ClearItemSlot();
            }
        }

        private bool ValidMovementFromInventoryToCrate()
        {
            return this.itemID != 0 && LootUIManager.Instance.GetIfCrateIsOpened() && !this.GetIfIsLootCrate();
        }
        
        private bool ValidMovementFromCrateToInventory()
        {
            return this.itemID != 0 && LootUIManager.Instance.GetIfCrateIsOpened() && this.GetIfIsLootCrate();
        }
        
        private void ResetItemSlot(ItemSlot itemSlot, DraggableItem draggableItem)
        {
            itemSlot.itemInSlot = null;
            itemSlot.amount = 0;
            itemSlot.itemSlotAmountText.text = "";
            itemSlot.itemID = 0;
            itemSlot.itemSlotImage.sprite = emptySprite;
            itemSlot.itemSlotImage.gameObject.transform.SetParent(draggableItem.parentBeforeDrag);
            itemSlot.itemSlotImage.gameObject.transform.position = draggableItem.parentBeforeDrag.position;
        }
        
        public Item GetItemInSlot()
        {
            return itemInSlot;
        }

        public bool GetIfIsLootCrate()
        {
            return isLootCrate;
        }

        public void ChangeSpriteImage(Sprite image)
        {
            this.itemSlotImage.sprite = image;
        }
    }
}