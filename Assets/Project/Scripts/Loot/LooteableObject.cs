using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace LootSystem
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

    public enum ItemsEnum
    {
       None,  
       Battery,
       Coin_AirFilter, 
       EmptyFoodCan,
       EmptyItem, 
       Flashlight,
       FoodcanHam,
       FoodCanTomSauce, 
       MetalPlate, 
       MineralGreen, 
       MineralPurple, 
       MineralRed,
       Pickaxe,
       Scrap, 
       Screws,
       Shovel, 
       Sickle, 
       Tools
    }
    
    public class LooteableObject : MonoBehaviour
    {
        [SerializeField] private LootSpriteContainer chestType;
        public LootSpriteContainer ChestType
        {
            get { return chestType; }
        }
        private SpriteRenderer chestSprite;
        private GameObject currentHotkeyGameObject;
        private Dictionary<Item, int> itemsInLootableObject;
        private bool alreadyChecked = false;
        public bool AlreadyChecked
        {
            get { return alreadyChecked; }
            set { this.alreadyChecked = value; }
        }
        private int itemIndexChecked = -1;

        public int ItemIndexChecked
        {
            get { return itemIndexChecked; }
            set { this.itemIndexChecked = value; }
        }

        public bool canLootAllItems;

        public Dictionary<Item, int> itemsInLootCrate
        {
            get { return itemsInLootableObject; }
        }
        [Header("Need to spawn an specific item (Only for keycards)")]
        [SerializeField] private bool onlyOneItemInBag;
        [SerializeField] private bool needToSpawnXObject;
        [SerializeField] private List<string> itemsToSpawn;
        public bool CheckIfNeedToSpawnXObject => needToSpawnXObject;

        

        [Header("Spawn For tutorial MAX 6")] 
        [SerializeField] private bool isCrateForTutorial;
        public bool CheckIfItIsTutorial => isCrateForTutorial;
        [SerializeField] private List<ItemsEnum> itemsToSpawnForTutorial;
        public bool AlreadyLoadedWithLoot { get; private set; }

        private List<Item> itemsNeededToSpawn;
        [Header("Hotkey Prefab offset ")]
        [SerializeField] private float verticalOffset = 0.5f;
        private Dictionary<Item, ItemInterval> itemsIntervalSpawn;
        private List<ItemInterval> intervalList;
        private bool isTemporalBox = false; 
        /// <summary>
        /// When we need to spawn X Items 100%
        /// </summary>
        [SerializeField] private int maxSlotsInCrate;
 
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
                            //CUIDADO QUE ESTÁ AL REVES, PILLA EL NOMBRE DEL OTR
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
            if (chestType != LootSpriteContainer.Enemy)
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
            }else if (isCrateForTutorial)
            {
                PrepareLootForTutorial();
            }
            else
            {
                
                if (chestType == LootSpriteContainer.Enemy)
                {
                    PrepareLootForEnemyBody(2);
                }
                else
                {
                    InitializeLootObject(null);  
                }
            }
            AlreadyLoadedWithLoot = true;
        }

        private void PrepareLootForTutorial()
        {
            List<string> itemsToSpawn = new List<string>();

            foreach (var tutorialItem in itemsToSpawnForTutorial)
            {
                itemsToSpawn.Add(tutorialItem.ToString());
            }
            Debug.Log("KW: " + itemsToSpawn[0]);
            PrepareItemsNeededToSpawnForTutorial(itemsToSpawn);
   
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
                }
            }
            else
            {
                PrepareLoot(LootManager.Instance.GetRandomAmount());
            }

            AlreadyLoadedWithLoot = true;
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
            AlreadyLoadedWithLoot = true;
        }
        private void PrepareItemsNeededToSpawnForTutorial(List<string> itemsList)
        {
            foreach (var itemName in itemsList)
            {
                Object itemNeeded = UnityEngine.Resources.Load("Items/Scrap/" + itemName.ToString());
                Item itemSO = itemNeeded as Item;
                itemsNeededToSpawn.Add(itemSO);
                itemsInLootableObject.Add(itemSO, 1);
            }
        }

        private void PrepareLoot(int remainingSlotsInCrate)
        {
            List<Item> allItems = UnityEngine.Resources.LoadAll<Item>("Items/Scrap").ToList();
            List<Item> remainingItems = allItems.FindAll(item => item.canSpawnInCrates);
            float intervalAcount = 0;
            foreach (var item in remainingItems)
            {
                ItemInterval itemInverval = new ItemInterval(intervalAcount, intervalAcount + item.itemSpawnChance);
                itemsIntervalSpawn.Add(item, itemInverval);
                intervalList.Add(itemInverval);
                intervalAcount += item.itemSpawnChance + 1;
            }
            int itemsToLoot = 1;
            if (!onlyOneItemInBag)
                itemsToLoot = remainingSlotsInCrate;
            
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
                            //Debug.Log("KW3: ADDED: " + item.Key + " " + this.gameObject.transform.position) ;
                        }
                        intervalAcount = GenerateNewIntervalCount(item.Key, remainingItems);
                        break;
                    }
                }
            
            }
        }
        
        private void PrepareLootForEnemyBody(int remainingSlotsInCrate)
        {
            List<Item> allItems = UnityEngine.Resources.LoadAll<Item>("Items/Scrap").ToList();
            List<Item> remainingItems = allItems.FindAll(item => item.canSpawnInEnemyBodies);

            float intervalAcount = 0;
            foreach (var item in remainingItems)
            {
                ItemInterval itemInverval = new ItemInterval(intervalAcount, intervalAcount + item.itemSpawnChance);
                itemsIntervalSpawn.Add(item, itemInverval);
                intervalList.Add(itemInverval);
                intervalAcount += item.itemSpawnChance + 1;
            }
            int itemsToLoot = 1;
            if (!onlyOneItemInBag)
                itemsToLoot = remainingSlotsInCrate;
            
            for (int i = 0; i < itemsToLoot; i++)
            {
                int value = (int) Random.Range(0, intervalAcount);
                
                foreach (var item in itemsIntervalSpawn)
                {
                    if (item.Value.minNumber <= value && item.Value.maxNumber >= value)
                    {
                        if (item.Key.itemName != "EmptyItem")
                        {
                            int randomQuantity = Random.Range(1, 4);
                            if (itemsInLootableObject.ContainsKey(item.Key))
                            {
                                itemsInLootableObject[item.Key] += randomQuantity;
                            }
                            else
                            {
                                itemsInLootableObject.Add(item.Key, randomQuantity);
                                //Debug.Log("KW3: ADDED: " + item.Key + " " + this.gameObject.transform.position) ;
                            }
                            intervalAcount = GenerateNewIntervalCount(item.Key, remainingItems);
                            break;
                        }
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
            InventoryManager.Instance.ChangeText();
            
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
                try
                {
                    this.chestSprite.sprite =
                        LootUIManager.Instance.GetLootSprite(this.chestType, LootSpriteType.Empty);
                }
                catch (Exception e)
                {
                    
                }
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

        public bool CheckIfSlotAvailable()
        {
            int slotAvailable = maxSlotsInCrate;
            foreach (var itemPair in itemsInLootableObject)
            {
                Debug.Log("KW:  for item" + itemPair.Key.itemName + " reducer = " + Reducer(itemPair.Value));
                slotAvailable -= Reducer(itemPair.Value);
            }
            return slotAvailable > 0;
        }

        private int Reducer(float value)
        {
            int reducerValue = 1;
            int max = GameManager.Instance.GetMaxAmountPerSlot();
            switch (value)
            {
                //This means that object has more than 1 slot in use
                case float a when (a / max) > 1 && (a / max) <= 2:
                    Debug.Log("KW REDUCER CASE 2");
                    reducerValue = 2;
                    break;
                case float b when (b / max) > 2 && (b / max) <= 3:
                    reducerValue = 3;
                    break;
                case float c when (c / max) > 3 && (c / max) <= 4:
                    reducerValue = 4;
                    break;
                case float d when (d / max) > 4 && (d / max) <= 5:
                    reducerValue = 5;
                    break;
                case float d when (d / max) > 5 && (d / max) <= 6:
                    reducerValue = 6;
                    break;
                default: reducerValue = 1;
                    break;
            }
            return reducerValue;
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