
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LootSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils.CustomLogs;

namespace Inventory
{

    public class ItemSlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        
        [FormerlySerializedAs("comeFromLootCrate")] [SerializeField] private bool isLootCrate = false;
        private Sprite emptySprite;
        [SerializeField] private Image itemSlotImage;
        private TextMeshProUGUI itemSlotAmountText;
        private int ItemID;
        private GameObject hoverGameObject;

        [SerializeField] private GameObject blackPanel;
        [SerializeField] private GameObject searchPanel;
        private bool canThrowItemAway;
        private Item itemInSlot;
        private GameObject hoverInfo;
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
  
        public bool TrySetItemSlotPropertiesForManagerLootUI(Item item, int itemSlotAmount, out int remainingItems)
        {
            int MAX_ITEMS_SLOT = GameManager.Instance.GetMaxAmountPerSlot();
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
            if(itemSlotImage != null)
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
            //Por ahora tod el slot para probar
            int amountToMove = previousItemSlot.amount; 
            if (draggableItem.GetIfIsSplitting())
            {
                //We show slider
                if (this.itemID == previousItemSlot.itemID || this.itemID == 0)
                {
                    if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                        TrainBaseInventory.Instance.ActivateSplittingView(previousItemSlot.amount, draggableItem, this, previousItemSlot);
                    else
                        LootUIManager.Instance.ActivateSplittingView(previousItemSlot.amount, draggableItem, this, previousItemSlot);
                }
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
                            DraggingItemToOtherItem(draggableItem, amountToMove); 
                        }
                    }
                    else
                    {
                        LogManager.Log("MOVING FROM INVENTORY TO CRATE (STACKING)", FeatureType.Loot);
                        //Moving from inventory to crate
                        AddItemFromInventoryToCrateStacking(draggableItem, amountToMove);
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
            if (SceneManager.GetActiveScene().name != GameManager.Instance.GetNameTrainScene())
            {
                CheckIfWeNeedToHideHUD();
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

        /// <summary>
        /// Stacking items from inventory to inventory
        /// </summary>
        /// <param name="draggableItem"></param>
        /// <param name="amountToMove"></param>
        private void DraggingItemToOtherItem(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;

            if (this.amount + amountToMove <= GameManager.Instance.GetMaxAmountPerSlot())
            {
                //Just move 
                this.ModifyItemSlotAmount(this.amount + amountToMove);
                if (amountToMove == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
            }
            else
            {
                int valueRemainingInPreviousSlot = (this.amount + itemSlotBeforeDrop.amount) - GameManager.Instance.GetMaxAmountPerSlot();
                this.ModifyItemSlotAmount(GameManager.Instance.GetMaxAmountPerSlot());
                itemSlotBeforeDrop.ModifyItemSlotAmount(valueRemainingInPreviousSlot);
            }
            
        }
        
        /// <summary>
        /// Stacking items from inventory to crate
        /// </summary>
        /// <param name="draggableItem"></param>
        /// <param name="amountToMove"></param>
        private void AddItemFromInventoryToCrateStacking(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            
            if (this.amount + amountToMove <= GameManager.Instance.GetMaxAmountPerSlot())
            {
                //Just move 
                this.ModifyItemSlotAmount(this.amount + amountToMove);
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.AddItemToList(itemInSlot, amountToMove);
                else
                    LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemInSlot, amountToMove);
                PlayerInventory.Instance.RemovingItem(itemToAdd, amountToMove);
                itemSlotBeforeDrop.ClearItemSlot();
            }else if(this.amount == GameManager.Instance.GetMaxAmountPerSlot())
            {
                StackItemsFromInventoryToCrate(itemSlotBeforeDrop, amountToMove, draggableItem, itemToAdd);
            }
            else
            {
                int remainingSpace = (this.amount + amountToMove) - GameManager.Instance.GetMaxAmountPerSlot();
                int valueToMove = GameManager.Instance.GetMaxAmountPerSlot() - this.amount;
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.AddItemToList(itemInSlot, valueToMove);
                else
                    LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemInSlot, valueToMove);
                PlayerInventory.Instance.RemovingItem(itemToAdd, valueToMove);
                itemSlotBeforeDrop.ModifyItemSlotAmount(remainingSpace);
                this.ModifyItemSlotAmount(GameManager.Instance.GetMaxAmountPerSlot());

                    
                StackItemsFromInventoryToCrate(itemSlotBeforeDrop, remainingSpace, draggableItem, itemToAdd);

            }
        }
        /// <summary>
        /// Aux method from inventory to cache
        /// </summary>
        /// <param name="itemSlotBeforeDrop"></param>
        /// <param name="remainingSpace"></param>
        /// <param name="draggableItem"></param>
        /// <param name="itemToAdd"></param>
        private void StackItemsFromInventoryToCrate(ItemSlot itemSlotBeforeDrop, int remainingSpace,
            DraggableItem draggableItem, Item itemToAdd)
        {
            if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
            {
                StackItemsInBaseFromInventoryToCrate(itemSlotBeforeDrop, remainingSpace, draggableItem, itemToAdd);
            }
            else
            {
                StackItemsInGameFromInventoryToCrate(itemSlotBeforeDrop, remainingSpace, draggableItem, itemToAdd);
            }
            
        }

        private void StackItemsInBaseFromInventoryToCrate(ItemSlot itemSlotBeforeDrop, int remainingSpace,
            DraggableItem draggableItem, Item itemToAdd)
        {
            if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), 
                    remainingSpace, out int remainingItems))
            {
                if (remainingSpace == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - remainingSpace);
                draggableItem.SetItemComingFromInventoryToCrate(true);
                TrainBaseInventory.Instance.AddItemToList(itemSlotBeforeDrop.GetItemInSlot(), remainingSpace);
                PlayerInventory.Instance.RemovingItem(itemToAdd, remainingSpace);
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
                TrainBaseInventory.Instance.AddItemToList(itemSlotBeforeDrop.GetItemInSlot(), remainingSpace - remainingItems);
                PlayerInventory.Instance.RemovingItem(itemToAdd, remainingSpace - remainingItems);
            }
        }

        private void StackItemsInGameFromInventoryToCrate(ItemSlot itemSlotBeforeDrop, int remainingSpace,
            DraggableItem draggableItem, Item itemToAdd)
        {
            if (LootUIManager.Instance.TryAddItemCrateToItemSlot(itemSlotBeforeDrop.GetItemInSlot(), 
                    remainingSpace, out int remainingItems))
            {
                if (remainingSpace == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - remainingSpace);
                draggableItem.SetItemComingFromInventoryToCrate(true);
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToAdd, 
                    remainingSpace);
                PlayerInventory.Instance.RemovingItem(itemToAdd, remainingSpace);
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
                LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemSlotBeforeDrop.GetItemInSlot(),
                    remainingSpace - remainingItems);
                PlayerInventory.Instance.RemovingItem(itemToAdd, remainingSpace - remainingItems);
            }
        }

        /// <summary>
        /// Stacking items from crate into inventory
        /// </summary>
        /// <param name="draggableItem"></param>
        /// <param name="amountToMove"></param>
        private void AddExistingItemToInventoryDragging(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;
            
            if (this.amount + amountToMove <= GameManager.Instance.GetMaxAmountPerSlot())
            {
                //Just move 
                this.ModifyItemSlotAmount(this.amount + amountToMove);
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.DeleteItemFromList(itemInSlot, amountToMove);
                else
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemInSlot, amountToMove);
                PlayerInventory.Instance.TryAddingItemDragging(itemToAdd, amountToMove, true);
                itemSlotBeforeDrop.ClearItemSlot();
            }else if (this.amount == GameManager.Instance.GetMaxAmountPerSlot())
            {
                StackItemsFromCrateToInventory(itemSlotBeforeDrop, amountToMove, draggableItem, itemToAdd);
            }
            else
            {
                int remainingSpace = (this.amount + amountToMove) - GameManager.Instance.GetMaxAmountPerSlot();
                int valueToMove = GameManager.Instance.GetMaxAmountPerSlot() - this.amount;
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.DeleteItemFromList(itemInSlot, valueToMove);
                else
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemInSlot, valueToMove);
                PlayerInventory.Instance.TryAddingItemDragging(itemToAdd, valueToMove, true);
                itemSlotBeforeDrop.ModifyItemSlotAmount(remainingSpace);
                this.ModifyItemSlotAmount(GameManager.Instance.GetMaxAmountPerSlot());
                
                StackItemsFromCrateToInventory(itemSlotBeforeDrop, remainingSpace, draggableItem, itemToAdd);
            }
        }
        /// <summary>
        /// Aux method from crate to inventory
        /// </summary>
        /// <param name="itemSlotBeforeDrop"></param>
        /// <param name="remainingSpace"></param>
        /// <param name="draggableItem"></param>
        /// <param name="itemToAdd"></param>
        private void StackItemsFromCrateToInventory(ItemSlot itemSlotBeforeDrop, int remainingSpace, DraggableItem draggableItem, Item itemToAdd)
        {
            if (InventoryManager.Instance.TryAddInventoryToItemSlot(itemToAdd,
                    remainingSpace,
                    out int remainingItems))
            {
                //We have size, we take all items of this type
                if (remainingSpace == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - remainingSpace);
                
                
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), remainingSpace, true);
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.DeleteItemFromList(itemToAdd, remainingSpace);
                else
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToAdd,
                        remainingSpace);
                draggableItem.SetItemComingFromInventoryToCrate(false);
                draggableItem.parentAfterDrag = this.transform;
            }
            else
            {
                PlayerInventory.Instance.TryAddingItemDragging(this.GetItemInSlot(), remainingSpace - remainingItems,
                    true);
                itemSlotBeforeDrop.SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), remainingItems);
            }

        }
        
        /// <summary>
        /// Stacking items in crate
        /// </summary>
        /// <param name="draggableItem"></param>
        /// <param name="amountToMove"></param>
        private void StackingItemsInCrateDragging(DraggableItem draggableItem, int amountToMove)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            Item itemToAdd = itemSlotBeforeDrop.GetItemInSlot();
            int remainingItems = 0;


            if (this.amount + amountToMove <= GameManager.Instance.GetMaxAmountPerSlot())
            {
                //Just move 
                this.ModifyItemSlotAmount(this.amount + amountToMove);
                            
                if (amountToMove == itemSlotBeforeDrop.amount)
                    ResetItemSlot(itemSlotBeforeDrop, draggableItem);
                else
                    itemSlotBeforeDrop.ModifyItemSlotAmount(itemSlotBeforeDrop.amount - amountToMove);
            }
            else
            {
                int valueRemainingInPreviousSlot = (this.amount + itemSlotBeforeDrop.amount) - GameManager.Instance.GetMaxAmountPerSlot();
                this.ModifyItemSlotAmount(GameManager.Instance.GetMaxAmountPerSlot());
                itemSlotBeforeDrop.ModifyItemSlotAmount(valueRemainingInPreviousSlot);
            }
        }

        private void MovingItemToOtherSlot(DraggableItem draggableItem, bool fromInventoryToCrate, int amountToMove, bool showMessage)
        {
            ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
            SetItemSlotProperties(itemSlotBeforeDrop.GetItemInSlot(), amountToMove);
            if (fromInventoryToCrate)
            {
                draggableItem.SetItemComingFromInventoryToCrate(true);
                Debug.Log(SceneManager.GetActiveScene().name.ToString());
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.AddItemToList(this.GetItemInSlot(), amountToMove);
                else 
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
                    if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                        TrainBaseInventory.Instance.DeleteItemFromList(itemSlotBeforeDrop.GetItemInSlot(), amountToMove);
                    else 
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
            if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemToLoot, amountToLoot, out remainingItemsWithoutSpace);
            else
                 LootUIManager.Instance.TryAddItemCrateToItemSlot(itemToLoot, amountToLoot, out remainingItemsWithoutSpace);
            if (remainingItemsWithoutSpace > 0)
            {
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.AddItemToList(itemToLoot, amountToLoot - remainingItemsWithoutSpace);
                else 
                    LootUIManager.Instance.GetCurrentLootableObject().AddItemToList(itemToLoot, 
                        amountToLoot - remainingItemsWithoutSpace);
                PlayerInventory.Instance.RemovingItem(itemToLoot, amountToLoot - remainingItemsWithoutSpace);
                ModifyItemSlotAmount(remainingItemsWithoutSpace);
            }
            else
            {
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.AddItemToList(itemToLoot, amountToLoot);
                else 
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
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                {
                    TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemToLoot, remainingItemsWithoutSpace,
                        out int remainingItems);
                    TrainBaseInventory.Instance.AddItemToList(itemToLoot, remainingItemsWithoutSpace);
                }
                else
                {
                    LootUIManager.Instance.TryAddItemCrateToItemSlot(itemToLoot, remainingItemsWithoutSpace,
                        out int remainingItems);
                    LootUIManager.Instance.GetCurrentLootableObject()
                        .AddItemToList(itemToLoot, remainingItemsWithoutSpace);
                }

                //And update item slot
                ModifyItemSlotAmount(remainingItemsWithoutSpace);
            }
            else
            {
                if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    TrainBaseInventory.Instance.DeleteItemFromList(itemToLoot, this.amount);
                else
                    LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemToLoot, this.amount);
                
                    
                this.ClearItemSlot();
            }
        }

        private bool ValidMovementFromInventoryToCrate()
        {
            if (SceneManager.GetActiveScene().name != GameManager.Instance.GetNameTrainScene())
                return this.itemID != 0 && LootUIManager.Instance.GetIfCrateIsOpened() && !this.GetIfIsLootCrate();
            else
                return this.itemID != 0 && !this.GetIfIsLootCrate();
        }
        
        private bool ValidMovementFromCrateToInventory()
        {      
            if (SceneManager.GetActiveScene().name != GameManager.Instance.GetNameTrainScene())
                return this.itemID != 0 && LootUIManager.Instance.GetIfCrateIsOpened() && this.GetIfIsLootCrate();
            else
                return this.itemID != 0 && this.GetIfIsLootCrate();
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
                 if (SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                 {
                     TrainBaseInventory.Instance.DeleteItemFromList(GetItemInSlot(), amount);
                 }
                 else
                     LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(GetItemInSlot(), amount);
             }
             else
             {
                 //We throw item out of inventory
                 PlayerInventory.Instance.RemovingItem(GetItemInSlot(), amount);
             }
             //We create new box
             if (SceneManager.GetActiveScene().name != GameManager.Instance.GetNameTrainScene())
             {
                 LooteableObject temporalBox = LooteableObjectSelector.Instance.GetClosestTemporalBox();

                 if (temporalBox != null && temporalBox.CheckIfSlotAvailable())
                 {
                     Debug.Log(temporalBox.itemsInLootCrate.Count);
                     temporalBox.AddItemToList(GetItemInSlot(), amount);
                 }
                 else
                 {
                     GameObject looteableObject = Instantiate(InventoryManager.Instance.GetLooteableObjectPrefab(),
                         PlayerInventory.Instance.transform.position, Quaternion.identity);
                     LooteableObject lootObject = looteableObject.GetComponent<LooteableObject>();
                     lootObject.SetIfItIsTemporalBox(true);
                     lootObject.ClearLooteableObject();
                     lootObject.AddItemToList(GetItemInSlot(), amount);  
                 }
             }

             ClearItemSlot();
             
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            //Doble click eventData.clickCount == 2
            if (Input.GetKey(KeyCode.LeftShift) && eventData.button == PointerEventData.InputButton.Left) {
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
                Destroy(hoverGameObject);
                if (SceneManager.GetActiveScene().name != GameManager.Instance.GetNameTrainScene())
                    CheckIfWeNeedToHideHUD();
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (itemID != 0)
                {
                    Destroy(hoverGameObject);
                    if(SceneManager.GetActiveScene().name == GameManager.Instance.GetNameTrainScene())
                    {
                        if (TrainManager.Instance.TrainStatus == TrainStatus.onMarketScreen && TrainInventoryManager.Instance.isSellingItems)
                        {
                            SellItem();
                        }
                        else
                        {
                            TrainInventoryManager.Instance.ActivateContextMenuInterface(this);  
                        }
                       
                    }
                    else
                    {
                        InventoryManager.Instance.ActivateContextMenuInterface(this);
                    }
                   
                }
            }
        }

        public void SellItem()
        {
            int goldEarned = this.GetItemInSlot().itemPriceAtMarket * this.amount;
            TrainManager.Instance.resourceAirFilter += goldEarned;
            MarketSystem.Instance.ShowGoldEarnedByItemSold(goldEarned, this);
            PlayerInventory.Instance.RemovingItem(this.GetItemInSlot(), this.amount);
            this.ClearItemSlot();
        }

        private void CheckIfWeNeedToHideHUD()
        {
            LooteableObject loot = LootUIManager.Instance.GetCurrentLootableObject();
            if (loot != null)
            {
                if (loot.GetIfItIsTemporalBox() && loot.CheckIfLootBoxIsEmpty())
                {
                    Destroy(loot.gameObject);
                    InventoryManager.Instance.DesactivateInventory();
                    LootUIManager.Instance.DesactivateLootUIPanel();
                }
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.itemID != 0 && !isLootCrate)
            {
                canThrowItemAway = true;
                StartCoroutine(StartHoverCountdown());
            }
        }
        
        

        private IEnumerator StartHoverCountdown()
        {
            yield return new WaitForSeconds(0.4f); 
            Vector2 offsetPosition = new Vector2(75, 35);
            if (SceneManager.GetActiveScene().name == "TrainBase")
            {
               hoverGameObject =  Instantiate(TrainInventoryManager.Instance.HoverItemPrefab, (Vector2)
                  this.transform.position + offsetPosition,
                    Quaternion.identity, GameManager.Instance.GetMenuCanvas().transform);
            }
            else
            { 
                hoverGameObject = Instantiate(InventoryManager.Instance.HoverItemPrefab, (Vector2)
                    this.transform.position + offsetPosition,
                    Quaternion.identity, GameManager.Instance.GetMenuCanvas().transform);
            }
            
            hoverGameObject.GetComponentInChildren<HoverItem>().SetUpHoverView(this.GetItemInSlot());
            Debug.Log("KW: START HOVER" );
        }

        public void ShowBlackPanel()
        {
            blackPanel.SetActive(true);
        }
        public void HideBlackPanel()
        {
            blackPanel.SetActive(false);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.itemID != 0)
            {
                canThrowItemAway = false;
                if (hoverGameObject != null)
                {
                    Destroy(hoverGameObject);
                }
            }
            StopAllCoroutines();
        }

        public void HideSearchPanel()
        {
            this.searchPanel.SetActive(false);
            this.gameObject.GetComponentInChildren<Animator>(true).SetBool("StartLoading", false);
        }
        public void ActivateSearchLoadingAnimation()
        {
            this.gameObject.GetComponentInChildren<Animator>(true).SetBool("StartLoading", true);
        }
        public void ShowSearchPanel()
        {
            this.searchPanel.SetActive(true);
        }
    }
}