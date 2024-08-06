using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;

public class TrainInventoryManager : IInventoryManager
{
    public static TrainInventoryManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TrainInventoryManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void Start()
    {
        base.Start();
        //We load elements from previous raid
        DataPlayerInventory data = SaveManager.Instance.TryLoadPlayerInventoryInBaseJson();
        Dictionary<Item, int> inventory = new Dictionary<Item, int>();
        inventory = GetItemsFromID(data.GetInventory());
        PlayerInventory.Instance.SetInventoryDictionary(inventory);
        if (inventory.Count != 0)
        {
            LoadItemsInPlayerInventory(inventory);
        }
        
        
        //Load base Inventory
    }


    private Dictionary<Item, int>  GetItemsFromID(Dictionary<int, int> itemsID)
    {
        Dictionary<Item, int> items = new Dictionary<Item, int>();
        UnityEngine.Object[] itemsResource = UnityEngine.Resources.LoadAll("Items/Scrap");
        List<Item> itemsList = new List<Item>();
        foreach (var item in itemsResource)
        {
            itemsList.Add(item as Item);
        }
        foreach (var item in itemsID) { items.Add(GetItemFromID(item.Key, itemsList), item.Value); }
        return items;
    }

    private Item GetItemFromID(int id, List<Item> list)
    {
        foreach (var item in list)
        {
            if (item.itemID == id)
            {
                return item;
            }
        }
        return null;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ReverseInventoryStatus();
        }
    }


    public void LoadItemsInPlayerInventory(Dictionary<Item, int> items)
    {
        foreach (var itemPair in items)
        {
            TryAddInventoryToItemSlot(itemPair.Key, itemPair.Value, out int remainingItems);
        }
    }
}
