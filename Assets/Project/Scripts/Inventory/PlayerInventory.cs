using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        public bool TryAddItem(Item item, int amount)
        {
            if (InventoryManager.Instance.TryAddInventoryToItemSlot(item, amount))
            {
                if (inventoryItemDictionary.ContainsKey(item))
                {
                    inventoryItemDictionary[item] += amount;
                }
                else
                {
                    inventoryItemDictionary.Add(item, amount);
                }
                ShowItemTaken(item.itemName, amount);
                InventoryManager.Instance.ChangeText(inventoryItemDictionary);
                return true; 
            }
            else
            {
                return false;
            }
        }

        public bool TryAddingItemDragging(Item item, int amount)
        {
            if (inventoryItemDictionary.ContainsKey(item))
            {
                inventoryItemDictionary[item] += amount;
            }
            else
            {
                inventoryItemDictionary.Add(item, amount);
            }
            ShowItemTaken(item.itemName, amount);
            InventoryManager.Instance.ChangeText(inventoryItemDictionary);
            return true; 
        }

        private void ShowItemTaken(string name, int amount)
        {
            if (floatingTextPrefab)
            {
                GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity,
                    this.transform);

                prefab.GetComponentInChildren<TextMeshProUGUI>().text = "x" + amount + " " + name;
            }
        }

        public Dictionary<Item, int> GetInventoryItems()
        {
            return inventoryItemDictionary;
        }
    }
}
