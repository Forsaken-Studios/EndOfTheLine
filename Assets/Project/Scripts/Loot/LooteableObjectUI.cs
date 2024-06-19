
using System;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.AI;
using Utils.CustomLogs;
/// <summary>
/// NOT USING THIS CLASS
/// </summary>
public class LooteableObjectUI : MonoBehaviour
{
    
    [SerializeField] private GameObject looteableItemsPanel;
    [SerializeField] private GameObject player;
    private List<ItemSlot> itemSlotInBox;
    
    private bool crateIsOpened = false;
    private int availableSlotIndex = 0;
    [Header("Max Slots in this crate")]
    [Tooltip("Variable to control how many slots are in this crate")]
    [SerializeField] private int maxSlotsInCrate = 3;
    private float distanceNeededToClosePanel = 2f;

    private void Awake()
    {
        itemSlotInBox = new List<ItemSlot>(maxSlotsInCrate);
    }

    private void Start()
    {
        /*ItemSlot[] itemSlots = GetComponentsInChildren<ItemSlot>(includeInactive:true);
        foreach (var itemSlot in itemSlots)
        {
            //itemSlot.SetItemSlotProperties(null, 0, 0);
           // itemSlotInBox.Add(itemSlot);
        }*/
    }


    // Update is called once per frame
    void Update()
    {
        /*if (looteableItemsPanel.activeSelf)
        {
             if (Vector2.Distance(player.transform.position, this.gameObject.transform.position) >
                    distanceNeededToClosePanel)
                {
                    DesactivateLooteablePanel();
                    InventoryManager.Instance.DesactivateInventory();
                }
        }*/
    }

    public void AddItemToCrate(Item item, int amount)
    {
        //TODO: Añadir slot y setear la imagen del item Slot
        itemSlotInBox[availableSlotIndex].SetItemSlotProperties(item, amount);
        availableSlotIndex++;
    }
    
    public void ActivateLooteablePanel()
    {
        this.looteableItemsPanel.SetActive(true);
        crateIsOpened = true;
    }

    public void DesactivateLooteablePanel()
    {
        this.looteableItemsPanel.SetActive(false);
        crateIsOpened = false; 
    }

    public bool GetIfCrateIsOpened()
    {
        return crateIsOpened;
    }

    public int GetMaxSlotsInCrate()
    {
        return maxSlotsInCrate;
    }
}
