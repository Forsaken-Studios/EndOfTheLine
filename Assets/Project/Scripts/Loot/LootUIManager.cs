using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Loot;
using Player;
using UnityEngine;
using Utils.CustomLogs;

public class LootUIManager : MonoBehaviour
{
    public static LootUIManager Instance;
    
    
    [SerializeField] private GameObject lootUIPanel;
    private List<ItemSlot> itemsSlotsList;
    private LooteableObject currentCrateLooting;
    private bool getIfCrateIsOpened;
    private float distanceNeededToClosePanel = 2f;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LootUIManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        itemsSlotsList = new List<ItemSlot>();
        itemsSlotsList = lootUIPanel.GetComponentsInChildren<ItemSlot>().ToList();
        
        lootUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (lootUIPanel.activeSelf)
        {
            //Check if we want to take all items pressing E Button
            if (Input.GetKeyDown(KeyCode.E))
            {
                LootAllItemsInCrate();
            }
            //Check if we need to disable it if we are to far away
            if (currentCrateLooting != null)
            {
                if(Vector2.Distance(PlayerInventory.Instance.transform.position, currentCrateLooting.transform.position) >
                    distanceNeededToClosePanel)
                {
                    LogManager.Log( Vector2.Distance(PlayerInventory.Instance.transform.position, 
                        this.gameObject.transform.position).ToString(), FeatureType.Inventory);
                    DesactivateLootUIPanel();
                    InventoryManager.Instance.DesactivateInventory();
                }
            }

        }
    }

    public void LootAllItemsInCrate()
    {
        currentCrateLooting.LootAllItems();
        DesactivateLootUIPanel();
        InventoryManager.Instance.DesactivateInventory();
        currentCrateLooting = null;
    }
    
    public void LoadItemsInSlots(Dictionary<Item, int> itemsInLootableObject)
    {
        foreach (var item in itemsInLootableObject)
        {
             itemsSlotsList[GetFirstIndexSlotAvailable()].SetItemSlotProperties(item.Key, item.Value);
        }
    }

    public void UnloadItemsInSlots()
    {
        foreach (var itemSlot in itemsSlotsList)
        {
            itemSlot.ClearItemSlot();
        } 
    }
    
    
    public void ActivateLootUIPanel()
    {
        lootUIPanel.SetActive(true);
        getIfCrateIsOpened = true;
    }

    public void DesactivateLootUIPanel()
    {
        lootUIPanel.SetActive(false);
        getIfCrateIsOpened = false;
    }
    
    private int GetFirstIndexSlotAvailable()
    {
        for (int i = 0; i < itemsSlotsList.Count; i++)
        {
            if (itemsSlotsList[i].itemID == 0)
            {
                return i;
            }
        }
        return -1;
    }
    
    public bool GetIfCrateIsOpened()
    {
        return getIfCrateIsOpened;
    }

    public LooteableObject GetCurrentLootableObject()
    {
        return currentCrateLooting;
    }

    public void SetCurrentCrateLooting(LooteableObject aux)
    {
        this.currentCrateLooting = aux;
    }
    
    public void SetPropertiesAndLoadPanel(LooteableObject looteableObject, Dictionary<Item, int> itemsInLootableObject)
    {
        LootUIManager.Instance.UnloadItemsInSlots();
        LootUIManager.Instance.SetCurrentCrateLooting(looteableObject);
        LootUIManager.Instance.LoadItemsInSlots(itemsInLootableObject);
        LootUIManager.Instance.ActivateLootUIPanel();
    }
}
