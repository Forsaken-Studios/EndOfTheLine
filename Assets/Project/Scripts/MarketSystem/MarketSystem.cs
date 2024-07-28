using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketSystem : MonoBehaviour
{
    public static MarketSystem Instance;
    private MarketSlot itemSelected;
    
    [SerializeField] private Button buyButton;

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
        TrainManager.Instance.OnDayChanged += UpdateStoreEvent;
        buyButton.onClick.AddListener(() => BuyItem());

   
    }

    private void UnsubscribeAllEvents()
    {
        foreach (var slot in marketSlots)
        {
            slot.onItemClicked -= OnItemClicked;
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
       // UsableItemSO item = sender as UsableItemSO;
      //  Debug.Log("CLICKED SLOT WITH ITEM: " + item.itemName);
    }

    private void UpdateStoreEvent(object sender, EventArgs e)
    {
        UpdateStore();
        SubscribeMarketSlotsEvents();
    }


    private void UpdateStore()
    {
        System.Object itemNeeded = UnityEngine.Resources.Load("Items/UsableItems/Medkit");
        UsableItemSO medkitTest = itemNeeded as UsableItemSO;
        Debug.Log("UPDATING STORE");
        //TODO: Dependiendo de lo que queramos, aparecerán X Objetos, por ahora vamos a poner 3 o 4
        float numberOfItemsToBuy = UnityEngine.Random.Range(3, 4);

        for (int i = 0; i < numberOfItemsToBuy; i++)
        {
           marketSlots[i].SetUpProperties(medkitTest);
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
