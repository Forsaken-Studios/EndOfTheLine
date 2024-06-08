using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils.CustomLogs;

namespace Inventory
{

    public class ItemSlot : MonoBehaviour, IDropHandler
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
            this.itemSlotAmountText.text = "x" + itemSlotAmount.ToString();
            this.amount = itemSlotAmount;
            
        }
        
        public bool TrySetItemSlotPropertiesForManager(Item item, int itemSlotAmount, out int remaningItems)
        {
            int MAX_ITEMS_SLOT = InventoryManager.Instance.GetMaxItemsForSlots();
            if (itemSlotAmount > MAX_ITEMS_SLOT)
            {
                SetItemSlotProperties(item, MAX_ITEMS_SLOT);
                remaningItems = itemSlotAmount - MAX_ITEMS_SLOT;
                return false; 
            }
            SetItemSlotProperties(item, itemSlotAmount);
            remaningItems = 0; 
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
            //Check to do, to add more amount to an item
            if (this.itemID == 0)
            {
                if (this.isLootCrate)
                {
                    //Adding element to crate 
                    LogManager.Log("MOVING FROM INVENTORY TO CRATE", FeatureType.Loot);
                    AddNewItemToInventoryDragging(draggableItem, true);
                }
                else
                {
                    //Adding element to inventory
                    LogManager.Log("MOVING FROM CRATE TO INVENTORY", FeatureType.Loot);
                    AddNewItemToInventoryDragging(draggableItem, false);
                }
               
            }else if (this.itemID == draggableItem.parentBeforeDrag.GetComponent<ItemSlot>().GetItemInSlot().itemID)
            {
                if (!draggableItem.parentBeforeDrag.GetComponent<ItemSlot>().isLootCrate)
                {
                    if (!this.isLootCrate)
                    {
                        LogManager.Log("MOVING FROM INVENTORY TO INVENTORY (STACKING)", FeatureType.Loot);
                        //Moving from inventory to inventory 
                        DraggingItemToOtherItem(draggableItem);
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

        private void DraggingItemToOtherItem(DraggableItem draggableItem)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            PlayerInventory.Instance.RemovingItemDragging(itemToAdd, auxAmount);
            if (InventoryManager.Instance.TryAddInventoryToItemSlot(itemToAdd, 
                    auxAmount, out remainingItems))
            {
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), auxAmount);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
        }
        
        private void AddItemFromInventoryToCrate(DraggableItem draggableItem)
        {
            //Its not coming from crate, we are trying to move from inventory to crate
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            int auxAmount = itemSlotBeforeDrop.amount;
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;

            if (LootUIManager.Instance.TryAddItemCrateToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), 
                    auxAmount, out remainingItems))
            {
                //We have size, we take all items of this type
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                //Necesitamos poner algo para que entre en el if de draggable item
                draggableItem.SetItemComingFromInventoryToCrate(true);
                if (!itemSlotBeforeDrop.GetIfIsLootCrate() && this.GetIfIsLootCrate())
                {
                    PlayerInventory.Instance.RemovingItemDragging(this.GetItemInSlot(), auxAmount);
                    LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToAdd, 
                        auxAmount);
                    PlayerInventory.Instance.RemovingItemDragging(itemToAdd, auxAmount);
                }
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemSlotBeforeDrop.GetItemInSlot(), 
                   auxAmount - remainingItems);
                PlayerInventory.Instance.RemovingItemDragging(itemToAdd, auxAmount - remainingItems);
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
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), auxAmount);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                //TODO: No size for all items, we need to let X items in crate
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), auxAmount - remainingItems);
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

        private void AddNewItemToInventoryDragging(DraggableItem draggableItem, bool fromInventoryToCrate)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), itemSlotBeforeDrop.amount);
            if (fromInventoryToCrate)
            {
                draggableItem.SetItemComingFromInventoryToCrate(true); 
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(this.GetItemInSlot(), 
                    itemSlotBeforeDrop.amount);
                PlayerInventory.Instance.RemovingItemDragging(this.GetItemInSlot(), itemSlotBeforeDrop.amount);
            }
            else
            {
                if (itemSlotBeforeDrop.GetIfIsLootCrate() && !this.GetIfIsLootCrate())
                {
                    PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), itemSlotBeforeDrop.amount);
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemSlotBeforeDrop.GetItemInSlot(),
                        itemSlotBeforeDrop.amount);
                    draggableItem.SetItemComingFromInventoryToCrate(false); 
                }
            }
            ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            draggableItem.parentAfterDrag = this.transform;
            itemSlotImage.gameObject.transform.position = draggableItem.parentAfterDrag.position;
           
            
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