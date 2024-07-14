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
        private GameObject currentHotkeyGameObject;
        private Dictionary<Item, int> itemsInLootableObject;
        [SerializeField] private bool onlyOneItemInBag;
        [SerializeField] private bool needToSpawnXObject;
        [SerializeField] private List<string> itemsToSpawn;
        private bool isLooting = false;
        private bool isTemporalBox = false; 
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

        private void Awake()
        {
            itemsInLootableObject = new Dictionary<Item, int>();
            itemsNeededToSpawn = new List<Item>();
            List<string> testList = new List<string>();
            
            //En el futuro, hay que ver esto, porque no podemos hacer spawn en el start, habrá que modificar las opciones antes
            StartSpawingObjects(itemsToSpawn);
        }
        private void Update()
        {
            if (_isLooteable)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (LooteableObjectSelector.Instance.GetIfSelectorIsActive() &&
                        LooteableObjectSelector.Instance.GetIfIndexIsThisLooteableObject(this))
                    {
                        //CUIDADO QUE ESTÁ AL REVES, PILLA EL NOMBRE DEL OTRO
                        Debug.Log(this.name);
                        HandleInventory();
                    }
                    
                    
                    if(!LooteableObjectSelector.Instance.GetIfSelectorIsActive())
                    {
                        //No scroll selector
                        HandleInventory();
                    }
                }
            }
        }

        private void HandleInventory()
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
        }
        
        public void ClearLooteableObject()
        {
            itemsInLootableObject = new Dictionary<Item, int>();
            itemsInLootableObject.Clear();
        }
        
        public void SetIfItIsTemporalBox(bool aux)
        {
            this.isTemporalBox = aux;
        }

        public bool GetIfItIsTemporalBox()
        {
            return isTemporalBox;
        }

        public bool CheckIfLootBoxIsEmpty()
        {
            return itemsInLootableObject.Count == 0;
        }
        
        public void StartSpawingObjects(List<string> testList)
        {
            if (needToSpawnXObject)
            {
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
                Object itemNeeded = UnityEngine.Resources.Load("Items/Special/" + itemsList[0].ToString());
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
            
            if (isTemporalBox)
            {
                Destroy(this.gameObject);
                Debug.Log("DESTROYING TEMPORAL BOX");
                InventoryManager.Instance.DesactivateInventory();
                LootUIManager.Instance.DesactivateLootUIPanel();
            }
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
            currentHotkeyGameObject = Instantiate(LootUIManager.Instance.GetHotkeyPrefab(),
                new Vector2(this.transform.position.x, this.transform.position.y + 1), Quaternion.identity); 
            _isLooteable = true;
        }

        public void DesactivateKeyHotkeyImage()
        {
            Destroy(currentHotkeyGameObject);
            currentHotkeyGameObject = null;
            _isLooteable = false;
        }


    }
}