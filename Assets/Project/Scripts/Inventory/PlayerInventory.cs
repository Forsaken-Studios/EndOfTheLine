using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utils.CustomLogs;

namespace Inventory
{

    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance;

        [SerializeField] private GameObject floatingTextPrefab;

        [Tooltip(
            "Variable to link with the script that help us to write on top of the character what item did he take")]
        [SerializeField]
        private TakeItemText takeItemScript;

        private Dictionary<Item, int> inventoryItemDictionary;
        private float currentWeight;
        private float MAX_WEIGHT = 34.5f;
        private int MAX_INVENTORY_SLOTS = 10;
        private int MAX_STACK_PER_SLOT = 4;

   


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


        void Update()
        {

        }

        public bool TryAddItem(Item item, int amount, out int remainingItemsWithoutSpace, bool showItemsTakenMessage)
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
                Debug.Log("ADDING: " + item.itemWeight * amount );
                AddWeight(item.itemWeight * amount);
                if(showItemsTakenMessage)
                     ShowItemTaken(item.itemName, amount - remainingItemsWithoutSpace);
                InventoryManager.Instance.ChangeText(inventoryItemDictionary);
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
            AddWeight(item.itemWeight * amount);
            if(showMessage)
                ShowItemTaken(item.itemName, amount);
            InventoryManager.Instance.ChangeText(inventoryItemDictionary);
            return true; 
        }

        private void ShowItemTaken(string name, int amount)
        {
            if (floatingTextPrefab)
            {
                GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = "x" + amount + " " + name;
            }
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
                
                DecreaseWeight(item.itemWeight * itemSlotAmount);
                InventoryManager.Instance.ChangeText(inventoryItemDictionary);
            }
        }

        public bool CheckIfPlayerHasKey()
        {
            Object itemNeeded = UnityEngine.Resources.Load("Items/Keycards/Keycard");
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

        private void AddWeight(float weight)
        {
            currentWeight += weight;
        }

        private void DecreaseWeight(float weight)
        {
            currentWeight -= weight;
        }

        public float GetCurrentWeight()
        {
            return currentWeight;
        }

        public float GetMaxWeight()
        {
            return MAX_WEIGHT;
        }
    }
}
