using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.UI;
using Utils.CustomLogs;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Loot
{


    public class LooteableObject : MonoBehaviour
    {

        [SerializeField] private GameObject hotkeyImage;
        private Dictionary<Item, int> itemsInLootableObject;
        [SerializeField] private bool onlyOneItemInBag;
        [SerializeField] private bool needToSpawnXObject;
        private bool isLooting = false;
        /// <summary>
        /// When we need to spawn X Items 100%
        /// </summary>
        private List<Item> itemsNeededToSpawn;
        [SerializeField] private int maxSlotsInCrate;
        
        private bool _isLooteable = false;
        public bool IsLooteable
        {
            get { return _isLooteable; }
            set { _isLooteable = value; }
        }

        private void Start()
        {
            itemsInLootableObject = new Dictionary<Item, int>();
            itemsNeededToSpawn = new List<Item>();
            List<string> testList = new List<string>();

            if (needToSpawnXObject)
            {
                testList.Add("Keycard");
                InitializeLootObject(testList);  
            }
            else
            {
                InitializeLootObject(null);  
            }
            
        }

        private void InitializeLootObject(List<string> itemsList)
        {
            if (itemsList != null)
            {
                PrepareItemsNeededToSpawn(itemsList);
                int remainingItems = maxSlotsInCrate - itemsList.Count;
                if (remainingItems > 0)
                {
                    PrepareLoot(remainingItems); 
                }
                else
                {
                    Debug.Log("NO SLOTS AVAILABLE FOR THAT CRATE");
                }
            }
            else
            {
                PrepareLoot(maxSlotsInCrate);
            }
        }

        private void PrepareItemsNeededToSpawn(List<string> itemsList)
        {
            foreach (var itemName in itemsList)
            {
                Object itemNeeded = UnityEngine.Resources.Load("Items/Keycards/Keycard");
                Item itemSO = itemNeeded as Item;
                itemsNeededToSpawn.Add(itemSO);
                itemsInLootableObject.Add(itemSO, 1);
            }
        }

        private void PrepareLoot(int remainingSlotsInCrate)
        {
            Object[] allItems = UnityEngine.Resources.LoadAll("Items/Scrap");
            List<Object> allItemsList = allItems.ToList();
            int itemsToLoot = 1;
            if (!onlyOneItemInBag)
                itemsToLoot = Random.Range(1, remainingSlotsInCrate);
                //itemsToLoot = Random.Range(2, 3); NO PANEL
            for (int i = 0; i < itemsToLoot; i++)
            {
                int randomItemIndex = Random.Range(0, allItemsList.Count);
                int randomQuantity = Random.Range(1, 4);
                Item itemSO = allItemsList[randomItemIndex] as Item;
                itemsInLootableObject.Add(itemSO, randomQuantity);
                //WE MODIFIE THE UI
                //looteableObjectUI.AddItemToCrate(itemSO, randomQuantity);
                allItemsList.RemoveAt(randomItemIndex);
            }
        }
        private void Update()
        {
            if (_isLooteable)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //Loot
                    if (LootUIManager.Instance.GetIfCrateIsOpened())
                    {
                        LootUIManager.Instance.DesactivateLootUIPanel();
                        InventoryManager.Instance.DesactivateInventory();
                        //looteableObjectUI.DesactivateLooteablePanel(); 
                    }
                    else
                    {
                        //We load objects to this panel
                        LootUIManager.Instance.SetPropertiesAndLoadPanel(this, itemsInLootableObject);
                        InventoryManager.Instance.ActivateInventory();
                        //looteableObjectUI.ActivateLooteablePanel();
                    }
                    //LootAllItems();
                }
            }
        }

        public void LootAllItems()
        {
            Dictionary<Item, int> recoverItems = new Dictionary<Item, int>();
            Dictionary<Item, int> itemsTaken = new Dictionary<Item, int>();
            foreach (var item in itemsInLootableObject)
            {
                int remainingItems = 0;
                if (!PlayerInventory.Instance.TryAddItem(item.Key, item.Value, 
                        out remainingItems, false))
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
            itemsInLootableObject.Clear();
            foreach (var items in recoverItems)
            {
                itemsInLootableObject.Add(items.Key, items.Value);
            }
            
            //Text to indicate we take X Item
            PlayerInventory.Instance.ShowFullListItemTaken(itemsTaken);
            InventoryManager.Instance.ChangeText(PlayerInventory.Instance.GetInventoryItems());
        }
        
        public void AddItemToList(Item item, int amount)
        {
            if (itemsInLootableObject.ContainsKey(item))
            {
                itemsInLootableObject[item] += amount;
            }
            else
            {
                itemsInLootableObject.Add(item, amount);
            }
        }
        
        public void DeleteItemFromList(Item item, int amount)
        {
            if (itemsInLootableObject[item] > amount)
            {
                itemsInLootableObject[item] -= amount; 
            }
            else
            {
                itemsInLootableObject.Remove(item); 
            }
            
        }

        public void ActivateKeyHotkeyImage()
        {
            hotkeyImage.SetActive(true);
            _isLooteable = true;
        }

        public void DesactivateKeyHotkeyImage()
        {
            hotkeyImage.SetActive(false);
            _isLooteable = false;
        }


    }
}