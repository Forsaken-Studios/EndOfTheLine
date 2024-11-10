using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using LootSystem;
using SaveManagerNamespace;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TrainInventoryManager : IInventoryManager
{
    public static TrainInventoryManager Instance;
    
    private int numberOfTools = -1;
    [SerializeField] private GameObject canvasInventory;
    [SerializeField] private TextMeshProUGUI textSwap;
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
        LoadPlayerInventory();
        
        //Load base Inventory
        LoadBaseInventory();
    }
    
    private void Update()
    {
        if (TrainManager.Instance.ValidStatusToOpenInventory() && !WagonScreen())
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ReverseInventoryStatus();
            }
        }
        
        if (inventoryIsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GetInspectViewList().Count != 0)
                {

                    GameObject mostRecentInspectView = inspectListViewList[inspectListViewList.Count - 1];
                    Destroy(mostRecentInspectView);
                    RemoveInspectView(mostRecentInspectView);
                }else if (splittingViewActivated)
                {
                    if(splittingView != null)
                        splittingView.gameObject.SetActive(false);
                    splittingViewActivated = false;
                }
                else
                {
                    ReverseInventoryStatus();
                }
            }
        }
    }

    private bool WagonScreen()
    {
        return TrainManager.Instance.canvasActivated;
    }

    public void OpenInventoryStatusToSell()
    {
        inventoryHUD.SetActive(true);
        SaveManager.Instance.SavePlayerInventoryJson();
        foreach (var itemSlot in itemSlotList)
        {
            itemSlot.ClearItemSlot();
        }
        PlayerInventory.Instance.GetInventoryItems().Clear();
        textSwap.text = "Items To Sell";
    }

    public void OpenInventoryInMarketRoom()
    {
        inventoryHUD.SetActive(true);
        inventoryHUD.transform.Find("BaseInventory").gameObject.SetActive(true);
        inventoryHUD.transform.Find("PlayerInventory").gameObject.SetActive(false);

        Transform expanded1 = inventoryHUD.transform.Find("ExpandedInventory1");
        if(expanded1 != null)
            expanded1.gameObject.SetActive(false);
        
        Transform expanded2 = inventoryHUD.transform.Find("ExpandedInventory2");
        if(expanded2 != null)
            expanded2.gameObject.SetActive(false);
        
        Transform expanded3 = inventoryHUD.transform.Find("ExpandedInventory3");
        if(expanded3 != null)
            expanded3.gameObject.SetActive(false);
    }
    public void CloseInventoryInMarketRoom()
    {
        inventoryHUD.SetActive(false);
        inventoryHUD.transform.Find("BaseInventory").gameObject.SetActive(true);
        inventoryHUD.transform.Find("PlayerInventory").gameObject.SetActive(true);

        Transform expanded1 = inventoryHUD.transform.Find("ExpandedInventory1");
        if(expanded1 != null)
            expanded1.gameObject.SetActive(true);
        
        Transform expanded2 = inventoryHUD.transform.Find("ExpandedInventory2");
        if(expanded2 != null)
            expanded2.gameObject.SetActive(true);
        
        Transform expanded3 = inventoryHUD.transform.Find("ExpandedInventory3");
        if(expanded3 != null)
            expanded3.gameObject.SetActive(true);
    }
    
    
    public void CloseSellingInventory()
    {
        if (PlayerInventory.Instance.GetInventoryItems().Count > 0)
        {
            foreach (var itemSlot in itemSlotList)
            {
                if (itemSlot.GetItemInSlot() != null)
                {
                    if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemSlot.GetItemInSlot(), itemSlot.amount,
                            out int remainingItems))
                    {
                        itemSlot.ClearItemSlot();
                    } 
                }
            } 
        }
        if(inventoryHUD != null)
            inventoryHUD.SetActive(false);
    }
    
    public int GetNumberOfToolsInInventory()
    {
        if (numberOfTools == -1)
        {
            numberOfTools = 0;
            foreach (var itemSlot in itemSlotList)
            {
                if (itemSlot.GetItemInSlot() != null)
                {
                    if (itemSlot.GetItemInSlot().itemID == 8)
                    {
                        numberOfTools += itemSlot.amount;
                    }
                }
               
            }
            return numberOfTools;
        }
        else
        {
            return numberOfTools;
        }
    }
    

    public void LoadPlayerInventory()
    {
        ItemsDiccionarySave data = SaveManager.Instance.TryLoadPlayerInventoryInBaseJson();
        if (data != null)
        {
            Dictionary<Item, int> inventory = new Dictionary<Item, int>();
            inventory = GetItemsFromID(data.GetInventory());
            PlayerInventory.Instance.SetInventoryDictionary(inventory);
            if (inventory.Count != 0)
            {
                LoadItemsInPlayerInventory(inventory);
            }     
        }
    }

    private void LoadBaseInventory()
    {
        DataBaseInventory dataBase = SaveManager.Instance.TryLoadInventoryInBaseJson();
        if (dataBase != null)
        {
            Dictionary<int, ItemInBaseDataSave> baseInventory = new Dictionary<int, ItemInBaseDataSave>();
            baseInventory = dataBase.GetInventory();
            List<Item> itemsList = GetItemList();
            foreach (var itemPair in baseInventory)
            {
                if (itemPair.Value.itemID != 0)
                {
                    Item item = GetItemFromID(itemPair.Value.itemID, itemsList);
                    TrainBaseInventory.Instance.AddItemInXSlot(itemPair.Key, item,
                        itemPair.Value.itemSlotAmount);
                    TrainBaseInventory.Instance.AddItemToList(item, itemPair.Value.itemSlotAmount);
                }
            }
        }

    }


    public Dictionary<Item, int> GetItemsFromID(Dictionary<int, int> itemsID)
    {
        Dictionary<Item, int> items = new Dictionary<Item, int>();
        List<Item> itemsList = GetItemList();
        foreach (var item in itemsID)
        {
            items.Add(GetItemFromID(item.Key, itemsList), item.Value);
        }
        return items;
    }
    public Dictionary<Item, bool> GetItemsFromIDForMarket(Dictionary<int, bool> itemsID)
    {
        Dictionary<Item, bool> items = new Dictionary<Item, bool>();
        List<Item> itemsList = GetItemList();
        foreach (var item in itemsID)
        {
            items.Add(GetItemFromID(item.Key, itemsList), item.Value);
        }
        return items;
    }
    public Dictionary<Item, bool> GetItemsFromIDWithBooleans(Dictionary<int, bool> itemsID)
    {
        Dictionary<Item, bool> items = new Dictionary<Item, bool>();
        List<Item> itemsList = GetItemList();
        foreach (var item in itemsID)
        {
            items.Add(GetItemFromID(item.Key, itemsList), item.Value);
        }
        return items;
    }
    

    public List<Item> GetItemList()
    {
        UnityEngine.Object[] itemsResource = UnityEngine.Resources.LoadAll("Items/Scrap");
        List<Item> itemsList = new List<Item>();
        foreach (var item in itemsResource)
        {
            itemsList.Add(item as Item);
        }

        return itemsList;
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
    public Item GetItemFromID(int id)
    {
        List<Item> list = GetItemList();
        foreach (var item in list)
        {
            if (item.itemID == id)
            {
                return item;
            }
        }
        return null;
    }
    
    public void LoadItemsInPlayerInventory(Dictionary<Item, int> items)
    {
        foreach (var itemPair in items)
        {
            TryAddInventoryToItemSlot(itemPair.Key, itemPair.Value, out int remainingItems);
        }
    }

    public GameObject GetInventoryCanvas()
    {
        return canvasInventory;
    }
}
