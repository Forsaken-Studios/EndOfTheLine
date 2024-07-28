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
    private UsableItemSO item;

    public event EventHandler onItemClicked;
    private Sprite emptySprite;
    [SerializeField] private Image itemSlotImage;
   

    public void SetUpProperties(UsableItemSO itemInSlot)
    {
        item = itemInSlot;
        this.itemSlotImage.sprite = itemInSlot.itemIcon;
    }
    
    public string GetItemName()
    {
        return item.itemName;
    }
    
    public UsableItemSO GetItem()
    {
        return item;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ITEM CLICKED");
        onItemClicked?.Invoke(this, EventArgs.Empty);
    }
}
