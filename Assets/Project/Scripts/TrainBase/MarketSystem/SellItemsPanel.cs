using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellItemsPanel : MonoBehaviour
{
    public static SellItemsPanel Instance; 
    
    
    private Button sellButton;
    [SerializeField] private TextMeshProUGUI itemValue;
    private int currentAmount;
    private int previousAmount;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one SellItemsPanel! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        } 
        Instance = this;
    }

    private void Update()
    {
        if (TrainInventoryManager.Instance.GetItemSlotList().Count == 0)
        {
            return;
        }
        
        currentAmount = TrainInventoryManager.Instance.GetItemSlotList().FindAll(t => t.GetItemInSlot() != null).Count;

        if (previousAmount != currentAmount)
        {
            Debug.Log("UPDATE");
            itemValue.text = CalculateTotalPrice().ToString() + " $";
            previousAmount = currentAmount;
        }
    }

    private float CalculateTotalPrice()
    {
        float totalValue = 0;
        foreach (var itemSlot in TrainInventoryManager.Instance.GetItemSlotList())
        {
            if(itemSlot.GetItemInSlot() != null)
                totalValue += itemSlot.GetItemInSlot().itemPriceAtMarket * itemSlot.amount;
        }
        return totalValue;
    }

    private void OnEnable()
    {
        sellButton = GetComponentInChildren<Button>();
        sellButton.onClick.AddListener(() => SellItems());
    }

    private void SellItems()
    {
        foreach (var itemSlot in TrainInventoryManager.Instance.GetItemSlotList())
        {
            if (itemSlot.GetItemInSlot() != null)
            {
                itemSlot.SellItem(); 
            }
        }
    }
    
    
}
