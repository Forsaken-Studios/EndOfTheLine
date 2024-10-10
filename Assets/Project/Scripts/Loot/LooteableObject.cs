using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.CustomLogs;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Loot
{
    public class ItemInterval
    {
        public float minNumber;
        public float maxNumber;

        public ItemInterval(float minNumber, float maxNumber)
        {
            this.minNumber = minNumber;
            this.maxNumber = maxNumber;
            
        }
    }
    
    public class LooteableObject : MonoBehaviour
    {
        [SerializeField] private LootSpriteContainer chestType;
        private SpriteRenderer chestSprite;
        private GameObject currentHotkeyGameObject;
        private Dictionary<Item, int> itemsInLootableObject;
        private bool chestOpened = false;
        [SerializeField] private bool onlyOneItemInBag;
        [SerializeField] private bool needToSpawnXObject;
        [SerializeField] private List<string> itemsToSpawn;
        private bool isLooting = false;
        [SerializeField] private float verticalOffset = 0.5f;

        private Dictionary<Item, ItemInterval> itemsIntervalSpawn;
        private bool isTemporalBox = false; 
        /// <summary>
        /// When we need to spawn X Items 100%
        /// </summary>
        private List<Item> itemsNeededToSpawn;
        [SerializeField] private int maxSlotsInCrate;
        private List<ItemInterval> intervalList;
        
        private bool _isLooteable = false;
        public bool IsLooteable
        {
            get { return _isLooteable; }
            set { _isLooteable = value; }
        }

        private void Awake()
        {
            chestSprite = GetComponent<SpriteRenderer>();
            itemsInLootableObject = new Dictionary<Item, int>();
            itemsNeededToSpawn = new List<Item>();
            intervalList = new List<ItemInterval>();
            //En el futuro, hay que ver esto, porque no podemos hacer spawn en el start, habrá que modificar las opciones antes
            LootManager.Instance.AddLooteableObjectToList(this);
         
        }
        private void Update()
        {
            if (_isLooteable)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    AbilityManager.Instance.Holder1.TryToCancelAbility();
                    AbilityManager.Instance.Holder2.TryToCancelAbility();
                    if (InventoryManager.Instance.GetInspectViewList().Count == 0)
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
        }

        private void HandleInventory()
        {
            //Loot
            if (LootUIManager.Instance.GetIfCrateIsOpened())
            {
                LootUIManager.Instance.DesactivateLootUIPanel();
                InventoryManager.Instance.DesactivateInventory();
                SetSpriteToEmptyCrate();
            }
            else
            {
                //We load objects to this panel
                HandleCrateSprite();
                LootUIManager.Instance.SetPropertiesAndLoadPanel(this, itemsInLootableObject);
                InventoryManager.Instance.ActivateInventory();
            }
        }

        private void HandleCrateSprite()
        {
            if (itemsInLootableObject.Count > 0)
            {
                this.chestSprite.sprite = LootUIManager.Instance.GetLootSprite(this.chestType, LootSpriteType.Looted);
            }
            else
            {
                this.chestSprite.sprite = LootUIManager.Instance.GetLootSprite(this.chestType, LootSpriteType.Empty);
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
        
        public void StartSpawingObjects()
        {
            itemsIntervalSpawn = new Dictionary<Item, ItemInterval>();
            if (needToSpawnXObject)
            {
                InitializeLootObject(itemsToSpawn);  
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
                if (!onlyOneItemInBag)
                {
                    int remainingItems = maxSlotsInCrate - itemsList.Count;
                    if (remainingItems >= 2)
                    {
                        remainingItems = UnityEngine.Random.Range(1, 3);
                    }
                    if (remainingItems > 0)
                    {
                        PrepareLoot(remainingItems); 
                    }
                    else
                    {
                        Debug.Log("NO SLOTS AVAILABLE FOR THAT CRATE");
                    } 
                }
            
            }
            else
            {
                PrepareLoot(LootManager.Instance.GetRandomAmount());
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
            List<Item> allItems = UnityEngine.Resources.LoadAll<Item>("Items/Scrap").ToList();
            List<Item> remainingItems = allItems;
            float intervalAcount = 0;
            foreach (var item in allItems)
            {
                ItemInterval itemInverval = new ItemInterval(intervalAcount, intervalAcount + item.itemSpawnChance);
                itemsIntervalSpawn.Add(item, itemInverval);
                intervalList.Add(itemInverval);
                intervalAcount += item.itemSpawnChance + 1;
            }
            int itemsToLoot = 1;
            if (!onlyOneItemInBag)
                itemsToLoot = remainingSlotsInCrate;
            
            Debug.Log(itemsToLoot);

            for (int i = 0; i < itemsToLoot; i++)
            {
                int value = (int) Random.Range(0, intervalAcount);
                
                foreach (var item in itemsIntervalSpawn)
                {
                    if (item.Value.minNumber <= value && item.Value.maxNumber >= value)
                    {
                        int randomQuantity = Random.Range(1, 4);
                        if (itemsInLootableObject.ContainsKey(item.Key))
                        {
                            itemsInLootableObject[item.Key] += randomQuantity;
                        }
                        else
                        {
                            itemsInLootableObject.Add(item.Key, randomQuantity);
                        }
                        intervalAcount = GenerateNewIntervalCount(item.Key, remainingItems);
                        break;
                    }
                }
            
            }
        }

        private int GenerateNewIntervalCount(Item itemToDelete, List<Item> remainingItems)
        {
            int intervalAcount = 0;
            itemsIntervalSpawn.Clear();
            foreach (var item in remainingItems)
            {
                if (item != itemToDelete)
                {
                    ItemInterval itemInverval = new ItemInterval(intervalAcount, intervalAcount + item.itemSpawnChance);
                    itemsIntervalSpawn.Add(item, itemInverval);
                    intervalList.Add(itemInverval);
                    intervalAcount += (int) item.itemSpawnChance + 1;  
                }
            }

            remainingItems.Remove(itemToDelete);
            return intervalAcount;
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

            if (recoverItems.Count == 0)
            {
                this.chestSprite.sprite = LootUIManager.Instance.GetLootSprite(this.chestType, LootSpriteType.Empty);
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

        public void SetSpriteToEmptyCrate()
        {
            if (itemsInLootableObject.Count == 0)
            {
                this.chestSprite.sprite = LootUIManager.Instance.GetLootSprite(this.chestType, LootSpriteType.Empty);
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
                new Vector2(this.transform.position.x, this.transform.position.y + verticalOffset), Quaternion.identity); 
            _isLooteable = true;
        }

        public void DesactivateKeyHotkeyImage()
        {
            Destroy(currentHotkeyGameObject);
            currentHotkeyGameObject = null;
            _isLooteable = false;
        }

        public void SetIfNeedToSpawnXObject(List<string> itemsToSpawn)
        {
            this.itemsToSpawn = itemsToSpawn;
            needToSpawnXObject = true;
        }
        
    }
}