using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketSlot : MonoBehaviour, IPointerClickHandler
{
    //TODO: Here we have the item
    private UsableItemSO usableItem;

    private Item itemSO; 
    public event EventHandler onItemClicked;
    private Sprite emptySprite;
    [SerializeField] private Image itemSlotImage;
    
    public void SetUpProperties(UsableItemSO itemInSlot)
    {
        usableItem = itemInSlot;
        this.itemSlotImage.sprite = itemInSlot.itemIcon;
    }

    public void ClearMarketSlot()
    {
        this.itemSlotImage.sprite = MarketSystem.Instance.GetEmptySprite();
        this.itemSO = null;
        this.usableItem = null;
        GetComponent<Button>().interactable = false;
    }

    public void SetUpProperties(Item itemSO)
    {
        this.itemSO = itemSO;
        this.itemSlotImage.sprite = itemSO.itemIcon;
    }
    
    public string GetItemName()
    {
        return itemSO == null ? usableItem.itemName : itemSO.itemName;
    }
    
    public UsableItemSO GetUsableItemSO()
    {
        return usableItem;
    }   
    public Item GetItemSO()
    {
        return itemSO;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemSO != null || usableItem != null)
        {
            Debug.Log("ITEM CLICKED");
            onItemClicked?.Invoke(this, EventArgs.Empty); 
        }
    }
}
