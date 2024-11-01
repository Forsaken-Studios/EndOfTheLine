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
