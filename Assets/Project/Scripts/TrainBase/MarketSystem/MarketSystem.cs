using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketSystem : MonoBehaviour
{
    public static MarketSystem Instance;
    private MarketSlot itemSelected;
    
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject itemSoldTextPrefab;
    [SerializeField] private List<MarketSlot> marketSlots;
    
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
        /*TrainManager.Instance.OnDayChanged += UpdateStoreEvent;
        buyButton.onClick.AddListener(() => BuyItem());

        //TODO: Here we have to put the same store we had before we closed the game
        UpdateStore();
        SubscribeMarketSlotsEvents();*/
    }

    private void UnsubscribeAllEvents()
    {
        foreach (var slot in marketSlots)
        {
            slot.onItemClicked -= OnItemClicked;
        }
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
            if (slot.GetItem() != null)
            {
                slot.onItemClicked += OnItemClicked;
            }
        }
    }
    private void OnItemClicked(object sender, EventArgs e)
    {
        //TODO: Da error
        MarketSlot item = sender as MarketSlot;
        Debug.Log("CLICKED SLOT WITH ITEM: " + item.GetItem().itemName);
    }

    private void UpdateStoreEvent(object sender, EventArgs e)
    {
        UpdateStore();
        SubscribeMarketSlotsEvents();
    }
    
    private void UpdateStore()
    {
        System.Object[] allItems = UnityEngine.Resources.LoadAll("Items/UsableItems");
        List<System.Object> itemsToSpawn = allItems.ToList();
        Debug.Log("UPDATING STORE");
        //TODO: Dependiendo de lo que queramos, aparecerán X Objetos, por ahora vamos a poner 3 o 4
        float numberOfItemsToBuy = UnityEngine.Random.Range(3, 4);

        for (int i = 0; i < numberOfItemsToBuy; i++)
        { 
            //TODO: Cuando pongamos un elemento, debemos de eliminarlo de la lista (Cuando tengamos mas objetos)
            int itemToSpawnIndex = UnityEngine.Random.Range(0, itemsToSpawn.Count);
            UsableItemSO item = allItems.GetValue(itemToSpawnIndex) as UsableItemSO;
            marketSlots[i].SetUpProperties(item);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeAllEvents();
    }

    private void BuyItem()
    {
        if (itemSelected != null)
        {
            Debug.Log("WE CAN BUY ITEM: " + itemSelected.GetItemName());
        }
    }



    
    
    
    
}
