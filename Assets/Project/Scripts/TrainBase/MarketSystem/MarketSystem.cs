using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketSystem : MonoBehaviour
{
    public static MarketSystem Instance;
    private MarketSlot itemSelected;
    private Dictionary<Item, int> itemsInMarket;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject itemSoldTextPrefab;
    [SerializeField] private List<MarketSlot> marketSlots;
    [SerializeField] private GameObject amountSelector;
    [SerializeField] private Sprite emptySprite;
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
        itemsInMarket = new Dictionary<Item, int>();
        TrainManager.Instance.OnDayChanged += UpdateStoreEvent;
        buyButton.onClick.AddListener(() => BuyItem());

        
        //TODO: Here we have to put the same store we had before we closed the game
        LoadCurrentDayStore();
        /*UpdateStore();
        SubscribeMarketSlotsEvents();*/
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
        ItemsDiccionarySave store = SaveManager.Instance.TryLoadCurrentDayStoreJson();
        if (store != null)
        {
            itemsInMarket = TrainInventoryManager.Instance.GetItemsFromID(store.GetInventory());
            int aux = 0;
            foreach (var item in itemsInMarket)
            {
                marketSlots[aux].SetUpProperties(item.Key, item.Value);
                aux++;
            }
        
            for (int i = (int) itemsInMarket.Count; i < marketSlots.Count; i++)
            {
                marketSlots[i].GetComponent<Button>().interactable = false;
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
                slot.onItemClicked += OnItemClicked;
            }
        }
    }
    private void OnItemClicked(object sender, EventArgs e)
    {
        MarketSlot item = sender as MarketSlot;
        itemSelected = item;
        Debug.Log("CLICKED SLOT WITH ITEM: " + item.GetItemSO().itemName);
    }

    private void UpdateStoreEvent(object sender, EventArgs e)
    {
        UpdateStore();
        SubscribeMarketSlotsEvents();
    }

    public void RemoveItemFromList(Item item, int amount)
    {
        if (itemsInMarket.ContainsKey(item))
        {
            itemsInMarket[item] -= amount;
            
            if (itemsInMarket[item] <= 0)
            {
                itemsInMarket.Remove(item);
            }
        }
    }
    
    private void UpdateStore()
    {
        itemsInMarket.Clear();
        Item[] allItems = UnityEngine.Resources.LoadAll<Item>("Items/Market");
        List<Item> itemsToSpawn = allItems.ToList();
        float numberOfItemsToBuy = UnityEngine.Random.Range(2 ,3);

        for (int i = 0; i < numberOfItemsToBuy; i++)
        { 
            int itemToSpawnIndex = UnityEngine.Random.Range(0, itemsToSpawn.Count);
            Item item = itemsToSpawn[itemToSpawnIndex];
            itemsToSpawn.Remove(item);
            
            itemsInMarket.Add(item, 1);
            marketSlots[i].SetUpProperties(item, 1);
        }

        for (int i = (int) numberOfItemsToBuy; i < marketSlots.Count; i++)
        {
            marketSlots[i].GetComponent<Button>().interactable = false;
        }
        
        SaveManager.Instance.SaveCurrentDayStoreJson();
    }

    public Dictionary<Item, int> GetItemsInMarket()
    {
        return itemsInMarket;
    }

    public Sprite GetEmptySprite()
    {
        return emptySprite;
    }

    private void OnDestroy()
    {
        UnsubscribeAllEvents();
    }
    public void ActivateAmountSelector(int maxAmount)
    {
        this.amountSelector.SetActive(true);
        this.amountSelector.GetComponent<BuyAmountSelector>().SetUpProperties(maxAmount, this.itemSelected);
    }
    private void BuyItem()
    {
        if (itemSelected != null)
        {
            if (TrainManager.Instance.resourceAirFilter >= itemSelected.GetItemSO().itemPriceAtMarket)
            {
                if (itemSelected.GetSlotAmount() == 1) //por ahora siempre es 1
                {
                    if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemSelected.GetItemSO(), 1,
                            out int remainingItemsWithoutSpace))
                    {
                        //TODO: Spend Money
                        TrainManager.Instance.resourceAirFilter -= itemSelected.GetItemSO().itemPriceAtMarket;
                        RemoveItemFromList(itemSelected.GetItemSO(), 1);
                        itemSelected.ClearMarketSlot();
                        SaveManager.Instance.SaveCurrentDayStoreJson();
                    }
                    else
                    {
                        Debug.Log("NO SPACE FOR ITEM");
                    }
                }
                else
                {
                    ActivateAmountSelector(this.itemSelected.GetSlotAmount());
                }
            }
        }
    }
    



    
    
    
    
}
