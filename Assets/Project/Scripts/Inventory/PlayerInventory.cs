using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using LootSystem;
using Player;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.CustomLogs;

namespace Inventory
{

    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance;

        [SerializeField] private GameObject floatingTextPrefab;
        
        private Dictionary<Item, int> inventoryItemDictionary;
        private float currentWeight;
        private float MAX_WEIGHT = 34.5f;
        private int MAX_INVENTORY_SLOTS = 10;

        [Header("Player Prefs")] 
        private string RESOURCES_AIR_FILTER_NAME = "Resources_Air_Filter";

        private int gasFilterID = 12;



        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[PlayerInventory.cs] : There is already a PlayerInventory Instance");
                Destroy(this);
            }

            Instance = this;
        }

        void Start()
        {
            inventoryItemDictionary = new Dictionary<Item, int>();
        }

        public bool TryAddItem(Item item, int amount, out int remainingItemsWithoutSpace, bool showItemsTakenMessage)
        {  
            if (SceneManager.GetActiveScene().name != "TrainBase")
                return TryAddItemInGame(item, amount, out remainingItemsWithoutSpace, showItemsTakenMessage);
            else
               return TryAddItemInBase(item, amount, out remainingItemsWithoutSpace, showItemsTakenMessage);
        }

        public void RemoveCoinFromInventory()
        {
            Item gasFilter = null;
            foreach (var item in inventoryItemDictionary)
            {
                if (item.Key.itemID == gasFilterID)
                {
                     gasFilter = item.Key;
                }
            }
            if (gasFilter != null)
            {
                inventoryItemDictionary.Remove(gasFilter);
            }
        }
        
        public bool GetIfItemIsInPlayerInventory(Item item, int amount)
        {
            int amountAux = -1;
            inventoryItemDictionary.TryGetValue(item, out amountAux);
            return amountAux >= amount;
        }

        public bool TryAddItemInBase(Item item, int amount, out int remainingItemsWithoutSpace,
            bool showItemsTakenMessage)
        {
            if (TrainInventoryManager.Instance.TryAddInventoryToItemSlot(item, amount, out remainingItemsWithoutSpace))
            {
                if (inventoryItemDictionary.ContainsKey(item))
                {
                    inventoryItemDictionary[item] += amount;
                }
                else
                {
                    inventoryItemDictionary.Add(item, amount);
                }  
                
                //Change Weight
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.CurrentWeight += item.itemWeight * amount;
                }
               
                if(showItemsTakenMessage)
                    ShowItemTaken(item.itemName, amount - remainingItemsWithoutSpace);
                if (remainingItemsWithoutSpace > 0)
                    return false;
                else
                    return true; 
            }
            else
            {
                return false;
            }
        }
        
    
        
        public void StashAllItemsInBase()
        {
            Dictionary<Item, int> recoverItems = new Dictionary<Item, int>();
            Dictionary<Item, int> itemsTaken = new Dictionary<Item, int>();
            foreach (var item in inventoryItemDictionary)
            {
                int remainingItems = 0;
                if (!TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(item.Key, item.Value, 
                        out remainingItems))
                {
                    //If we cant find a place, we add it to recover items
                    //We will need to check if we take X amount of the stack
                    recoverItems.Add(item.Key, remainingItems);
                }
                else
                {
                    itemsTaken.Add(item.Key, item.Value);
                }
            } 
            //We cant clear, we need to check if we dont take an item because we dont have space in inventory
            this.inventoryItemDictionary.Clear();
            foreach (var items in recoverItems)
            {
                inventoryItemDictionary.Add(items.Key, items.Value);
            }
        }
        public bool TryAddItemInGame(Item item, int amount, out int remainingItemsWithoutSpace,
            bool showItemsTakenMessage)
        {
            if (InventoryManager.Instance.TryAddInventoryToItemSlot(item, amount, out remainingItemsWithoutSpace))
            {
                if (inventoryItemDictionary.ContainsKey(item))
                {
                    inventoryItemDictionary[item] += amount;
                }
                else
                {
                    inventoryItemDictionary.Add(item, amount);
                }  

                if(showItemsTakenMessage)
                    ShowItemTaken(item.itemName, amount - remainingItemsWithoutSpace);

                //Change Weight
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.CurrentWeight += item.itemWeight * amount;
                    InventoryManager.Instance.ChangeText();
                }
 
                if (remainingItemsWithoutSpace > 0)
                    return false;
                else
                    return true; 
            }
            else
            {
                return false;
            }
        }

        public bool TryAddingItemDragging(Item item, int amount, bool showMessage)
        {
            if (inventoryItemDictionary.ContainsKey(item))
            {
                inventoryItemDictionary[item] += amount;
            }
            else
            {
                inventoryItemDictionary.Add(item, amount);
            }

            if(showMessage)
                ShowItemTaken(item.itemName, amount);
            //Change Weight
            if (SceneManager.GetActiveScene().name != "TrainBase")
            {
                 if (PlayerController.Instance != null)
                 {
                     PlayerController.Instance.CurrentWeight += item.itemWeight * amount;
                     InventoryManager.Instance.ChangeText();
                 }
            }
            return true; 
        }

        private void ShowItemTaken(string name, int amount)
        {
            if (amount > 0)
            {
                if (floatingTextPrefab)
                {
                    GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
                    prefab.GetComponentInChildren<TextMeshProUGUI>().text = "x" + amount + " " + name;
                } 
            }
        }

        public void ShowPlayerHasTooMuchWeight()
        {
            if (floatingTextPrefab)
            {
                GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = "You can't handle to much weight, drop something";
            } 
        }

        public void HandleItemsAtEndGame()
        {
            int currentAirFilter = PlayerPrefs.GetInt(RESOURCES_AIR_FILTER_NAME);

            foreach (var item in inventoryItemDictionary)
            {
                switch (item.Key.ItemType)
                {
                    //Get Air Filters
                    case ItemType.Scrap:
                        if (item.Key.itemID == gasFilterID)
                        {
                            currentAirFilter += item.Value * item.Key.itemPriceAtMarket;
                        }
                        break;
                }
            }
            PlayerPrefs.SetInt(RESOURCES_AIR_FILTER_NAME, currentAirFilter);

        }
        
        public void ShowFullListItemTaken(Dictionary<Item, int> itemList)
        {
            string fullItemList = "";

            foreach (var item in itemList)
            {
                fullItemList += "x" + item.Value + " " + item.Key.itemName + "\n"; 
            }
            
            if (floatingTextPrefab)
            {
                GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = fullItemList;
            }
        }

        public Dictionary<Item, int> GetInventoryItems()
        {
            return inventoryItemDictionary;
        }
        public void RemovingItem(Item item, int itemSlotAmount)
        {
            if (inventoryItemDictionary.ContainsKey(item))
            {         
                inventoryItemDictionary[item] -= itemSlotAmount;
                if (inventoryItemDictionary[item] <= 0)
                {
                    inventoryItemDictionary.Remove(item);
                }
                
                //Change Weight
                if (InventoryManager.Instance != null)
                {
                    if (PlayerController.Instance != null)
                    {
                        PlayerController.Instance.CurrentWeight -= item.itemWeight * itemSlotAmount;
                    }
                    InventoryManager.Instance.ChangeText();
                }
                    
            }
        }

        public bool CheckIfPlayerHasKey()
        {
            Object itemNeeded = UnityEngine.Resources.Load("Items/Special/Keycard");
            Item keycardItem = itemNeeded as Item;
            return inventoryItemDictionary.ContainsKey(keycardItem);
        }


        private void UpdateFullCurrentWeight()
        {
            foreach (var item in inventoryItemDictionary)
            {
                currentWeight += item.Value * item.Key.itemWeight; 
            }
        }
        
        public void SetInventoryDictionary(Dictionary<Item, int> inventory)
        {
            this.inventoryItemDictionary = inventory;
        }
        
    }
}
