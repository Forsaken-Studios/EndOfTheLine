using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Loot;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils.CustomLogs;
using Object = System.Object;

namespace Inventory
{

    public class ItemSlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        
        [FormerlySerializedAs("comeFromLootCrate")] [SerializeField] private bool isLootCrate = false;
        private Sprite emptySprite;
        [SerializeField] private Image itemSlotImage;
        private TextMeshProUGUI itemSlotAmountText;
        private int ItemID;

        private bool canThrowItemAway;
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

        private void Update()
        {
            if (canThrowItemAway)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    Debug.Log("DESECHAR");
                    ThrowItemToGround();
                }
            }
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
            Debug.Log("NEW AMOUNT" + amount);
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
            //Por ahora tod el slot para probar
            int amountToMove = previousItemSlot.amount; 
            if (draggableItem.GetIfIsSplitting())
            {
                //We show slider
                LootUIManager.Instance.ActivateSplittingView(previousItemSlot.amount, draggableItem, this, previousItemSlot);
            }
            else
            {
                SwapItemsBetweenSlots(draggableItem, previousItemSlot, amountToMove);
            }
            
        }
        public void SwapItemsBetweenSlots(DraggableItem draggableItem, ItemSlot previousItemSlot,
            int amountToMove)
        {
            int totalAmount = previousItemSlot.amount;
            //Check to do, to add more amount to an item
            if (this.itemID == 0 && previousItemSlot.itemID != 0)
            {
                if (this.isLootCrate)
                {
                    if (!previousItemSlot.isLootCrate)
                    {
                        //Adding element to crate 
                        LogManager.Log("MOVING FROM INVENTORY TO CRATE", FeatureType.Loot);
                        MovingItemToOtherSlot(draggableItem, true, amountToMove, false);
                    }
                    else
                    {
                        //Moving element in crate to crate
                        LogManager.Log("MOVING FROM CRATE TO CRATE", FeatureType.Loot);
                        MoveItemInCrate(draggableItem, amountToMove); 
                    }
                }
                else
                {
                    if (!draggableItem.parentBeforeDrag.GetComponent<ItemSlot>().isLootCrate)
                    {

                        //Adding element to inventory
                        LogManager.Log("MOVING FROM INVENTORY TO INVENTORY", FeatureType.Loot);
                        MovingItemToOtherSlot(draggableItem, false, amountToMove, false);
                    }
                    else
                    {
                        //Adding element to inventory
                        LogManager.Log("MOVING FROM CRATE TO INVENTORY", FeatureType.Loot);
                        MovingItemToOtherSlot(draggableItem, false, amountToMove, true);
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
                            //TODO: Check when working 
                            DraggingItemToOtherItem(draggableItem, amountToMove); 
                        }
                    }
                    else
                    {
                        LogManager.Log("MOVING FROM INVENTORY TO CRATE (STACKING)", FeatureType.Loot);
                        //Moving from inventory to crate
                        AddItemFromInventoryToCrate(draggableItem, amountToMove);
                    }
   
                }
                else
                {
                    if (!this.isLootCrate)
                    {
                        //Moving from crate to inventory
                        LogManager.Log("MOVING FROM CRATE TO INVENTORY (STACKING)", FeatureType.Loot);
                        AddExistingItemToInventoryDragging(draggableItem, amountToMove);
                    }
                    else
                    {
                        //Moving from crate to crate (stacking)
                        LogManager.Log("MOVING FROM CRATE TO CRATE (STACKING)", FeatureType.Loot);
                        StackingItemsInCrateDragging(draggableItem, amountToMove);
                    }
                }
            }

            LooteableObject loot = LootUIManager.Instance.GetCurrentLootableObject();
            if (loot != null)
            {
                if (loot.GetIfItIsTemporalBox() && loot.CheckIfLootBoxIsEmpty())
                {
                    Destroy(loot.gameObject);
                    Debug.Log("TEMPORAL BOX DESTROYED");
                }
            }
        }

        private void MoveItemInCrate(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            if (amountToMove == itemSlotBeforeDrop.amount)
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            else
                itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
            
            this.SetItemSlotProperties(itemToAdd, amountToMove);
        }

        private void DraggingItemToOtherItem(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            if (amountToMove == itemSlotBeforeDrop.amount)
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            else
                itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
            PlayerInventory.Instance.RemovingItem(itemToAdd, amountToMove);
            if (InventoryManager.Instance.TryAddInventoryToItemSlot(itemToAdd, 
                    amountToMove, out remainingItems))
            {
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), amountToMove, false);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
        }
        
        private void AddItemFromInventoryToCrate(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            Debug.Log("MOVING FROM METHOD: " + amountToMove);
            if (LootUIManager.Instance.TryAddItemCrateToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), 
                    amountToMove, out remainingItems))
            {
                if (amountToMove == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
                
                draggableItem.SetItemComingFromInventoryToCrate(true);
                if (!itemSlotBeforeDrop.GetIfIsLootCrate() && this.GetIfIsLootCrate())
                {
                    LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToAdd, 
                        amountToMove);
                    PlayerInventory.Instance.RemovingItem(itemToAdd, amountToMove);
                }
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemSlotBeforeDrop.GetItemInSlot(), 
                   amountToMove - remainingItems);
                PlayerInventory.Instance.RemovingItem(itemToAdd, amountToMove - remainingItems);
            }
        }
        
        private void AddExistingItemToInventoryDragging(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;

            if (InventoryManager.Instance.TryAddInventoryToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), amountToMove,
                    out remainingItems))
            {
                //We have size, we take all items of this type
                if (amountToMove == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), amountToMove, true);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), amountToMove - remainingItems, true);
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
            }
            
            LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToAdd,
                amountToMove);
        }
        
        private void StackingItemsInCrateDragging(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            if (amountToMove == itemSlotBeforeDrop.amount)
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            else
                itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
            LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToAdd,
                amountToMove);
            if (LootUIManager.Instance.TryAddItemCrateToItemSlot(itemToAdd, 
                    amountToMove, out remainingItems))
            {
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToAdd, amountToMove);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            } 
        }

        private void MovingItemToOtherSlot(DraggableItem draggableItem, bool fromInventoryToCrate, int amountToMove, bool showMessage)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), amountToMove);
            if (fromInventoryToCrate)
            {
                draggableItem.SetItemComingFromInventoryToCrate(true); 
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(this.GetItemInSlot(), 
                    amountToMove);
                PlayerInventory.Instance.RemovingItem(this.GetItemInSlot(), amountToMove);
            }
            else
            {
                if (itemSlotBeforeDrop.GetIfIsLootCrate() && !this.GetIfIsLootCrate())
                {
                    PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), amountToMove, 
                        showMessage);
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemSlotBeforeDrop.GetItemInSlot(),
                        amountToMove);
                    draggableItem.SetItemComingFromInventoryToCrate(false); 
                }
            }

            if (amountToMove == itemSlotBeforeDrop.amount)
                ResetItemSlot(itemSlotBeforeDrop, draggableItem);
            else
                itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);

            draggableItem.parentAfterDrag = this.transform;
            itemSlotImage.gameObject.transform.position = draggableItem.parentAfterDrag.position;
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

    
        public void ThrowItemToGround()
        {

             //We will need to check if it crate or not
             if (this.isLootCrate)
             {
                 //We throw item out of loot box
                 LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(GetItemInSlot(), amount);
             }
             else
             {
                 //We throw item out of inventory
                 PlayerInventory.Instance.RemovingItem(GetItemInSlot(), amount);
             }
             //We create new box
             GameObject looteableObject = Instantiate(InventoryManager.Instance.GetLooteableObjectPrefab(),
                 PlayerInventory.Instance.transform.position, Quaternion.identity);
             LooteableObject lootObject = looteableObject.GetComponent<LooteableObject>();
             
             lootObject.SetIfItIsTemporalBox(true);
             lootObject.ClearLooteableObject();
             lootObject.AddItemToList(GetItemInSlot(), amount);
             ClearItemSlot();
             
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            //Doble click eventData.clickCount == 2
            
            if (Input.GetKey(KeyCode.LeftShift)) {
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

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (itemID != 0)
                {
                    InventoryManager.Instance.ActivateContextMenuInterface(this);
                }
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.itemID != 0)
            {
                canThrowItemAway = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.itemID != 0)
            {
                canThrowItemAway = false;
            }
        }
    }
}