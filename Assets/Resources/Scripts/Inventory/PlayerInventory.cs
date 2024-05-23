using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Tooltip("Variable to link with the script that help us to write on top of the character what item did he take")]
    [SerializeField] private TakeItemText takeItemScript;
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

    public void AddItem(Item item, int amount)
    {
        if (inventoryItemDictionary.ContainsKey(item))
        {
            inventoryItemDictionary[item] += amount;
        }
        else
        {
            inventoryItemDictionary.Add(item, amount);
        }

        takeItemScript.NewItemAddedToInventory(item, amount);
        InventoryManager.Instance.AddInventoryToItemSlot(item, amount);
        
    }

    public Dictionary<Item, int> GetInventoryItems()
    {
        return inventoryItemDictionary;
    }
}
