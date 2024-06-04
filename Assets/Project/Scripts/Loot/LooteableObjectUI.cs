
using System;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class LooteableObjectUI : MonoBehaviour
{
    [SerializeField] private GameObject looteableItemsPanel;
    [SerializeField] private GameObject player;
    private List<ItemSlot> itemInBox;
    
    private bool crateIsOpened = false;
    [Header("Max Slots in this crate")]
    [Tooltip("Variable to control how many slots are in this crate")]
    [SerializeField] private int maxSlotsInCrate = 3;
    private float distanceNeededToClosePanel = 2f;

    private void Awake()
    {
        ItemSlot[] itemSlots = GetComponentsInChildren<ItemSlot>();

        foreach (var itemSlot in itemInBox)
        {
            itemSlot.SetItemSlotProperties(null, 0, 0);
            itemInBox.Add(itemSlot);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if (looteableItemsPanel.activeSelf)
        {
             if (Vector2.Distance(player.transform.position, this.gameObject.transform.position) >
                    distanceNeededToClosePanel)
                {
                    DesactivateLooteablePanel();
                }
        }
    }

    public void AddItemToCrate(Item item, int amount)
    {
        //TODO: Añadir slot y setear la imagen del item Slot
        
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
