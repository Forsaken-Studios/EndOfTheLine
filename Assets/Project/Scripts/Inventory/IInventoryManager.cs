using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
using Utils.CustomLogs;

public abstract class IInventoryManager : MonoBehaviour
{
    
    [SerializeField] protected GameObject inventoryHUD;
    [SerializeField] protected List<ItemSlot> itemSlotList;
    protected int nextIndexSlotAvailable = 0;
    [SerializeField] protected GameObject rightClickInterfacePrefab;
    protected GameObject currentRightClickInterface; 
    protected List<GameObject> inspectListViewList;
    [SerializeField] private GameObject canvasInventory;
    
    public virtual void Start()
    {
        inspectListViewList = new List<GameObject>();
        inventoryHUD.SetActive(false);
    }
    
    /// <summary>
    /// If inventory is opened, we close it, if it is the other way, we open it
    /// </summary>
    public virtual void ReverseInventoryStatus()
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
        GameManager.Instance.GameState = inventoryHUD.activeSelf ? GameState.OnInventory: GameState.OnGame;
    }

    public void ActivateInventory()
    {
        GameManager.Instance.GameState = GameState.OnInventory;
        inventoryHUD.SetActive(true);
    }

    public void DesactivateInventory()
    {
        GameManager.Instance.GameState = GameState.OnGame;
        inventoryHUD.SetActive(false);
        TryDestroyContextMenu();
    }   


        
        /// <summary>
        /// Methods that finds and set the first available spot for item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="remainingItemsWithoutSpace"></param>
        /// <returns></returns>
        public virtual bool TryAddInventoryToItemSlot(Item item, int amount, out int remainingItemsWithoutSpace)
        {
            int availableIndex = 0;
            remainingItemsWithoutSpace = 0;
            foreach (var itemSlot in itemSlotList)
            {
                if (itemSlot.itemID == item.itemID)
                {
                    int totalAmount = itemSlot.amount + amount;
                    if (totalAmount <= GameManager.Instance.GetMaxAmountPerSlot())
                    {
                        LogManager.Log("FIND EMPTY SLOT", FeatureType.Loot);
                        //ADD ELEMENT TO THIS SLOT
                        itemSlot.AddMoreItemsToSameSlot(amount);
                        return true;
                    }
                    else
                    {
                        if (itemSlot.amount != GameManager.Instance.GetMaxAmountPerSlot()) //We dont want to check a full slot
                        {
                            //We refill one slot and fill other slots until we are completed
                            int amountToFill = GameManager.Instance.GetMaxAmountPerSlot() - itemSlot.amount; //Space available
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

        public virtual bool SetRemainingItemsSlots(int amount, int availableIndex, Item item, out int remainingAmount)
        {
            if (amount >= GameManager.Instance.GetMaxAmountPerSlot())
            {
                itemSlotList[availableIndex].SetItemSlotProperties(item, GameManager.Instance.GetMaxAmountPerSlot());
                int remainingItemsAux = amount - GameManager.Instance.GetMaxAmountPerSlot();
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

        public virtual int GetNextAmountToFill(int totalAmount, out int remainingItems)
        {
            // if > MAX -> itemToFill = MAX | remaining -= MAX
            // if < MAX -> itemToFill = remaining
            remainingItems = totalAmount;
            if (totalAmount >= GameManager.Instance.GetMaxAmountPerSlot())
            {
                remainingItems -= GameManager.Instance.GetMaxAmountPerSlot();
                return GameManager.Instance.GetMaxAmountPerSlot();
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
            return GameManager.Instance.GetMaxAmountPerSlot();
        }
        
        public void ActivateContextMenuInterface(ItemSlot itemSlot)
        {
            
            if (currentRightClickInterface != null)
            {
                Destroy(currentRightClickInterface.gameObject);
            }
            GameObject rightClickInterface = Instantiate(rightClickInterfacePrefab, Input.mousePosition, Quaternion.identity);
            rightClickInterface.GetComponentInChildren<ContextMenu.ContextMenu>().SetItemSlotProperties(itemSlot);
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

        public List<GameObject> GetInspectViewList()
        {
            return inspectListViewList;
        }

        public void AddInspectView(GameObject gameObject)
        {
            if (inspectListViewList.Count == 3)
            {
                GameObject mostRecentInspectView = inspectListViewList[0];
                Destroy(mostRecentInspectView);
                RemoveInspectView(mostRecentInspectView);
            }
            this.inspectListViewList.Add(gameObject);
        }  
        public void RemoveInspectView(GameObject gameObject)
        {
            this.inspectListViewList.Remove(gameObject);
        }
}
