using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using LootSystem;
using SaveManagerNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketSystem : MonoBehaviour
{
    public static MarketSystem Instance;

    [SerializeField] private int minItemsAtMarket = 2;
    [SerializeField] private int maxItemsAtMarket = 4;
    
    private MarketSlot itemSelected;
    private Dictionary<Item, bool> itemsInMarket;
    
    [Header("Hierarchy Properties")]
    [SerializeField] private Button buyButton;
    [SerializeField] private List<MarketSlot> marketSlots;
    [Header("Prefabs")]
    [SerializeField] private GameObject itemSoldTextPrefab;
 

    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[GameManager.cs] : There is already a MarketSystem Instance");
            Destroy(this);
        }
        Instance = this;
    
    }

    private void Start()
    {
        itemsInMarket = new Dictionary<Item, bool>();
        TrainManager.Instance.OnDayChanged += UpdateStoreEvent;
        buyButton.onClick.AddListener(() => BuyItem());
        LoadCurrentDayStore();
    }

    private void UnsubscribeAllEvents()
    {
        foreach (var slot in marketSlots)
        {
            slot.onItemClicked -= OnItemClicked;
        }
    }

    private void LoadCurrentDayStore()
    {
        ItemsBoolDiccionarySave store = SaveManager.Instance.TryLoadCurrentDayStoreJson();
        if (store != null)
        {
            
            itemsInMarket = TrainInventoryManager.Instance.GetItemsFromIDForMarket(store.GetInventory());
            int aux = 0;
            foreach (var item in itemsInMarket)
            {
                marketSlots[aux].SetUpProperties(item.Key, item.Value);
                aux++;
            }
        }
        else
        {
            buyButton.interactable = false;
        }

        SubscribeMarketSlotsEvents();
    }
    
    public void ShowGoldEarnedByItemSold(int goldEarned, ItemSlot itemSlot)
    {
            if (itemSoldTextPrefab)
            {
                GameObject prefab = Instantiate(itemSoldTextPrefab, itemSlot.gameObject.transform.position, Quaternion.identity, 
                    TrainInventoryManager.Instance.GetInventoryCanvas().transform);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = "+" + goldEarned + "$";
            } 
    }
    
    private void SubscribeMarketSlotsEvents()
    {
        foreach (var slot in marketSlots)
        {
            if (slot.GetItemSO() != null || slot.GetUsableItemSO() != null)
            {
                if (!slot.GetIfIsAlreadyBought())
                {
                    slot.onItemClicked += OnItemClicked; 
                }
                else
                {
                    slot.GetComponentInChildren<Button>().interactable = false;
                }
            }
            else
            {
                slot.GetComponentInChildren<Button>().interactable = false;
            }
        }
    }
    private void OnItemClicked(object sender, EventArgs e)
    {
        MarketSlot item = sender as MarketSlot;
        itemSelected = item;
    }

    private void UpdateStoreEvent(object sender, EventArgs e)
    {
        UpdateStore();
        SubscribeMarketSlotsEvents();
    }

    public void RemoveItemFromList(Item item)
    {
        if (itemsInMarket.ContainsKey(item))
        {
            itemsInMarket[item] = true;
        }
    }
    
    private void UpdateStore()
    {
        itemsInMarket.Clear();
        Item[] allItems = UnityEngine.Resources.LoadAll<Item>("Items/Market");
        List<Item> itemsToSpawn = allItems.ToList();
        float numberOfItemsToBuy = UnityEngine.Random.Range(minItemsAtMarket ,maxItemsAtMarket);

        for (int i = 0; i < numberOfItemsToBuy; i++)
        { 
            int itemToSpawnIndex = UnityEngine.Random.Range(0, itemsToSpawn.Count);
            Item item = itemsToSpawn[itemToSpawnIndex];
            itemsToSpawn.Remove(item);
            itemsInMarket.Add(item, false);
            marketSlots[i].SetUpProperties(item, false);
        }
        SaveManager.Instance.SaveCurrentDayStoreJson();
    }

    public Dictionary<Item, bool> GetItemsInMarket()
    {
        return itemsInMarket;
    }
    

    private void OnDestroy()
    {
        UnsubscribeAllEvents();
    }
    private void BuyItem()
    {
        if (itemSelected != null)
        {
            if (TrainManager.Instance.resourceAirFilter >= itemSelected.GetItemSO().itemPriceAtMarket)
            {
                if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemSelected.GetItemSO(), 1,
                        out int remainingItemsWithoutSpace))
                {
                    //TODO: Spend Money
                    TrainManager.Instance.resourceAirFilter -= itemSelected.GetItemSO().itemPriceAtMarket;
                    RemoveItemFromList(itemSelected.GetItemSO());
                    itemSelected.ClearMarketSlot();
                    SaveManager.Instance.SaveCurrentDayStoreJson();
                }
                else
                {
                }
            }
        }
    }
    



    
    
    
    
}
